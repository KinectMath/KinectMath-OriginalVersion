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
using MDI_PlotGraph_Integration.Model.DataType;
using MDI_PlotGraph_Integration.Functions;
using OxyPlot;

namespace MDI_PlotGraph_Integration.Model
{
    public partial class MainModel
    {
        private MainWindow mWindow;
        public SkeletonData mSkeleton;
        public KinectDevice mKinect;
        //public VoiceControl mVoiceControl;
        public SpeechRecognizer speechRecognizer = null;

        //Curves
        private Curve mDistanceCurve;
        private Curve mVelocityCurve;
        private Curve mAccelerationCurve;
        private LineGraphing mLineGraphing;
        private ParabolaGraphing mParabolaGraphing;
        private SineGraphing mSineGraphing;
        public Curve mCurrentSelectedCurve;
        private Curve mRandomCurve;

        private double mPreviousDist;
        private double mPreviousVel;

        private bool mAllowLineEdit;
        private bool mAllowParabolaEdit;
        private bool mAllowSineEdit;

        private DispatcherTimer mGraphingTimer;
        public int mTimeTrackTotal = 10;
        private int mTimerTotalTicks = 0;
        private float mNumTicksPerSec = 8.0f;
        private int mNumRandomPoints = 3;

        private ModeState mCurrentModeState;
        public enum ModeState 
        { 
            TrackingMode = 0, 
            EditingMode = 1, 
            MatchingMode = 2 
        };
        
        public ModeState CurrentModeState
        {
            get { return mCurrentModeState; }
        }

        private TrackingState mCurrentTrackingState;
        private enum TrackingState
        {
            NoUser = 0,
            Ready = 1,
            Tracking = 2,
            Finished = 3
        }

        private Joint mCurrentJoint;  // Currently tracked joint
        //private int mTicksPerSecond;  // Number of times to track per sec
        private string mSelectedJoint;

        public GraphPlotModel mGraphPlot;

        private Random mRandom;

        public MainModel()
        {
            mKinect = KinectDevice.Instance;
            mWindow = (MainWindow)System.Windows.Application.Current.MainWindow;

            //mVoiceControl = VoiceControl.Instance();
            speechRecognizer = SpeechRecognizer.Create();         //returns null if problem with speech prereqs or instantiation.
            if (speechRecognizer != null)
            {
                speechRecognizer.Start(new KinectAudioSource());  //KinectSDK TODO: expose Runtime.AudioSource to return correct audiosource.
                speechRecognizer.SaidSomething += new EventHandler<SpeechRecognizer.SaidSomethingEventArgs>(recognizer_SaidSomething);
            }

            mSkeleton = mKinect.mSkeleton;
            mGraphPlot = mWindow.theGraphPlot.vm;
            mCurrentModeState = ModeState.TrackingMode;
            mWindow.IndicatorPan.TrackingButton.IsChecked = true;
            UpdateMode((int)ModeState.TrackingMode);
            mSelectedJoint = "Head";
            ChangeCurrentJoint(mSelectedJoint);
            mLineGraphing = new LineGraphing();
            mParabolaGraphing = new ParabolaGraphing();
            mSineGraphing = new SineGraphing();
            mAllowLineEdit = false;
            mAllowParabolaEdit = false;
            mAllowSineEdit = false;
            mRandom = new Random();
           
        }

        public void Update_Skeleton()
        {
            mSkeleton = mKinect.mSkeleton;
            ChangeCurrentJoint(mSelectedJoint);
            if (mSkeleton != null)
            {
                if (mAllowLineEdit == true)
                {
                    if (NaturalLineEditing)
                        mLineGraphing.EditLineNatural(mSkeleton, (int)mNumTicksPerSec, mGraphPlot.mCurrentXMin, mGraphPlot.mCurrentXMax);
                    else
                        mLineGraphing.EditLine(mSkeleton, (int)mNumTicksPerSec, mGraphPlot.mCurrentXMin, mGraphPlot.mCurrentXMax);
                    mWindow.theGraphPlot.Refresh(true);
                }
                else if (mAllowParabolaEdit == true)
                {
                    mParabolaGraphing.EditParabola(mSkeleton, (int)mNumTicksPerSec, mGraphPlot.mCurrentXMin, mGraphPlot.mCurrentXMax);
                    mWindow.theGraphPlot.Refresh(true);
                }
                else if (mAllowSineEdit == true)
                {
                    mSineGraphing.EditSine(mSkeleton, (int)mNumTicksPerSec, mGraphPlot.mCurrentXMin, mGraphPlot.mCurrentXMax);
                    mWindow.theGraphPlot.Refresh(true);
                }
            }
                
        }

        public void SetJoint(string joint)
        {
            mSelectedJoint = joint;
        }

        public void UpdateMode(int incMode)
        {
            StopTracking();
            switch (incMode)
            {
                case 0:
                    mCurrentModeState = ModeState.TrackingMode;
                    mWindow.ControlPan.StartButton.IsEnabled = true;
                    mWindow.ControlPan.StopButton.IsEnabled = true;
                    mWindow.ControlPan.AddLine.IsEnabled = false;
                    mWindow.ControlPan.AddParabola.IsEnabled = false;
                    mWindow.ControlPan.AddSine.IsEnabled = false;
                    mGraphPlot.ChangeAxes(0, 10, -5, 5);
                    break;
                case 1:
                    mCurrentModeState = ModeState.EditingMode;
                    mWindow.ControlPan.StartButton.IsEnabled = true;
                    mWindow.ControlPan.StopButton.IsEnabled = true;
                    mWindow.ControlPan.AddLine.IsEnabled = true;
                    mWindow.ControlPan.AddParabola.IsEnabled = true;
                    mWindow.ControlPan.AddSine.IsEnabled = true;
                    mGraphPlot.ChangeAxes(-10, 10, -10, 10);
                    break;
                case 2:
                    mCurrentModeState = ModeState.MatchingMode;
                    mWindow.ControlPan.StartButton.IsEnabled = true;
                    mWindow.ControlPan.StopButton.IsEnabled = true;
                    mWindow.ControlPan.AddLine.IsEnabled = true;
                    mWindow.ControlPan.AddParabola.IsEnabled = false;
                    mWindow.ControlPan.AddSine.IsEnabled = false;
                    mGraphPlot.ChangeAxes(0, 10, 0, 5);
                    break;
                default:
                    mCurrentModeState = ModeState.TrackingMode;
                    mWindow.ControlPan.StartButton.IsEnabled = true;
                    mWindow.ControlPan.StopButton.IsEnabled = true;
                    mWindow.ControlPan.AddLine.IsEnabled = false;
                    mWindow.ControlPan.AddParabola.IsEnabled = false;
                    mWindow.ControlPan.AddSine.IsEnabled = false;
                    mGraphPlot.ChangeAxes(0, 10, -5, 5);
                    break;
            }
        }

        public void Start()
        {
           
            switch (mCurrentModeState)
            {
                case ModeState.TrackingMode:
                    ResetTracking();
                    StartTracking();
                    break;
                case ModeState.EditingMode:
                    StartEditing();
                    break;
                case ModeState.MatchingMode:
                    ResetTracking();
                    StartMatching();
                    break;
                default:
                    StartTracking();
                    break;
            }
        }

        public void Stop()
        {
            StopEditing();
            switch (mCurrentModeState)
            {
                case ModeState.TrackingMode:
                    StopTracking();
                    break;
                case ModeState.EditingMode:
                    break;
                case ModeState.MatchingMode:
                    StopTracking();
                    break;
                default:
                    
                    break;
            }
        }

        public void Clear()
        {
            StopTracking();
            mGraphPlot.ClearGraph();
            mWindow.FunctionListBox.Items.Clear();
            ClearTable();
            mWindow.theGraphPlot.EquationText.Text = "";
        }


        public void StartTracking()
        {
            mDistanceCurve = new Curve("Distance", OxyColor.FromArgb(0xFF, 225, 0, 0), mGraphPlot);
            mVelocityCurve = new Curve("Velocity", OxyColor.FromArgb(0xFF, 0, 225, 0), mGraphPlot);
            mAccelerationCurve = new Curve("Acceleration", OxyColor.FromArgb(0xFF, 0, 0, 225), mGraphPlot);

            mGraphPlot.SetLineAndMarkerVisiable("Distance", DistanceVisable);
            mGraphPlot.SetLineAndMarkerVisiable("Velocity", VelocityVisable);
            mGraphPlot.SetLineAndMarkerVisiable("Acceleration", AccelerationVisable);

            mPreviousDist = mCurrentJoint.Position.Z;
            mPreviousVel = Math.Abs(mCurrentJoint.Position.Z - mPreviousDist) / (1 / mNumTicksPerSec);
            mGraphingTimer = new DispatcherTimer();
            mGraphingTimer.Tick += new EventHandler(mGraphingTimer_Tick);
            mGraphingTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)(1000 / mNumTicksPerSec));
            mGraphingTimer.Start();
            mGraphingTimer.IsEnabled = true;
        }

        public void StartEditing()
        {
            if (mCurrentSelectedCurve != null && mSkeleton != null)
            {
                if (mCurrentSelectedCurve.mCurveType == MDI_PlotGraph_Integration.Model.DataType.Curve.CurveType.Line)
                {
                    mAllowLineEdit = true;
                    mLineGraphing.SetSelectedLineOrgins(mSkeleton);
                }
                else if (mCurrentSelectedCurve.mCurveType == MDI_PlotGraph_Integration.Model.DataType.Curve.CurveType.Parabola)
                {
                    mAllowParabolaEdit = true;
                    mParabolaGraphing.SetSelectedParabolaOrgins(mSkeleton);
                }
                else if (mCurrentSelectedCurve.mCurveType == MDI_PlotGraph_Integration.Model.DataType.Curve.CurveType.Sine)
                {
                    mAllowSineEdit = true;
                    mSineGraphing.SetSelectedSineOrgins(mSkeleton);
                }
            }
            
        }

        public void StartMatching()
        {
            mDistanceCurve = new Curve("Distance", OxyColor.FromArgb(0xFF, 225, 0, 0), mGraphPlot);

            mGraphingTimer = new DispatcherTimer();
            mGraphingTimer.Tick += new EventHandler(mGraphingTimer_Tick);
            mGraphingTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)(1000 / mNumTicksPerSec));
            mGraphingTimer.Start();
            mGraphingTimer.IsEnabled = true;
        }

        void mGraphingTimer_Tick(object sender, EventArgs e)
        {
            if (mTimerTotalTicks < mNumTicksPerSec * mTimeTrackTotal)
            {
                float ticks = mTimerTotalTicks * (1/mNumTicksPerSec);
                Point tempPoint;
                if (mCurrentModeState == ModeState.MatchingMode)
                {
                    tempPoint = CalculateDistance();
                    mDistanceCurve.AddPoint(tempPoint, mGraphPlot);
                    mTimerTotalTicks++;
                }
                else
                {
                    tempPoint = CalculateDistance();
                    mDistanceCurve.AddPoint(tempPoint, mGraphPlot);
                    mCurrentSelectedCurve = mDistanceCurve;
                    mWindow.TableListBox.Items.Add(Truncate(tempPoint.X) + "\t" + Truncate(tempPoint.Y));
                    double curDist = tempPoint.Y;

                    tempPoint = CalculateVelocity(curDist);
                    mVelocityCurve.AddPoint(tempPoint, mGraphPlot);
                    double curVel = tempPoint.Y;

                    tempPoint = CalculateAcceleration(curVel);
                    mAccelerationCurve.AddPoint(tempPoint, mGraphPlot);

                    
                    
                    mTimerTotalTicks++;
                    mPreviousVel = curVel;
                    mPreviousDist = mCurrentJoint.Position.Z;
                }

            }
            else
            {
                if (mGraphingTimer != null)
                {
                    mGraphingTimer.Stop();
                    mGraphingTimer.IsEnabled = false;
                }
                mTimerTotalTicks = 0;
            }

            mWindow.theGraphPlot.Refresh(true);
        }

        public void ResetTracking()
        {
            if (mDistanceCurve != null)
            {
                mDistanceCurve.Remove(mGraphPlot);
                RemoveFromFunctionList(mDistanceCurve.Title);
                mDistanceCurve = null;
            }
            if (mAccelerationCurve != null)
            {
                mAccelerationCurve.Remove(mGraphPlot);
                RemoveFromFunctionList(mAccelerationCurve.Title);
                mAccelerationCurve = null;
            }
            if (mVelocityCurve != null)
            {
                mVelocityCurve.Remove(mGraphPlot);
                RemoveFromFunctionList(mVelocityCurve.Title);
                mVelocityCurve = null;
            }
            ClearTable();
            mCurrentTrackingState = TrackingState.Finished;
            if (mGraphingTimer != null)
            {
                mGraphingTimer.Stop();
                mGraphingTimer.IsEnabled = false;
            }
            mTimerTotalTicks = 0;
        }

        public void StopTracking()
        {
            mCurrentTrackingState = TrackingState.Finished;
            if (mGraphingTimer != null)
            {
                mGraphingTimer.Stop();
                mGraphingTimer.IsEnabled = false;
            }
            mTimerTotalTicks = 0;
        }

        private Point CalculateDistance()
        {
            float ticks = mTimerTotalTicks * (1 / mNumTicksPerSec);
            Point tempPoint = new Point(ticks, mCurrentJoint.Position.Z);
            return tempPoint;
        }

        private Point CalculateVelocity(double curDist)
        {
            float ticks = mTimerTotalTicks * (1 / mNumTicksPerSec);
            Point tempPoint = new Point(ticks, (curDist - mPreviousDist) / (1 / mNumTicksPerSec));
            return tempPoint;
        }

        private Point CalculateAcceleration(double curVel)
        {
            float ticks = mTimerTotalTicks * (1 / mNumTicksPerSec);
            Point tempPoint = new Point(ticks, (curVel - mPreviousVel) / (1 / mNumTicksPerSec));
            return tempPoint;
        }

        public void ChangeCurrentJoint(string joint)
        {
            switch (joint)
            {
                case "Head":
                    try
                    {
                        mCurrentJoint = mSkeleton.Joints[JointID.Head];
                    }
                    catch (NullReferenceException)
                    {
                    }
                    break;
                case "RightHand":
                    try
                    {
                        mCurrentJoint = mSkeleton.Joints[JointID.HandRight];
                    }
                    catch (NullReferenceException)
                    {
                    }
                    break;
                case "LeftHand":
                    try
                    {
                        mCurrentJoint = mSkeleton.Joints[JointID.HandLeft];
                    }
                    catch (NullReferenceException)
                    {
                    }
                    break;
                default:
                    try
                    {
                        mCurrentJoint = mSkeleton.Joints[JointID.Head];
                    }
                    catch (NullReferenceException)
                    {
                    }
                    break;
            }
        }

        public void AddLine()
        {
            StopEditing();
            if (ModeState.EditingMode == mCurrentModeState)
            {
                string title;
                title = mLineGraphing.GraphDefualtLine((int)mNumTicksPerSec, mGraphPlot.mCurrentXMin, mGraphPlot.mCurrentXMax);
                ChangeSelectedCurve(title);
                mWindow.theGraphPlot.Refresh(true);
            }
            else if (ModeState.MatchingMode == mCurrentModeState)
            {
                CreateRandomLine();
                mWindow.theGraphPlot.Refresh(true);
            }
        }

        public void AddParabola()
        {
            StopEditing();
            if (ModeState.EditingMode == mCurrentModeState)
            {
                string title;
                title = mParabolaGraphing.GraphDefualtParabola((int)mNumTicksPerSec, mGraphPlot.mCurrentXMin, mGraphPlot.mCurrentXMax);
                ChangeSelectedCurve(title);
                mWindow.theGraphPlot.Refresh(true);
            }
        }

        public void  AddSine()
        {
            StopEditing();
            if (ModeState.EditingMode == mCurrentModeState)
            {
                string title;
                title = mSineGraphing.GraphDefualtSine((int)mNumTicksPerSec, mGraphPlot.mCurrentXMin, mGraphPlot.mCurrentXMax);
                ChangeSelectedCurve(title);
                mWindow.theGraphPlot.Refresh(true);
            }
        }
        
        private float Truncate(double incNum)
        {
            return (float)Math.Truncate(incNum * 100) / 100;
        }

        public void ClearTable()
        {
            for (int i = mWindow.TableListBox.Items.Count - 1; i >= 1; i--)
            {
                mWindow.TableListBox.Items.RemoveAt(i);
            }
        }

        public void ChangeTable(string incTitle)
        {
            ClearTable();
            LineSeries temp = mGraphPlot.FindLinebyTitle(incTitle);
            if (temp != null)
            {
                for (int i = 0; i < temp.Points.Count; i++)
                {
                    OxyPlot.IDataPoint pt = temp.Points.ElementAt(i);

                    mWindow.TableListBox.Items.Add(Truncate(pt.X) + "\t" + Truncate(pt.Y));
                }
            }
        }

        public void ChangeSelectedCurve(string incTitle)
        {
            StopEditing();
            mGraphPlot.DeselectLines();
            mGraphPlot.HighlightSelectedLine(incTitle);
            ChangeTable(incTitle);
            SetCurrentCurveByTitle(incTitle);
        }

        public void SetCurrentCurveByTitle(string incTitle)
        {
            switch (incTitle)
            {
                case "Distance":
                    mCurrentSelectedCurve = mDistanceCurve;
                    mWindow.theGraphPlot.EquationText.Text = "";
                    return;
                case "Velocity":
                    mCurrentSelectedCurve = mVelocityCurve;
                    mWindow.theGraphPlot.EquationText.Text = "";
                    return;
                case "Acceleration":
                    mCurrentSelectedCurve = mAccelerationCurve;
                    mWindow.theGraphPlot.EquationText.Text = "";
                    return;
                default:
                    break;
            }

            for (int i = 0; i < mLineGraphing.mLines.Count; i++)
            {
                LineCurve temp = mLineGraphing.mLines.ElementAt(i) as LineCurve;
                if (incTitle == temp.Title)
                {
                    mCurrentSelectedCurve = temp;
                    mLineGraphing.CurrentSelectedLine = temp;
                    mLineGraphing.UpdateEquation(mLineGraphing.CurrentSelectedLine.m, mLineGraphing.CurrentSelectedLine.b);
                    mParabolaCurveSelected = false;
                    mWindow.theGraphPlot.DisplayEquationFormConvertion(mParabolaCurveSelected);
                    return;
                }
            }

            for (int i = 0; i < mParabolaGraphing.mParabolas.Count; i++)
            {
                ParabolaCurve temp = mParabolaGraphing.mParabolas.ElementAt(i) as ParabolaCurve;
                if (incTitle == temp.Title)
                {
                    mCurrentSelectedCurve = temp;
                    mParabolaGraphing.CurrentSelectedParabola = temp;
                    mParabolaGraphing.UpdateEquation(mParabolaGraphing.CurrentSelectedParabola.a, 
                        mParabolaGraphing.CurrentSelectedParabola.h, mParabolaGraphing.CurrentSelectedParabola.k);
                    mParabolaCurveSelected = true;
                    mWindow.theGraphPlot.DisplayEquationFormConvertion(mParabolaCurveSelected);
                    return;
                }
            }

            for (int i = 0; i < mSineGraphing.mSines.Count; i++)
            {
                SineCurve temp = mSineGraphing.mSines.ElementAt(i) as SineCurve;
                if (incTitle == temp.Title)
                {
                    mCurrentSelectedCurve = temp;
                    mSineGraphing.CurrentSelectedSine = temp;
                    mSineGraphing.UpdateEquation(mSineGraphing.CurrentSelectedSine.a,
                        mSineGraphing.CurrentSelectedSine.w, mSineGraphing.CurrentSelectedSine.o, mSineGraphing.CurrentSelectedSine.d);
                    mParabolaCurveSelected = false;
                    mWindow.theGraphPlot.DisplayEquationFormConvertion(mParabolaCurveSelected);
                    return;
                }
            }

        }
        public void SetFunctionVars()
        {
            if (mCurrentSelectedCurve != null)
            {
                if (mCurrentSelectedCurve.mCurveType == MDI_PlotGraph_Integration.Model.DataType.Curve.CurveType.Line)
                {
                    mLineGraphing.SetLineMB();
                }
                else if (mCurrentSelectedCurve.mCurveType == MDI_PlotGraph_Integration.Model.DataType.Curve.CurveType.Parabola)
                {
                    mParabolaGraphing.SetParabolaAHK();
                }
                else if (mCurrentSelectedCurve.mCurveType == MDI_PlotGraph_Integration.Model.DataType.Curve.CurveType.Sine)
                {
                    mSineGraphing.SetSineAWOD();
                }
            }
        }

        public void StopEditing(){
            if (mAllowLineEdit || mAllowParabolaEdit || mAllowSineEdit)
                SetFunctionVars();
            mAllowLineEdit = false;
            mAllowParabolaEdit = false;
            mAllowSineEdit = false;
        }

        public void RemoveFromFunctionList(string incTitle)
        {
            mWindow.FunctionListBox.Items.Remove(incTitle);
        }

        public OxyColor GenerateRandomColor()
        {
            OxyColor color = OxyColor.FromArgb(0xFF, (byte)GetRandom(0, 255), (byte)GetRandom(0, 255),(byte)GetRandom(0, 255));
            return color;
        }

        public int GetRandom(int f, int l){
            if (f >= l)
                return 0;
            int diff = l - f;
            int value = mRandom.Next(diff);
            value += f;
            return value;
        }

        public void CreateRandomLine()
        {
            RemoveRandomLine();
            float rand;
            mRandomCurve = new Curve("Random Curve", OxyColor.FromArgb(0xFF, 0, 150, 0), mGraphPlot);
            for (int i = 0; i <= mNumRandomPoints; i++){
                rand = GetRandom(100, 400);
                rand = rand/100;
                System.Windows.Point tempPoint = new System.Windows.Point(((double)mTimeTrackTotal / (double)mNumRandomPoints) * (double)i, rand);
                mRandomCurve.AddPoint(tempPoint, mGraphPlot);
            }
            if (SmoothCurve)
                mRandomCurve.Smooth = true;
            
        }

        public void RemoveRandomLine()
        {
            if (mRandomCurve != null)
            {
                mRandomCurve.Remove(mGraphPlot);
                RemoveFromFunctionList(mRandomCurve.Title);
                mRandomCurve = null;

            }
        }

        public void InterpolateRandomLine()
        {
            if (mRandomCurve != null)
            {
                mRandomCurve.Smooth = !mRandomCurve.Smooth;
                SmoothCurve = !SmoothCurve;
                mWindow.theGraphPlot.Refresh(true);
            }
        }

        
    }
}
