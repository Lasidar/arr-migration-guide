using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using NLog;
using Readarr.Common.Disk;
using Readarr.Common.EnvironmentInfo;
using Readarr.Common.Extensions;
using Readarr.Core.Backup.Commands;
using Readarr.Core.Configuration;
using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Commands;
using Readarr.Core.Messaging.Events;

namespace Readarr.Core.Backup
{
    public interface IBackupService
    {
        void Backup(BackupType backupType);
        List<BackupResource> GetBackups();
        void DeleteBackup(int id);
        void Restore(int id);
    }

    public class BackupService : IBackupService, IExecute<BackupCommand>
    {
        private readonly IAppFolderInfo _appFolderInfo;
        private readonly IConfigService _configService;
        private readonly IDiskProvider _diskProvider;
        private readonly IDiskTransferService _diskTransferService;
        private readonly IMainDatabase _maindDb;
        private readonly IEventAggregator _eventAggregator;
        private readonly Logger _logger;

        private readonly string _backupTempFolder;

        public BackupService(IAppFolderInfo appFolderInfo,
                            IConfigService configService,
                            IDiskProvider diskProvider,
                            IDiskTransferService diskTransferService,
                            IMainDatabase maindDb,
                            IEventAggregator eventAggregator,
                            Logger logger)
        {
            _appFolderInfo = appFolderInfo;
            _configService = configService;
            _diskProvider = diskProvider;
            _diskTransferService = diskTransferService;
            _maindDb = maindDb;
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
                _diskProvider.CreateZipArchive(_backupTempFolder, backupPath);

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
                    var files = _diskProvider.GetFiles(folder, "readarr_backup_*.zip");
                    
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

            // TODO: Implement restore functionality
            throw new NotImplementedException("Restore functionality not yet implemented");
        }

        private void BackupDatabase()
        {
            _logger.ProgressDebug("Backing up database");

            var databaseFile = _maindDb.GetDatabasePath();
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

            var files = _diskProvider.GetFiles(backupFolder, "readarr_backup_*.zip")
                                    .OrderByDescending(f => _diskProvider.FileGetLastWrite(f))
                                    .Skip(retention)
                                    .ToList();

            foreach (var file in files)
            {
                _logger.Debug("Deleting old backup: {0}", file);
                _diskProvider.DeleteFile(file);
            }
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

    public enum BackupType
    {
        Scheduled = 0,
        Manual = 1,
        Update = 2
    }
}
