using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

namespace MDI_PlotGraph_Integration
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Window_Splash _splash;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // starts splash in separate GUI thread
            StartSplash();

            // continues on loading main application in main gui thread
            LoadMainAppFakeSteps(1000, 3);

            // tells splash screen to start shutting down
            Stop();

            // Creates mainwindow for application
            // The problem is that the mainwindow sometimes fails to activate, 
            // even when user has not touched mouse or keyboard (i.e has not given any other programs or components focus)
            MainWindow = new MainWindow();
            MainWindow.Topmost = true;
            MainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            MainWindow.Show();
            MainWindow.Activate();
            MainWindow.Topmost = false;
        }

        private void StartSplash()
        {
            var thread = new Thread(SplashThread);
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }

        private void SplashThread()
        {
            _splash = new Window_Splash();
            _splash.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _splash.Show();

            System.Windows.Threading.Dispatcher.Run();

            _splash = null;
        }

        private void LoadMainAppFakeSteps(int stepDelayMs, int numSteps)
        {
            for (int i = 1; i <= numSteps; i++)
            {
                Thread.Sleep(stepDelayMs);
            }
        }

        private void Stop()
        {
            if (_splash == null) throw new InvalidOperationException("Not showing splash screen");

            _splash.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
        }
    }
}
