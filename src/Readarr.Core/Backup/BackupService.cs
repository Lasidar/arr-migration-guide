using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json;
using NLog;
using Readarr.Common;
using Readarr.Common.Disk;
using Readarr.Common.EnvironmentInfo;
using Readarr.Common.Extensions;
using Readarr.Common.Instrumentation.Extensions;
using Readarr.Core.Backup.Commands;
using Readarr.Core.Configuration;
using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Commands;
using Readarr.Core.Messaging.Events;
using Readarr.Core.Books;

namespace Readarr.Core.Backup
{
    public interface IBackupService
    {
        void Backup(BackupType backupType);
        List<BackupResource> GetBackups();
        void DeleteBackup(int id);
        void Restore(int id);
        string GetBackupFolder();
    }

    public class BackupService : IBackupService, IExecute<BackupCommand>
    {
        private readonly IMainDatabase _maindDb;
        private readonly IMakeDatabaseBackup _makeDatabaseBackup;
        private readonly IAppFolderInfo _appFolderInfo;
        private readonly IArchiveService _archiveService;
        private readonly IDiskProvider _diskProvider;
        private readonly IDiskTransferService _diskTransferService;
        private readonly IConfigService _configService;
        private readonly IEventAggregator _eventAggregator;
        private readonly Logger _logger;

        private readonly string _backupTempFolder;

        private static readonly Regex BackupFileRegexValue = new Regex(@"readarr_backup_(v[0-9.]+_)?[._0-9]+\.zip", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static Regex BackupFileRegex => BackupFileRegexValue;

        public BackupService(IAppFolderInfo appFolderInfo,
                            IConfigService configService,
                            IDiskProvider diskProvider,
                            IArchiveService archiveService,
                            IDiskTransferService diskTransferService,
                            IMainDatabase maindDb,
                            IMakeDatabaseBackup makeDatabaseBackup,
                            IEventAggregator eventAggregator,
                            Logger logger)
        {
            _appFolderInfo = appFolderInfo;
            _configService = configService;
            _diskProvider = diskProvider;
            _archiveService = archiveService;
            _diskTransferService = diskTransferService;
            _maindDb = maindDb;
            _makeDatabaseBackup = makeDatabaseBackup;
            _eventAggregator = eventAggregator;
            _logger = logger;

            _backupTempFolder = Path.Combine(_appFolderInfo.TempFolder, "readarr_backup");
        }

        public void Backup(BackupType backupType)
        {
            _logger.ProgressInfo("Starting Backup");

            var backupFolder = GetBackupFolder(backupType);
            var backupFilename = string.Format("readarr_backup_v2_{0:yyyy.MM.dd_HH.mm.ss}.zip", DateTime.Now);
            var backupPath = Path.Combine(backupFolder, backupFilename);

            Cleanup();

            try
            {
                BackupDatabase();
                BackupConfigFile();
                CreateVersionInfo();

                _logger.ProgressInfo("Creating backup zip");
                _diskProvider.CreateFolder(_backupTempFolder);
                
                // Get all files in the backup temp folder
                var files = _diskProvider.GetFiles(_backupTempFolder, true);
                _archiveService.CreateZip(backupPath, files);

                _logger.ProgressInfo("Backup completed. File: {0}", backupPath);
                _eventAggregator.PublishEvent(new BackupCompleteEvent(backupPath));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Backup failed");
                throw;
            }
            finally
            {
                Cleanup();
            }

            RemoveOldBackups(backupType, backupFolder);
        }

        public List<BackupResource> GetBackups()
        {
            var backups = new List<BackupResource>();

            foreach (BackupType backupType in Enum.GetValues(typeof(BackupType)))
            {
                var folder = GetBackupFolder(backupType);
                if (_diskProvider.FolderExists(folder))
                {
                    var files = _diskProvider.GetFiles(folder, false)
                        .Where(f => Path.GetFileName(f).StartsWith("readarr_backup_") && f.EndsWith(".zip"));
                    
                    backups.AddRange(files.Select(f => new BackupResource
                    {
                        Id = f.GetHashCode(),
                        Name = Path.GetFileName(f),
                        Path = f,
                        Type = backupType,
                        Size = _diskProvider.GetFileSize(f),
                        Time = _diskProvider.FileGetLastWrite(f)
                    }));
                }
            }

            return backups.OrderByDescending(b => b.Time).ToList();
        }

        public void DeleteBackup(int id)
        {
            var backup = GetBackups().FirstOrDefault(b => b.Id == id);
            
            if (backup != null && _diskProvider.FileExists(backup.Path))
            {
                _diskProvider.DeleteFile(backup.Path);
            }
        }

        public void Restore(int id)
        {
            var backup = GetBackups().FirstOrDefault(b => b.Id == id);
            
            if (backup == null)
            {
                throw new ArgumentException("Backup not found");
            }

            _logger.Info("Restoring backup: {0}", backup.Name);

            var tempRestoreFolder = Path.Combine(_appFolderInfo.TempFolder, "readarr_restore_" + Guid.NewGuid());

            try
            {
                // Extract backup to temp folder
                _archiveService.Extract(backup.Path, tempRestoreFolder);

                // Stop any background jobs during restore
                _eventAggregator.PublishEvent(new ApplicationShutdownRequested());

                // Restore database
                var dbPath = _appFolderInfo.GetDatabase();
                var backupDbPath = Path.Combine(tempRestoreFolder, "readarr.db");
                
                if (_diskProvider.FileExists(backupDbPath))
                {
                    _logger.Info("Restoring database from backup");
                    _diskProvider.CopyFile(backupDbPath, dbPath, true);
                }

                // Restore config file
                var configPath = _appFolderInfo.GetConfigPath();
                var backupConfigPath = Path.Combine(tempRestoreFolder, "config.xml");
                
                if (_diskProvider.FileExists(backupConfigPath))
                {
                    _logger.Info("Restoring config from backup");
                    _diskProvider.CopyFile(backupConfigPath, configPath, true);
                }

                _logger.Info("Backup restored successfully");
                _eventAggregator.PublishEvent(new BackupRestoredEvent());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to restore backup");
                throw new BackupException("Failed to restore backup", ex);
            }
            finally
            {
                // Clean up temp folder
                if (_diskProvider.FolderExists(tempRestoreFolder))
                {
                    _diskProvider.DeleteFolder(tempRestoreFolder, true);
                }
            }
        }

        private void BackupDatabase()
        {
            _logger.ProgressDebug("Backing up database");

            var databaseFile = _appFolderInfo.GetDatabase();
            var tempDatabaseFile = Path.Combine(_backupTempFolder, Path.GetFileName(databaseFile));

            _diskProvider.CreateFolder(_backupTempFolder);
            _diskTransferService.TransferFile(databaseFile, tempDatabaseFile, TransferMode.Copy);
        }

        private void BackupConfigFile()
        {
            _logger.ProgressDebug("Backing up config.xml");

            var configFile = _appFolderInfo.GetConfigPath();
            var tempConfigFile = Path.Combine(_backupTempFolder, "config.xml");

            _diskTransferService.TransferFile(configFile, tempConfigFile, TransferMode.Copy);
        }

        private void CreateVersionInfo()
        {
            var versionInfo = new BackupVersionInfo
            {
                Version = BuildInfo.Version.ToString(),
                CreatedOn = DateTime.UtcNow
            };

            var versionInfoPath = Path.Combine(_backupTempFolder, "version.json");
            var json = JsonSerializer.Serialize(versionInfo, new JsonSerializerOptions { WriteIndented = true });
            
            _diskProvider.WriteAllText(versionInfoPath, json);
        }

        private void Cleanup()
        {
            if (_diskProvider.FolderExists(_backupTempFolder))
            {
                _diskProvider.DeleteFolder(_backupTempFolder, true);
            }
        }

        private string GetBackupFolder(BackupType backupType)
        {
            return backupType switch
            {
                BackupType.Scheduled => _configService.BackupFolder,
                BackupType.Manual => _configService.BackupFolder,
                BackupType.Update => Path.Combine(_appFolderInfo.GetUpdateBackUpFolder(), "readarr_v2"),
                _ => throw new ArgumentException("Unknown backup type")
            };
        }

        private void RemoveOldBackups(BackupType backupType, string backupFolder)
        {
            var retention = _configService.BackupRetention;
            
            if (retention <= 0)
            {
                return;
            }

            var files = _diskProvider.GetFiles(backupFolder, false)
                                    .Where(f => Path.GetFileName(f).StartsWith("readarr_backup_") && f.EndsWith(".zip"))
                                    .OrderByDescending(f => _diskProvider.FileGetLastWrite(f))
                                    .Skip(retention)
                                    .ToList();

            foreach (var file in files)
            {
                _logger.Debug("Deleting old backup: {0}", file);
                _diskProvider.DeleteFile(file);
            }
        }

        public string GetBackupFolder()
        {
            var backupFolder = _configService.BackupFolder;
            
            if (Path.IsPathRooted(backupFolder))
            {
                return backupFolder;
            }
            
            return Path.Combine(_appFolderInfo.GetAppDataPath(), backupFolder);
        }

        public void Execute(BackupCommand message)
        {
            Backup(message.Type);
        }
    }

    public class BackupResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public BackupType Type { get; set; }
        public long Size { get; set; }
        public DateTime Time { get; set; }
    }

    public class BackupVersionInfo
    {
        public string Version { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
