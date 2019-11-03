using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Research.Kinect.Nui;
using Microsoft.Research.Kinect.Audio;
using MDI_PlotGraph_Integration.Kinect;
using MDI_PlotGraph_Integration.Component;
using MDI_PlotGraph_Integration.Panels;

namespace MDI_PlotGraph_Integration.Model
{
    public partial class MainModel
    {
        void recognizer_SaidSomething(object sender, SpeechRecognizer.SaidSomethingEventArgs e)
        {
            FlyingText.NewFlyingText(mWindow.theGraphPlot.Graph_Rect.Width / 100,
                new Point(mWindow.theGraphPlot.Graph_Rect.Width / 2, mWindow.theGraphPlot.Graph_Rect.Height / 2), e.Matched);
            switch (e.Verb)
            {
                case SpeechRecognizer.Verbs.Start:
                    mWindow.ControlPan.StartButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
                    break;
                case SpeechRecognizer.Verbs.Stop:
                    mWindow.ControlPan.StopButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
                    break;
                case SpeechRecognizer.Verbs.Reset:
                    mWindow.ControlPan.ResetButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
                    break;
                case SpeechRecognizer.Verbs.Pause:
                    mWindow.Voice_Button.Background = Brushes.Red;
                    mWindow.Voice_Button.Header = "Voice Paused";
                    break;
                case SpeechRecognizer.Verbs.Resume:
                    mWindow.Voice_Button.Background = Brushes.Green;
                    mWindow.Voice_Button.Header = "Voice Running";
                    break;
                case SpeechRecognizer.Verbs.AddLine:
                    mWindow.ControlPan.AddLine.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
                    break;
                case SpeechRecognizer.Verbs.AddSineCurve:
                    mWindow.ControlPan.AddSine.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
                    break;
                case SpeechRecognizer.Verbs.AddParabola:
                    mWindow.ControlPan.AddParabola.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
                    break;
                case SpeechRecognizer.Verbs.EditingMode:
                    mWindow.IndicatorPan.EditingButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
                    break;
                case SpeechRecognizer.Verbs.MatchingMode:
                    mWindow.IndicatorPan.MatchingButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
                    break;
                case SpeechRecognizer.Verbs.TrackingMode:
                    mWindow.IndicatorPan.TrackingButton.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
                    break;
                case SpeechRecognizer.Verbs.NewRandomLine:
                    mWindow.ControlPan.AddLine.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));
                    break;
            }
        }

    }
}
