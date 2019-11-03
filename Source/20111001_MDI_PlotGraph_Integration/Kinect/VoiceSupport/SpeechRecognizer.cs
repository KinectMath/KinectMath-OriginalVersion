using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Microsoft.Research.Kinect.Audio;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.Threading;
using System.Windows.Threading;
using System.Speech.Synthesis;

namespace MDI_PlotGraph_Integration.Kinect
{
    public class SpeechRecognizer
    {
        #region Private state
        private KinectAudioSource kinectAudioSource;
        private SpeechRecognitionEngine sre;
        private bool paused = false;
        TimeSpan readThreshold = new TimeSpan(0, 0, 0, 0, 500);
        private float mConfiLvel = 0.90f;
        private float mLowerConfiLevel = 0.80f;
        #endregion Private state

        #region Accessor
        public float confiLevel
        {
            get { return mConfiLvel; }
            set { mConfiLvel = value; }
        }
        public float lowerConfiLevel
        {
            get { return mLowerConfiLevel; }
            set { mLowerConfiLevel = value; }
        }
        public bool isPaused
        {
            get { return paused; }
            set { paused = value; }
        }
        #endregion Accesoor

        #region Speech Grammar

        #region Grammar Data
        public enum Verbs
        {
            None = 0,
            Start,
            Stop,
            Pause,
            Resume,
            Reset,
            CloseWindow,
            TrackingMode,
            EditingMode,
            MatchingMode,
            AddLine,
            AddParabola,
            AddSineCurve,
            NewRandomLine
        };

        struct WhatSaid
        {
            public Verbs verb;
        }

        Dictionary<string, WhatSaid> InModePhrases = new Dictionary<string, WhatSaid>()
        {
            //{"Start", new WhatSaid()    {verb = Verbs.Start}},
            //{"Stop", new WhatSaid()     {verb = Verbs.Stop}},
            //{"Pause", new WhatSaid()    {verb = Verbs.Pause}},
            //{"Resume", new WhatSaid()   {verb = Verbs.Resume}},
            //{"Reset", new WhatSaid()    {verb = Verbs.Reset}},
            {"Kinect Start", new WhatSaid()    {verb = Verbs.Start}},
            {"Kinect Stop", new WhatSaid()     {verb = Verbs.Stop}},
            {"Kinect Pause", new WhatSaid()    {verb = Verbs.Pause}},
            {"Kinect Unpause", new WhatSaid()   {verb = Verbs.Resume}},
            {"Kinect Reset", new WhatSaid()    {verb = Verbs.Reset}},
            {"Tracking Mode", new WhatSaid()    {verb = Verbs.TrackingMode}},
            {"Editing Mode", new WhatSaid()     {verb = Verbs.EditingMode}},
            {"Matching Mode", new WhatSaid()    {verb = Verbs.MatchingMode}},
            {"New Line", new WhatSaid()   {verb = Verbs.AddLine}},
            {"New Parabola", new WhatSaid()    {verb = Verbs.AddParabola}},
            {"New Random Line", new WhatSaid() {verb = Verbs.NewRandomLine}},
            {"New Sine Curve", new WhatSaid() {verb = Verbs.AddSineCurve}},
            {"New Sine", new WhatSaid() {verb = Verbs.AddSineCurve}}
        };
        #endregion Grammar Data

        private void LoadGrammar(SpeechRecognitionEngine speechRecognitionEngine)
        {
            var InMode = new Choices();
            foreach (var phrase in InModePhrases)
                InMode.Add(phrase.Key);

            var gb = new GrammarBuilder();
            gb.Culture = speechRecognitionEngine.RecognizerInfo.Culture;
            gb.Append(InMode);

            var g = new Grammar(gb);
            speechRecognitionEngine.LoadGrammar(g);
            speechRecognitionEngine.SpeechRecognized += sre_SpeechRecognized;
            speechRecognitionEngine.SpeechHypothesized += sre_SpeechHypothesized;
            speechRecognitionEngine.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(sre_SpeechRecognitionRejected);
        }

        #endregion Speech Grammar

        #region SaidSomethingEventArgs
        public class SaidSomethingEventArgs : EventArgs
        {
            public Verbs Verb { get; set; }
            public string Phrase { get; set; }
            public string Matched { get; set; }
        }

        public event EventHandler<SaidSomethingEventArgs> SaidSomething;
        #endregion SaidSomethingEventArgs

        #region Speech Feedback
        private SpeechSynthesizer speech;
        private bool speechPause = false;
        void speech_SpeakStarted(object sender, SpeakStartedEventArgs e)
        {
            speechPause = true;
        }
        void speech_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            Console.Write("Done");
            Countdown(30, TimeSpan.FromMilliseconds(1), cur => speechPause = cur);
        }
        void Countdown(int count, TimeSpan interval, Action<bool> ts)
        {
            var dt = new System.Windows.Threading.DispatcherTimer();
            dt.Interval = interval;
            dt.Tick += (_, a) =>
            {
                if (count-- == 0)
                {
                    dt.Stop();
                    ts(false);
                }
                else
                {
                    Console.WriteLine(count);
                }
            };
            dt.Start();
        }
        #endregion

        public static SpeechRecognizer Create()
        {
            SpeechRecognizer recognizer = null;

            try
            {
                recognizer = new SpeechRecognizer();
            }
            catch (Exception)
            {
                //speech prereq isn't installed. a null recognizer will be handled properly by the app.
            }
            return recognizer;
        }

        private SpeechRecognizer()
        {
            RecognizerInfo ri = GetKinectRecognizer();
            sre = new SpeechRecognitionEngine(ri);
            LoadGrammar(sre);
        }

        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }

        public void Start(KinectAudioSource kinectSource)
        {
            kinectAudioSource = kinectSource;
            kinectAudioSource.SystemMode = SystemMode.OptibeamArrayOnly;
            kinectAudioSource.FeatureMode = true;
            kinectAudioSource.AutomaticGainControl = false;
            kinectAudioSource.MicArrayMode = MicArrayMode.MicArrayAdaptiveBeam;
            kinectSource.NoiseSuppression = true;
            var kinectStream = kinectAudioSource.Start();
            sre.SetInputToAudioStream(kinectStream, new SpeechAudioFormatInfo(
                                                  EncodingFormat.Pcm, 16000, 16, 1,
                                                  32000, 2, null));
            sre.RecognizeAsync(RecognizeMode.Multiple);

            if (speech == null)
            {
                speech = new SpeechSynthesizer();
                // 0 to 100
                speech.Volume = 100;
                // -10 to 10
                speech.Rate = -2;
                speech.SpeakStarted += new EventHandler<SpeakStartedEventArgs>(speech_SpeakStarted);
                speech.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(speech_SpeakCompleted);
            }
        }

        public void Stop()
        {
            if (sre != null)
            {
                kinectAudioSource.Stop();
                sre.RecognizeAsyncCancel();
                sre.RecognizeAsyncStop();
                kinectAudioSource.Dispose();
            }
            if (speech != null)
                speech = null;
        }

        void sre_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            if (!paused)
            {
                var said = new SaidSomethingEventArgs();
                said.Verb = Verbs.None;
                //said.Matched = "?";
                //Kinect SDK TODO: should make sure after Stop, that we don't get Speech event calls.
                if (SaidSomething != null)
                {
                    SaidSomething(new object(), said);
                }
            }
            Console.WriteLine("\nSpeech Rejected");
        }

        void sre_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Console.Write("\rSpeech Hypothesized: \t{0}", e.Result.Text);
        }

        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.Write("\rSpeech Recognized: \t{0}", e.Result.Text);

            if (speechPause) return;

            if (SaidSomething == null)
                return;

            var said = new SaidSomethingEventArgs();

            if (e.Result.Confidence < mConfiLvel)
            {
                if (e.Result.Confidence > mConfiLvel * mLowerConfiLevel)
                {
                    said.Verb = Verbs.None;
                    //said.Matched = "Say that again?";
                    if (SaidSomething != null)
                    {
                        SaidSomething(new object(), said);
                    }
                }
                return;
            }
            else
            {
                said.Verb = 0;
                said.Phrase = e.Result.Text;
            }

            // Look for a match in the order of the lists below, first match wins.
            List<Dictionary<string, WhatSaid>> allDicts = new List<Dictionary<string, WhatSaid>>() { InModePhrases };

            bool found = false;
            for (int i = 0; i < allDicts.Count && !found; ++i)
            {
                foreach (var phrase in allDicts[i])
                {
                    if (e.Result.Text.Contains(phrase.Key))
                    {
                        said.Verb = phrase.Value.verb;
                        said.Matched = phrase.Key;
                        found = true;
                        break;
                    }
                }
            }

            if (!found)
                return;

            if (paused) // Only accept Unpause
            {
                if ((said.Verb != Verbs.Resume))
                    return;
                paused = false;
            }
            else
            {
                if (said.Verb == Verbs.Resume)
                    return;
            }

            // Speech Feedback
            speech.SpeakAsync(said.Matched);

            if (said.Verb == Verbs.Pause)
                paused = true;

            if (SaidSomething != null)
            {
                SaidSomething(new object(), said);
            }
        }

        public void Pause()
        {
            var said = new SaidSomethingEventArgs();
            said.Verb = Verbs.Pause;
            said.Matched = "Kinect Pause";
            paused = true;
            if (SaidSomething != null)
            {
                SaidSomething(new object(), said);
            }
        }

        public void UnPause()
        {
            var said = new SaidSomethingEventArgs();
            said.Verb = Verbs.Resume;
            said.Matched = "Kinect Unpause";
            paused = false;
            if (SaidSomething != null)
            {
                SaidSomething(new object(), said);
            }
        }
    }
}
