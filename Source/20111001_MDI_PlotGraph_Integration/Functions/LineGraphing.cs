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
    public class LineGraphing
    {
        private const int M_SCALER = 4;
        private const int B_SCALER = 10;
        
        private MainWindow mWindow;

        //y = mx+b stuff
        public List<LineCurve> mLines;
        private LineCurve mCurrentSelectedLine;
        private int mNextLineIndex;
        private Microsoft.Research.Kinect.Nui.Vector mOrgM;
        private Microsoft.Research.Kinect.Nui.Vector mOrgB;

        private float mNewMHolder;
        private float mNewBHolder;


        public LineCurve CurrentSelectedLine
        {
            get { return mCurrentSelectedLine; }
            set { mCurrentSelectedLine = value; }
        }
        
        // ---------------------------------------------------------------------------------------------
        // ------------------------------  Line Graphing  ----------------------------------------------
        // ---------------------------------------------------------------------------------------------

        public LineGraphing()
        {
            mWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            mLines = new List<LineCurve>();
            mNextLineIndex = 1;
        }
        
      
        public void GraphLine()
        {
            
        }

        public string GraphDefualtLine(int incNumTicksPerSec, int Xmin, int Xmax)
        {
            incNumTicksPerSec /= 2;
            int totalXWidth = Xmax - Xmin;
            LineCurve mLine = new LineCurve("Line " + mNextLineIndex, mWindow.mMainModel.GenerateRandomColor(), mWindow.mMainModel.mGraphPlot);
            mLine.mNumTicksPerSec = incNumTicksPerSec;
            mNextLineIndex++;
            mLine.m = 1;
            mLine.b = 0;
            for (int i = 0; i < incNumTicksPerSec * totalXWidth; i++)
            {
                float ticks = i * (1.0f / (float)incNumTicksPerSec) + Xmin;
                System.Windows.Point tempPoint = new System.Windows.Point(ticks, mLine.m * ticks + mLine.b);
                mLine.AddPoint(tempPoint, mWindow.mMainModel.mGraphPlot);
            }
            mLines.Add(mLine);
            return "Line " + (mNextLineIndex-1);
            
        }

        public void EditLine(SkeletonData incSkeletonData, int incNumTicksPerSec, int Xmin, int Xmax)
        {
            incNumTicksPerSec /= 2;
            int totalXWidth = Xmax - Xmin;
            float LeftY = incSkeletonData.Joints[JointID.HandLeft].Position.Y;
            float RightY = incSkeletonData.Joints[JointID.HandRight].Position.Y;

            float DeltaB = 0;
            float DeltaM = 0;

            for (int i = 0; i < mCurrentSelectedLine.Points.Count; i++)
                {
                    DeltaB = Truncate((LeftY - mOrgB.Y) * B_SCALER * totalXWidth / 10);
                    DeltaM = Truncate((RightY - mOrgM.Y) * M_SCALER);
                    double x;
                    double y;
                    x = i * (1.0f / (float)incNumTicksPerSec) + Xmin;
                    y = (mCurrentSelectedLine.m + DeltaM) * x + (mCurrentSelectedLine.b + DeltaB);
                    mCurrentSelectedLine.SetY(mCurrentSelectedLine.Title, i, y, incNumTicksPerSec);
                    //mWindow.TableValues.Items.Add(Truncate(x) + "\t\t" + Truncate(y));

                }
            mNewMHolder = Truncate(mCurrentSelectedLine.m + DeltaM);
            mNewBHolder = Truncate(mCurrentSelectedLine.b + DeltaB);

            UpdateEquation(mNewMHolder, mNewBHolder);
        }

        public void EditLineNatural(SkeletonData incSkeletonData, int incNumTicksPerSec, int Xmin, int Xmax)
        {
            incNumTicksPerSec /= 2;
            int totalXWidth = Xmax - Xmin;
            float LeftY = incSkeletonData.Joints[JointID.HandLeft].Position.Y;
            float RightY = incSkeletonData.Joints[JointID.HandRight].Position.Y;
            float LeftX = incSkeletonData.Joints[JointID.HandLeft].Position.X;
            float RightX = incSkeletonData.Joints[JointID.HandRight].Position.X;

            float DeltaB = Truncate((((LeftY - mOrgB.Y) + (RightY - mOrgM.Y)) / 2) * B_SCALER * totalXWidth/10);
            float DeltaM = Truncate(((LeftY) - (RightY )) / ((LeftX ) - (RightX )));
            double x;
            double y;
                
            for (int i = 0; i < mCurrentSelectedLine.Points.Count; i++)
            {
                x = i * (1.0f / (float)incNumTicksPerSec) + Xmin;
                y = (DeltaM) * x + (DeltaB);
                mCurrentSelectedLine.SetY(mCurrentSelectedLine.Title, i, y, incNumTicksPerSec);
                //mWindow.TableValues.Items.Add(Truncate(x) + "\t\t" + Truncate(y));

            }
            mNewMHolder = DeltaM;
            mNewBHolder = DeltaB;

            UpdateEquation(mNewMHolder, mNewBHolder);
        }

        public void SetSelectedLineOrgins(SkeletonData incSkeletonData)
        {
            mOrgM.Y = incSkeletonData.Joints[JointID.HandRight].Position.Y;
            mOrgB.Y = incSkeletonData.Joints[JointID.HandLeft].Position.Y;
            mOrgM.X = incSkeletonData.Joints[JointID.HandRight].Position.X;
            mOrgB.X = incSkeletonData.Joints[JointID.HandLeft].Position.X;
        }

        public void SetLineMB()
        {
            mCurrentSelectedLine.m = mNewMHolder;
            mCurrentSelectedLine.b = mNewBHolder;
        }

        public void UpdateEquation(float m, float b)
        {
            string formatM;
            string formatB;
            if (mWindow.mMainModel.IntergerMode == true)
            {
                string fractionM = Utility.Convert((decimal)m);
                formatM = fractionM;
                formatB = Utility.Convert((decimal)b);
            }
            else
            {
                formatM = String.Format("{0:0.00}", m);
                formatB = String.Format("{0:0.00}", b);
            }
            string str = "y = " + formatM + "x + " + formatB;
            mWindow.theGraphPlot.EquationText.Text = str;
        }

        private float Truncate(double incNum)
        {

            return (float)Math.Truncate(incNum * 100) / 100;
        }

    }
    
}
