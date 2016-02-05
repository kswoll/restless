using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;
using Restless.Styles;
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

            GlobalStyles.RegisterStyles(Resources);

            var mainWindow = new MainWindow
            {
                Model = new MainWindowModel(SelectFile)
            };
            mainWindow.Show();
        }

        private string SelectFile()
        {
            var dialog = new SaveFileDialog();
            if (dialog.ShowDialog() ?? true)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }
    }
}