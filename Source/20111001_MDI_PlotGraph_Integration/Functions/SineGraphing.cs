using System;
using System.Drawing;
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
using OxyPlot;
using Microsoft.Research.Kinect.Nui;
using Microsoft.Research.Kinect.Audio;
using MDI_PlotGraph_Integration.Model.DataType;

namespace MDI_PlotGraph_Integration.Functions
{
    class SineGraphing
    {

        //Scalers
        private const int A_SCALER = 6;
        private const int W_SCALER = 3;
        private const int O_SCALER = 6;
        private const int D_SCALER = 3;
        private MainWindow mWindow;

     
        public List<SineCurve> mSines;
        private SineCurve mCurrentSelectedSine;

        private int mNextSineIndex = 0;
        private float Aorg;
        private float Worg;
        private float Oorg;
        private float Dorg;

        private float mNewAHolder;
        private float mNewWHolder;
        private float mNewOHolder;
        private float mNewDHolder;

        public SineCurve CurrentSelectedSine
        {
            get { return mCurrentSelectedSine; }
            set { mCurrentSelectedSine = value; }
        }

        // ---------------------------------------------------------------------------------------------
        // ------------------------------  Sine Graphing  ------------------------------------------
        // ---------------------------------------------------------------------------------------------

        public SineGraphing()
        {
            mWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            mSines = new List<SineCurve>();
            mNextSineIndex = 1;
        }

        private void GraphSine()
        {
        }

        public string GraphDefualtSine(int incNumTicksPerSec, int Xmin, int Xmax)
        {
            incNumTicksPerSec /= 2;
            int totalXWidth = Xmax - Xmin;
            SineCurve mSine = new SineCurve("Sine " + mNextSineIndex, mWindow.mMainModel.GenerateRandomColor(), mWindow.mMainModel.mGraphPlot);
            mSine.mNumTicksPerSec = incNumTicksPerSec;
            mNextSineIndex++;
            mSine.a = 1;
            mSine.w = 1;
            mSine.o = 0;
            mSine.d = 0;
            for (int i = 0; i < incNumTicksPerSec * totalXWidth; i++)
            {
                float ticks = i * (1.0f / (float)incNumTicksPerSec) + Xmin;
                System.Windows.Point tempPoint = new System.Windows.Point(ticks, mSine.a * (Math.Sin(mSine.w * ticks - mSine.o)) + mSine.d);
                mSine.AddPoint(tempPoint, mWindow.mMainModel.mGraphPlot);
            }
            mSines.Add(mSine);

            return "Sine " + (mNextSineIndex - 1);

        }


        public void EditSine(SkeletonData incSkeletonData, int incNumTicksPerSec, int Xmin, int Xmax)
        {
            incNumTicksPerSec /= 2;
            int totalXWidth = Xmax - Xmin;
            float LeftX = incSkeletonData.Joints[JointID.HandLeft].Position.X;
            float LeftY = incSkeletonData.Joints[JointID.HandLeft].Position.Y;
            float RightX = incSkeletonData.Joints[JointID.HandRight].Position.X;
            float RightY = incSkeletonData.Joints[JointID.HandRight].Position.Y;

            double a = 0;
            double w = 0;
            double o = 0;
            double d = 0;
           
            for (int i = 0; i < mCurrentSelectedSine.Points.Count; i++)
            {
                float DeltaA = Truncate((RightY - Aorg) * A_SCALER);
                float DeltaW = Truncate((RightX - Worg) * W_SCALER);
                float DeltaO = Truncate((LeftX - Oorg) * O_SCALER * totalXWidth / 10);
                float DeltaD = Truncate((LeftY - Dorg) * D_SCALER * totalXWidth / 10);

                double x;
                double y;

                x = Truncate(i * (1.0f / (float)incNumTicksPerSec)) + Xmin;
                a = Truncate(mCurrentSelectedSine.a + DeltaA);
                w = Truncate(mCurrentSelectedSine.w - DeltaW);
                o = Truncate(mCurrentSelectedSine.o + DeltaO);
                d = Truncate(mCurrentSelectedSine.d + DeltaD);
                y = a* (Math.Sin(w * x - o))+d;

                mCurrentSelectedSine.SetY(mCurrentSelectedSine.Title, i, y, incNumTicksPerSec);
            }

            mNewAHolder = Truncate(a);
            mNewWHolder = Truncate(w);
            mNewOHolder = Truncate(o);
            mNewDHolder = Truncate(d);

            UpdateEquation(mNewAHolder, mNewWHolder, mNewOHolder, mNewDHolder);
        }


        public void SetSelectedSineOrgins(SkeletonData incSkeletonData)
        {
            Aorg = incSkeletonData.Joints[JointID.HandRight].Position.Y;
            Worg = incSkeletonData.Joints[JointID.HandRight].Position.X;
            Oorg = incSkeletonData.Joints[JointID.HandLeft].Position.X;
            Dorg = incSkeletonData.Joints[JointID.HandLeft].Position.Y;
        }

        public void SetSineAWOD()
        {
            mCurrentSelectedSine.a = mNewAHolder;
            mCurrentSelectedSine.w = mNewWHolder;
            mCurrentSelectedSine.o = mNewOHolder;
            mCurrentSelectedSine.d = mNewDHolder;
        }

        public void UpdateEquation(float a, float w, float o, float d)
        {
            string formatA;
            string formatW;
            string formatO;
            string formatD;

                formatA = String.Format("{0:0.00}", a);
                formatW = String.Format("{0:0.00}", w);
                formatO = String.Format("{0:0.00}", o);
                formatD = String.Format("{0:0.00}", d);

            string str = "y = " + formatA + "* sin(" + formatW + "x - " + formatO + ") + " + formatD;
            mWindow.theGraphPlot.EquationText.Text = str;
        }

        private float Truncate(double incNum)
        {
            return (float)Math.Truncate(incNum * 100) / 100;
        }

    }
}
