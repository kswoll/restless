using System;
using System.Diagnostics;
using System.Windows;
using Restless.ViewModels;
using Restless.Windows.MainWindows;

namespace Restless
{
    public class App : Application
    {
        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [STAThread]
        [DebuggerNonUserCode]
        public static void Main()
        {
            var app = new App();
            app.Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow
            {
                Model = new MainWindowModel()
            };
            mainWindow.Show();
        }
    }
}