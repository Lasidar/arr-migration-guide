using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using Readarr.Common.EnvironmentInfo;
using Readarr.Common.Instrumentation;
using Readarr.Host;
using Readarr.SysTray;

namespace Readarr
{
    public static class WindowsApp
    {
        private static readonly Logger Logger = ReadarrLogger.GetLogger(typeof(WindowsApp));

        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            try
            {
                var startupArgs = new StartupContext(args);

                ReadarrLogger.Register(startupArgs, false, true);

                Bootstrap.Start(args, e => { e.ConfigureServices((_, s) => s.AddSingleton<IHostedService, SystemTrayApp>()); });
            }
            catch (Exception e)
            {
                Logger.Fatal(e, "EPIC FAIL");
                var message = string.Format("{0}: {1}", e.GetType().Name, e.ToString());

                if (RuntimeInfo.IsUserInteractive)
                {
                    MessageBox.Show($"{e.GetType().Name}: {e.Message}", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error, caption: "Epic Fail!");
                }
            }
        }
    }
}
