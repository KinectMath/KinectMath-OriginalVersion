using System;
using System.Collections.Generic;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Research.Kinect.Audio;
using Microsoft.Research.Kinect.Nui;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using MDI_PlotGraph_Integration.Model;
using MDI_PlotGraph_Integration.Kinect;
using MDI_PlotGraph_Integration.Component;
using MDI_PlotGraph_Integration.Windows;

namespace MDI_PlotGraph_Integration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainModel mModel;
        public MainModel mMainModel
        {
            get { return mModel; }
        }

        public Window_Option mOptionWindow;
        public Window_Ink mWhiteboardWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Exit_click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            Window_About.Show(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //AvalonDock.ThemeFactory.ChangeTheme(new Uri("/AvalonDock.Themes;component/themes/dev2010.xaml", UriKind.RelativeOrAbsolute));
            this.FunctionPanel.ToggleAutoHide();
            this.TablePanel.ToggleAutoHide();
            //this.OptionPan.ToggleAutoHide();

            this.KinectCameraPanelInMainWindow.Load_KinectCamera();
            mModel = new MainModel();
            //this.VoiceControl_Button.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
            //
            Win32Timer.timeBeginPeriod(TimerResolution);
            var gameThread = new Thread(GameThread);
            gameThread.SetApartmentState(ApartmentState.MTA);
            gameThread.Start();

            //FlyingText.NewFlyingText(theGraphPlot.Graph_Rect.Width / 100, new Point(theGraphPlot.Graph_Rect.Width / 2, theGraphPlot.Graph_Rect.Height / 2), "Program Start");

            this.theGraphPlot.ShowEquation.IsChecked = mModel.ShowEquation;
            mModel.DisplayEquation(mModel.ShowEquation);
        }

        private void VoiceControl_Click(object sender, RoutedEventArgs e)
        {
            //FlyingText.NewFlyingText(theGraphPlot.Graph_Rect.Width / 100, new Point(theGraphPlot.Graph_Rect.Width / 2, theGraphPlot.Graph_Rect.Height / 2), "Program Start");
            if (mModel.speechRecognizer != null)
            {
                if (mModel.speechRecognizer.isPaused)
                {
                    mModel.speechRecognizer.UnPause();
                }
                else
                {
                    mModel.speechRecognizer.Pause();
                }
            }
        }

        private void FunctionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string temp = this.FunctionListBox.SelectedItem as string;
            mModel.ChangeSelectedCurve(temp); 
        }

        private void FunctionTable_SelectChanged(object sender, RoutedEventArgs e)
        {
            MDI_PlotGraph_Integration.Panels.FunctionTablePane temp = (MDI_PlotGraph_Integration.Panels.FunctionTablePane) sender;
            mModel.mGraphPlot.HighlightSelectedLine(temp.Title);
        }

        private void FunctionTable_LostFocus(object sender, RoutedEventArgs e)
        {
            mModel.mGraphPlot.DeselectLines();
        }

        const double MaxFramerate = 60;
        const double MinFramerate = 15;
        const int TimerResolution = 2;

        bool runningGameThread = false;
        DateTime lastFrameDrawn = DateTime.MinValue;
        DateTime predNextFrame = DateTime.MinValue;
        double actualFrameTime = 0;
        double targetFramerate = MaxFramerate;
        int frameCount = 0;

        private void GameThread()
        {
            runningGameThread = true;
            predNextFrame = DateTime.Now;
            actualFrameTime = 1000.0 / targetFramerate;

            // Try to dispatch at as constant of a framerate as possible by sleeping just enough since
            // the last time we dispatched.
            while (runningGameThread)
            {
                // Calculate average framerate.  
                DateTime now = DateTime.Now;
                if (lastFrameDrawn == DateTime.MinValue)
                    lastFrameDrawn = now;
                double ms = now.Subtract(lastFrameDrawn).TotalMilliseconds;
                actualFrameTime = actualFrameTime * 0.95 + 0.05 * ms;
                lastFrameDrawn = now;

                // Adjust target framerate down if we're not achieving that rate
                frameCount++;
                if (((frameCount % 100) == 0) && (1000.0 / actualFrameTime < targetFramerate * 0.92))
                    targetFramerate = Math.Max(MinFramerate, (targetFramerate + 1000.0 / actualFrameTime) / 2);

                if (now > predNextFrame)
                    predNextFrame = now;
                else
                {
                    double msSleep = predNextFrame.Subtract(now).TotalMilliseconds;
                    if (msSleep >= TimerResolution)
                        Thread.Sleep((int)(msSleep + 0.5));
                }
                predNextFrame += TimeSpan.FromMilliseconds(1000.0 / targetFramerate);

                Dispatcher.Invoke(DispatcherPriority.Send,
                    new Action<int>(HandleGameTimer), 0);
            }
        }

        private void HandleGameTimer(int param)
        {
            theGraphPlot.GraphPlotCanvas.Children.Clear();
            FlyingText.Draw(theGraphPlot.GraphPlotCanvas.Children);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            runningGameThread = false;
        }

        private void Option_Click(object sender, RoutedEventArgs e)
        {
            String s = ((MenuItem)sender).Name;
            Window_Option.Show(this, s);
        }

        private void Youtube_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("http://www.youtube.com/playlist?list=PL2CD6A2E21B3C5824")); 
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "TableData"; // Default file name
            dlg.DefaultExt = ".text"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            string theData = "";
            for (int i = 1; i < TableListBox.Items.Count; i++)
            {
                theData += TableListBox.Items[i];
                theData += "\n";
            }
            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                using (Stream sw = (Stream)dlg.OpenFile())
                {

                    byte[] data = (new UTF8Encoding(true)).GetBytes(theData);
                    sw.Write(data, 0, data.Length);
                    sw.Close();
                }
            }
        }

        private void WhiteBoard_Click(object sender, RoutedEventArgs e)
        {
            Window_Ink tmp;
            tmp = new Window_Ink();
            tmp.Owner = this;
            tmp.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            tmp.Show();
        }

    }
}

// Since the timer resolution defaults to about 10ms precisely, we need to
// increase the resolution to get framerates above between 50fps with any
// consistency.
public class Win32Timer
{
    [DllImport("Winmm.dll")]
    public static extern int timeBeginPeriod(UInt32 uPeriod);
}
