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
using System.Threading;
using Microsoft.Research.Kinect.Nui;
using Microsoft.Research.Kinect.Audio;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.IO;

namespace MDI_PlotGraph_Integration.Kinect
{
    public sealed class VoiceControl
    {
        private const float CONFI_LEVL = 0.95f;
 
        KinectAudioSource kinectSource;
        private Thread t;
        SpeechRecognitionEngine speechEngine;
        Stream stream;
        string RecognizerId = "SR_MS_en-US_Kinect_10.0";
        TimeSpan readThreshold = new TimeSpan(0, 0, 0, 0, 100);
        private bool mEnabled;

        public bool isEnabled
        {
            set 
            { 
                mEnabled = value;
                if (value)
                {
                    Start();
                }
                else
                {
                    End();
                }
            }
            get { return mEnabled; }
        }

        #region Singlton Pattern
        private static VoiceControl instance;
        public static VoiceControl Instance()
        {
            if (instance == null)
                instance = new VoiceControl();
            return instance;
        }
        private VoiceControl()
        {
        }
        public void Start()
        {
            if (t == null)

            {
                t = new Thread(Initalize);
                t.SetApartmentState(ApartmentState.MTA);
                t.Start();
                t.Priority = ThreadPriority.Highest;
            }
        }
        public void End()
        {
            if (t != null)
            {
                kinectSource.Stop();
                t.Abort();
                t = null;
            }
        }
        #endregion

        private void Initalize()
        {
            kinectSource = new KinectAudioSource();
            kinectSource.FeatureMode = true;
            kinectSource.AutomaticGainControl = false;
            kinectSource.SystemMode = SystemMode.OptibeamArrayOnly;
            kinectSource.MicArrayMode = MicArrayMode.MicArrayAdaptiveBeam;
            kinectSource.NoiseSuppression = true;
            var rec = (from r in SpeechRecognitionEngine.InstalledRecognizers() where r.Id == RecognizerId select r).FirstOrDefault();
            speechEngine = new SpeechRecognitionEngine(rec.Id);

            // Word Choice
            var choices = new Choices();
            choices.Add("Kinect Reset");
            choices.Add("Kinect Start");
            choices.Add("Kinect Stop");
            choices.Add("Close Window");
            //choices.Add("Line One");
            //choices.Add("Line Two");
            //choices.Add("Line Three");
            //choices.Add("Line Four");
            //choices.Add("Line Five");
            //choices.Add("Parabola One");
            //choices.Add("Parabola Two");
            //choices.Add("Parabola Three");
            //choices.Add("Parabola Four");
            //choices.Add("Parabola Five");
            choices.Add("Tracking Mode");
            choices.Add("Editing Mode");
            choices.Add("Matching Mode");
            //choices.Add("Visible");
            //choices.Add("Invisible");
            choices.Add("Select Head");
            choices.Add("Select Right Hand");
            choices.Add("Select Left Hand");
            choices.Add("Add Line");
            choices.Add("Add Parabola");

            GrammarBuilder gb = new GrammarBuilder();
            gb.Culture = rec.Culture;
            gb.Append(choices);

            var g = new Grammar(gb);

            speechEngine.LoadGrammar(g);
            speechEngine.SpeechHypothesized += new EventHandler<SpeechHypothesizedEventArgs>(sre_SpeechHypothesized);
            speechEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);
            speechEngine.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(sre_SpeechRecognitionRejected);

            stream = kinectSource.Start(readThreshold);

            speechEngine.SetInputToAudioStream(stream,
              new SpeechAudioFormatInfo(
                  EncodingFormat.Pcm, 16000, 16, 1,
                  32000, 2, null));


            speechEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void sre_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Console.Write("\rSpeech Hypothesized: \t{0}", e.Result.Text);
            //Application.Current.Dispatcher.BeginInvoke(
            //    System.Windows.Threading.DispatcherPriority.Normal,
            //    new Action(
            //        delegate()
            //        {
            //            MainWindow t = (MainWindow)Application.Current.MainWindow;
            //            t.VoiceCommandDisplay.Text = " Speech Hypothesized: " + e.Result.Text + "\n Conf: " + e.Result.Confidence;
            //            t.VoiceCommandDisplay.Background = System.Windows.Media.Brushes.Orange;
            //        }
            //    )
            //);
        }

        private void sre_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.Write("\rSpeech Rejected: \t{0}", e.Result.Text);
            //Application.Current.Dispatcher.BeginInvoke(
            //    System.Windows.Threading.DispatcherPriority.Normal,
            //    new Action(
            //        delegate()
            //        {
            //            MainWindow t = (MainWindow)Application.Current.MainWindow;
            //            t.VoiceCommandDisplay.Text = " Speech Rejected: " + e.Result.Text + "\n Conf: " + e.Result.Confidence;
            //            t.VoiceCommandDisplay.Background = System.Windows.Media.Brushes.Gray;
            //        }
            //    )
            //);
        }

        private void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence > CONFI_LEVL)
            {
                switch (e.Result.Text)
                {
                    case "Tracking Mode":
                        Application.Current.Dispatcher.BeginInvoke(
                            System.Windows.Threading.DispatcherPriority.Send,
                            new Action(
                                delegate()
                                {
                                    MainWindow t = (MainWindow)Application.Current.MainWindow;
                                    t.IndicatorPan.TrackingButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                                }
                            )
                        );
                        break;
                    case "Editing Mode":
                        Application.Current.Dispatcher.BeginInvoke(
                            System.Windows.Threading.DispatcherPriority.Send,
                            new Action(
                                delegate()
                                {
                                    MainWindow t = (MainWindow)Application.Current.MainWindow;
                                    t.IndicatorPan.EditingButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                                }
                            )
                        );
                        break;
                    case "Matching Mode":
                        Application.Current.Dispatcher.BeginInvoke(
                            System.Windows.Threading.DispatcherPriority.Send,
                            new Action(
                                delegate()
                                {
                                    MainWindow t = (MainWindow)Application.Current.MainWindow;
                                    t.IndicatorPan.MatchingButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                                }
                            )
                        );
                        break;
                    case "Close Window":
                        Application.Current.Dispatcher.BeginInvoke(
                            System.Windows.Threading.DispatcherPriority.Send,
                            new Action(
                                delegate()
                                {
                                    MainWindow t = (MainWindow)Application.Current.MainWindow;
                                    t.Close();
                                }
                            )
                        );
                        break;
                    case "Kinect Reset":
                        Application.Current.Dispatcher.BeginInvoke(
                            System.Windows.Threading.DispatcherPriority.Send,
                            new Action(
                                delegate()
                                {
                                    MainWindow t = (MainWindow)Application.Current.MainWindow;
                                    t.ControlPan.ResetButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                                }
                            )
                        );
                        break;
                    case "Kinect Start":
                        Application.Current.Dispatcher.BeginInvoke(
                            System.Windows.Threading.DispatcherPriority.Send,
                            new Action(
                                delegate()
                                {
                                    MainWindow t = (MainWindow)Application.Current.MainWindow;
                                    t.ControlPan.StartButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                                }
                            )
                        );
                        break;
                    case "Kinect Stop":
                        Application.Current.Dispatcher.BeginInvoke(
                            System.Windows.Threading.DispatcherPriority.Send,
                            new Action(
                                delegate()
                                {
                                    MainWindow t = (MainWindow)Application.Current.MainWindow;
                                    t.ControlPan.StopButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                                }
                            )
                        );
                        break;
                    case "Add Line":
                        Application.Current.Dispatcher.BeginInvoke(
                            System.Windows.Threading.DispatcherPriority.Send,
                            new Action(
                                delegate()
                                {
                                    MainWindow t = (MainWindow)Application.Current.MainWindow;
                                    t.ControlPan.AddLine.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                                }
                            )
                        );
                        break;
                    case "Add Parabola":
                        Application.Current.Dispatcher.BeginInvoke(
                            System.Windows.Threading.DispatcherPriority.Send,
                            new Action(
                                delegate()
                                {
                                    MainWindow t = (MainWindow)Application.Current.MainWindow;
                                    t.ControlPan.AddParabola.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                                }
                            )
                        );
                        break;
                    case "Select Head":
                        //Application.Current.Dispatcher.BeginInvoke(
                        //    System.Windows.Threading.DispatcherPriority.Send,
                        //    new Action(
                        //        delegate()
                        //        {
                        //            MainWindow t = (MainWindow)Application.Current.MainWindow;
                        //            t.OptionPan.RadioButtonHead.RaiseEvent(new RoutedEventArgs(RadioButton.ClickEvent));
                        //        }
                        //    )
                        //);
                        break;
                    case "Select Right Hand":
                        //Application.Current.Dispatcher.BeginInvoke(
                        //    System.Windows.Threading.DispatcherPriority.Send,
                        //    new Action(
                        //        delegate()
                        //        {
                        //            MainWindow t = (MainWindow)Application.Current.MainWindow;
                        //            t.OptionPan.RadioButtonRightHand.RaiseEvent(new RoutedEventArgs(RadioButton.ClickEvent));
                        //        }
                        //    )
                        //);
                        break;
                    case "Select Left Hand":
                        //Application.Current.Dispatcher.BeginInvoke(
                        //    System.Windows.Threading.DispatcherPriority.Send,
                        //    new Action(
                        //        delegate()
                        //        {
                        //            MainWindow t = (MainWindow)Application.Current.MainWindow;
                        //            t.OptionPan.RadioButtonLeftHand.RaiseEvent(new RoutedEventArgs(RadioButton.ClickEvent));
                        //        }
                        //    )
                        //);
                        break;
                    default:
                        break;
                }
                // Feedback
                Console.Write("\rSpeech Recognized: \t{0} \n", e.Result.Text);
            }
        }

        //private void Mode(SpeechRecognizedEventArgs e)
        //{
        //    switch (mCurrentModeState)
        //    {
        //        case ModeState.TrackingMode:
        //            Mode_Track_Joint(e);
        //            switch (e.Result.Text)
        //            {
        //                case "Start":
        //                    Application.Current.Dispatcher.BeginInvoke(
        //                        System.Windows.Threading.DispatcherPriority.Normal,
        //                        new Action(
        //                            delegate()
        //                            {
        //                                MainWindow t = (MainWindow)Application.Current.MainWindow;
        //                                t.mModel.StartGraphing();
        //                            }
        //                        )
        //                    );
        //                    break;
        //                case "Stop":
        //                    break;
        //                case "Reset":
        //                    Application.Current.Dispatcher.BeginInvoke(
        //                        System.Windows.Threading.DispatcherPriority.Normal,
        //                        new Action(
        //                            delegate()
        //                            {
        //                                MainWindow t = (MainWindow)Application.Current.MainWindow;
        //                                t.mModel.ClearGraph();
        //                                t.mModel.ResetGraph();
        //                            }
        //                        )
        //                    );
        //                    break;
        //            }
        //            break;
        //        case ModeState.EditingMode:
        //            Mode_Line(e);
        //            switch (e.Result.Text)
        //            {
        //                case "Line One":
        //                    Application.Current.Dispatcher.BeginInvoke(
        //                        System.Windows.Threading.DispatcherPriority.Normal,
        //                        new Action(
        //                            delegate()
        //                            {
        //                                MainWindow t = (MainWindow)Application.Current.MainWindow;
        //                                t.mModel.CurrentLineSelected = 0;
        //                                t.mModel.mGraph.SelectLine("Line: 1");
        //                                t.mModel.mGraph.ResetAllCurveThickness(2);
        //                                t.mModel.mGraph.ChangeCurveThickness("Line: 1", 6);
        //                                mCurrentEditState = EditState.Line;
        //                                t.mModel.SetCurrentEquation(true, t.mModel.CurrentLineSelected);
        //                            }
        //                        )
        //                    );
        //                    break;

        //                case "Line Two":
        //                    Application.Current.Dispatcher.BeginInvoke(
        //                         System.Windows.Threading.DispatcherPriority.Normal,
        //                         new Action(
        //                             delegate()
        //                             {
        //                                 MainWindow t = (MainWindow)Application.Current.MainWindow;
        //                                 t.mModel.CurrentLineSelected = 1;
        //                                 t.mModel.mGraph.SelectLine("Line: 2");
        //                                 t.mModel.mGraph.ResetAllCurveThickness(2);
        //                                 t.mModel.mGraph.ChangeCurveThickness("Line: 2", 6);
        //                                 mCurrentEditState = EditState.Line;
        //                                 t.mModel.SetCurrentEquation(true, t.mModel.CurrentLineSelected);
        //                             }
        //                         )
        //                     );
        //                    break;

        //                case "Line Three":
        //                    Application.Current.Dispatcher.BeginInvoke(
        //                        System.Windows.Threading.DispatcherPriority.Normal,
        //                        new Action(
        //                            delegate()
        //                            {
        //                                MainWindow t = (MainWindow)Application.Current.MainWindow;
        //                                t.mModel.CurrentLineSelected = 2;
        //                                t.mModel.mGraph.SelectLine("Line: 3");
        //                                t.mModel.mGraph.ResetAllCurveThickness(2);
        //                                t.mModel.mGraph.ChangeCurveThickness("Line: 3", 6);
        //                                mCurrentEditState = EditState.Line;
        //                                t.mModel.SetCurrentEquation(true, t.mModel.CurrentLineSelected);
        //                            }
        //                        )
        //                    );
        //                    break;
        //                case "Parabola One":
        //                    Application.Current.Dispatcher.BeginInvoke(
        //                        System.Windows.Threading.DispatcherPriority.Normal,
        //                        new Action(
        //                            delegate()
        //                            {
        //                                MainWindow t = (MainWindow)Application.Current.MainWindow;
        //                                t.mModel.CurrentParabolaSelected = 0;
        //                                t.mModel.mGraph.SelectLine("Parabola: 1");
        //                                t.mModel.mGraph.ResetAllCurveThickness(2);
        //                                t.mModel.mGraph.ChangeCurveThickness("Parabola: 1", 6);
        //                                mCurrentEditState = EditState.Parabola;
        //                                t.mModel.SetCurrentEquation(false, t.mModel.CurrentParabolaSelected);
        //                            }
        //                        )
        //                    );
        //                    break;

        //                case "Parabola Two":
        //                    Application.Current.Dispatcher.BeginInvoke(
        //                         System.Windows.Threading.DispatcherPriority.Normal,
        //                         new Action(
        //                             delegate()
        //                             {
        //                                 MainWindow t = (MainWindow)Application.Current.MainWindow;
        //                                 t.mModel.CurrentParabolaSelected = 1;
        //                                 t.mModel.mGraph.SelectLine("Parabola: 2");
        //                                 t.mModel.mGraph.ResetAllCurveThickness(2);
        //                                 t.mModel.mGraph.ChangeCurveThickness("Parabola: 2", 6);
        //                                 mCurrentEditState = EditState.Parabola;
        //                                 t.mModel.SetCurrentEquation(false, t.mModel.CurrentParabolaSelected);
        //                             }
        //                         )
        //                     );
        //                    break;

        //                case "Parabola Three":
        //                    Application.Current.Dispatcher.BeginInvoke(
        //                        System.Windows.Threading.DispatcherPriority.Normal,
        //                        new Action(
        //                            delegate()
        //                            {
        //                                MainWindow t = (MainWindow)Application.Current.MainWindow;
        //                                t.mModel.CurrentParabolaSelected = 2;
        //                                t.mModel.mGraph.SelectLine("Parabola: 3");
        //                                t.mModel.mGraph.ResetAllCurveThickness(2);
        //                                t.mModel.mGraph.ChangeCurveThickness("Parabola: 3", 6);
        //                                mCurrentEditState = EditState.Parabola;
        //                                t.mModel.SetCurrentEquation(false, t.mModel.CurrentParabolaSelected);
        //                            }
        //                        )
        //                    );
        //                    break;

        //                case "Add Line":
        //                    Application.Current.Dispatcher.BeginInvoke(
        //                        System.Windows.Threading.DispatcherPriority.Normal,
        //                        new Action(
        //                            delegate()
        //                            {
        //                                MainWindow t = (MainWindow)Application.Current.MainWindow;
        //                                t.mModel.GraphDefualtLine();
        //                            }
        //                        )
        //                    );
        //                    break;
        //                case "Add Parabola":
        //                    Application.Current.Dispatcher.BeginInvoke(
        //                        System.Windows.Threading.DispatcherPriority.Normal,
        //                        new Action(
        //                            delegate()
        //                            {
        //                                MainWindow t = (MainWindow)Application.Current.MainWindow;
        //                                t.mModel.GraphDefualtParabola();
        //                            }
        //                        )
        //                    );
        //                    break;
        //            }
        //            break;
        //        case ModeState.MatchingMode:

        //            break;
        //    }
        //}

        //private void Mode_Track_Joint(SpeechRecognizedEventArgs e)
        //{
        //    switch (e.Result.Text)
        //    {
        //        case "Select Head":
        //            Application.Current.Dispatcher.BeginInvoke(
        //                System.Windows.Threading.DispatcherPriority.Normal,
        //                new Action(
        //                    delegate()
        //                    {
        //                        MainWindow t = (MainWindow)Application.Current.MainWindow;
        //                        t.RadioButtonHead.IsChecked = true;
        //                    }
        //                )
        //            );
        //            break;
        //        case "Select Right Hand":
        //            Application.Current.Dispatcher.BeginInvoke(
        //                System.Windows.Threading.DispatcherPriority.Normal,
        //                new Action(
        //                    delegate()
        //                    {
        //                        MainWindow t = (MainWindow)Application.Current.MainWindow;
        //                        t.RadioButtonRightHand.IsChecked = true;
        //                    }
        //                )
        //            );
        //            break;
        //        case "Select Left Hand":
        //            Application.Current.Dispatcher.BeginInvoke(
        //                System.Windows.Threading.DispatcherPriority.Normal,
        //                new Action(
        //                    delegate()
        //                    {
        //                        MainWindow t = (MainWindow)Application.Current.MainWindow;
        //                        t.RadioButtonLeftHand.IsChecked = true;
        //                    }
        //                )
        //            );
        //            break;

        //    }
        //}
        
        //private void Mode_Line(SpeechRecognizedEventArgs e)
        //{
        //    switch (e.Result.Text)
        //    {
        //        case "Start":
        //            switch (mCurrentEditState)
        //            {
        //                case EditState.Line:
        //                    Application.Current.Dispatcher.BeginInvoke(
        //                        System.Windows.Threading.DispatcherPriority.Normal,
        //                        new Action(
        //                            delegate()
        //                            {
        //                                MainWindow t = (MainWindow)Application.Current.MainWindow;
        //                                t.mModel.EnableEditLine = true;
        //                                t.mModel.SetSelectedLineOrgins();
        //                            }
        //                        )
        //                    );
        //                    break;
        //                case EditState.Parabola:
        //                    Application.Current.Dispatcher.BeginInvoke(
        //                        System.Windows.Threading.DispatcherPriority.Normal,
        //                        new Action(
        //                            delegate()
        //                            {
        //                                MainWindow t = (MainWindow)Application.Current.MainWindow;
        //                                t.mModel.EnableEditParabola = true;
        //                                t.mModel.SetSelectedParabolaOrgins();
        //                            }
        //                        )
        //                    );
        //                    break;
        //            }
        //            break;
        //        case "Stop":
        //            switch (mCurrentEditState)
        //            {
        //                case EditState.Line:
        //                    Application.Current.Dispatcher.BeginInvoke(
        //                        System.Windows.Threading.DispatcherPriority.Normal,
        //                        new Action(
        //                            delegate()
        //                            {
        //                                MainWindow t = (MainWindow)Application.Current.MainWindow;
        //                                t.mModel.EnableEditLine = false;
        //                                t.mModel.SetLineMB();
        //                            }
        //                        )
        //                    );
        //                    break;
        //                case EditState.Parabola:
        //                    Application.Current.Dispatcher.BeginInvoke(
        //                        System.Windows.Threading.DispatcherPriority.Normal,
        //                        new Action(
        //                            delegate()
        //                            {
        //                                MainWindow t = (MainWindow)Application.Current.MainWindow;
        //                                t.mModel.EnableEditParabola = false;
        //                                t.mModel.SetParabolaAHK();
        //                            }
        //                        )
        //                    );
        //                    break;
        //            }
        //            break;
        //        case "Visible":
        //            Application.Current.Dispatcher.BeginInvoke(
        //                System.Windows.Threading.DispatcherPriority.Normal,
        //                new Action(
        //                    delegate()
        //                    {
        //                        MainWindow t = (MainWindow)Application.Current.MainWindow;
        //                        t.mModel.SetCurrentLineVisible(true);
        //                    }
        //                )
        //            );
        //            break;
        //        case "Invisible":
        //            Application.Current.Dispatcher.BeginInvoke(
        //                System.Windows.Threading.DispatcherPriority.Normal,
        //                new Action(
        //                    delegate()
        //                    {
        //                        MainWindow t = (MainWindow)Application.Current.MainWindow;
        //                        t.mModel.SetCurrentLineVisible(false);
        //                    }
        //                )
        //            );
        //            break;
        //    }
        //}

    }
}