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
    public class ParabolaGraphing
    {
        public enum EquationForm
        {
            StandardForm,
            VertexForm,
            FactoredForm,
            FactoredFormNonCalcuated
        }
        private EquationForm mEqForm = EquationForm.VertexForm;
        public EquationForm EquationFormat
        {
            get { return mEqForm; }
            set { mEqForm = value; UpdateEquation(); }
        }

        private float mCureentA, mCureentH, mCureentK;

        //Scalers
        private const int A_SCALER = 5;
        private const int H_SCALER = 8;
        private const int K_SCALER = 8;
        private MainWindow mWindow;

        //y = a(x-h)^2+k stuff
        public List<ParabolaCurve> mParabolas;
        private ParabolaCurve mCurrentSelectedParabola;

        private int mNextParabolaIndex = 0;
        private float Aorg;
        private float Horg;
        private float Korg;

        private float mNewAHolder;
        private float mNewHHolder;
        private float mNewKHolder;

        public ParabolaCurve CurrentSelectedParabola
        {
            get { return mCurrentSelectedParabola; }
            set { mCurrentSelectedParabola = value; }
        }

        // ---------------------------------------------------------------------------------------------
        // ------------------------------  Parabola Graphing  ------------------------------------------
        // ---------------------------------------------------------------------------------------------

        public ParabolaGraphing()
        {
            mWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            mParabolas = new List<ParabolaCurve>();
            mNextParabolaIndex = 1;
        }

        private void GraphParabola()
        {


        }

        public string GraphDefualtParabola(int incNumTicksPerSec, int Xmin, int Xmax)
        {
            incNumTicksPerSec /= 2;
            int totalXWidth = Xmax - Xmin;
            ParabolaCurve mParabola = new ParabolaCurve("Parabola " + mNextParabolaIndex, mWindow.mMainModel.GenerateRandomColor(), mWindow.mMainModel.mGraphPlot);
            mParabola.mNumTicksPerSec = incNumTicksPerSec;
            mNextParabolaIndex++;
            mParabola.a = 1;
            mParabola.h = 1;
            mParabola.k = 1;
            for (int i = 0; i < incNumTicksPerSec * totalXWidth; i++)
            {
                float ticks = i * (1.0f / (float)incNumTicksPerSec) + Xmin;
                System.Windows.Point tempPoint = new System.Windows.Point(ticks, mParabola.a * (Math.Pow(ticks - mParabola.h, 2)) + mParabola.k);
                mParabola.AddPoint(tempPoint, mWindow.mMainModel.mGraphPlot);
            }
            mParabolas.Add(mParabola);

            return "Parabola " + (mNextParabolaIndex - 1);

        }


        public void EditParabola(SkeletonData incSkeletonData, int incNumTicksPerSec, int Xmin, int Xmax)
        {
            incNumTicksPerSec /= 2;
            int totalXWidth = Xmax - Xmin;
            float LeftX = incSkeletonData.Joints[JointID.HandLeft].Position.X;
            float LeftY = incSkeletonData.Joints[JointID.HandLeft].Position.Y;
            float AXWidth = incSkeletonData.Joints[JointID.HandRight].Position.X;

            double a = 0;
            double h = 0;
            double k = 0;

            for (int i = 0; i < mCurrentSelectedParabola.Points.Count; i++)
            {
                float deltaA = Truncate((AXWidth - Aorg) * A_SCALER);
                float deltaH = Truncate((LeftX - Horg) * H_SCALER * totalXWidth / 10);
                float deltaK = Truncate((LeftY - Korg) * K_SCALER * totalXWidth / 10);

                double x;
                double y;

                x = i * (1.0f / (float)incNumTicksPerSec) + Xmin;
                a = (mCurrentSelectedParabola.a - deltaA);
                h = (mCurrentSelectedParabola.h + deltaH);
                k = (mCurrentSelectedParabola.k + deltaK);

                y = a * (Math.Pow(x - h, 2)) + k;
                mCurrentSelectedParabola.SetY(mCurrentSelectedParabola.Title, i, y, incNumTicksPerSec);
            }

            mNewAHolder = Truncate(a);
            mNewHHolder = Truncate(h);
            mNewKHolder = Truncate(k);

            UpdateEquation(mNewAHolder, mNewHHolder, mNewKHolder);
        }


        public void SetSelectedParabolaOrgins(SkeletonData incSkeletonData)
        {
            Aorg = incSkeletonData.Joints[JointID.HandRight].Position.X;
            Horg = incSkeletonData.Joints[JointID.HandLeft].Position.X;
            Korg = incSkeletonData.Joints[JointID.HandLeft].Position.Y;
        }

        public void SetParabolaAHK()
        {
            mCurrentSelectedParabola.a = mNewAHolder;
            mCurrentSelectedParabola.h = mNewHHolder;
            mCurrentSelectedParabola.k = mNewKHolder;
        }

        public void UpdateEquation()
        {
            switch (EquationFormat)
            {
                case EquationForm.VertexForm:
                    mWindow.theGraphPlot.EquationText.Text = VetexForm(mCureentA, mCureentH, mCureentK);
                    break;
                case EquationForm.StandardForm:
                    mWindow.theGraphPlot.EquationText.Text = convertVertexToStandardForm(mCureentA, mCureentH, mCureentK);
                    break;
                case EquationForm.FactoredForm:
                    mWindow.theGraphPlot.EquationText.Text = convertVertexToFacotrForm(mCureentA, mCureentH, mCureentK);
                    break;
                case EquationForm.FactoredFormNonCalcuated:
                    mWindow.theGraphPlot.EquationText.Text = convertVertexToFacotrFormNonCalculated(mCureentA, mCureentH, mCureentK);
                    break;
            }
        }

        public void UpdateEquation(float a, float h, float k)
        {
            mCureentA = a;
            mCureentH = h;
            mCureentK = k;
            switch (EquationFormat)
            {
                case EquationForm.VertexForm:
                    mWindow.theGraphPlot.EquationText.Text = VetexForm(a, h, k);
                    break;
                case EquationForm.StandardForm:
                    mWindow.theGraphPlot.EquationText.Text = convertVertexToStandardForm(a, h, k);
                    break;
                case EquationForm.FactoredForm:
                    mWindow.theGraphPlot.EquationText.Text = convertVertexToFacotrForm(a, h, k);
                    break;               
                case EquationForm.FactoredFormNonCalcuated:
                    mWindow.theGraphPlot.EquationText.Text = convertVertexToFacotrFormNonCalculated(a, h, k);
                    break;
            }
        }

        private float Truncate(double incNum)
        {
            if (mWindow.mMainModel.IntergerMode == true)
            {
                return (float)Math.Round(Math.Truncate(incNum * 100) / 100);
            }
            return (float)Math.Truncate(incNum * 100) / 100;
        }

        private string convertVertexToStandardForm(float a, float h, float k)
        {
            // y = ax^2 - 2ahx + ah^2 + k
            float mA, mB, mC;

            mA = a;
            mB = 2 * a * h;
            mC = a * (float)Math.Pow(h, 2) + k;

            string formatA, formatB, formatC;
            if (mWindow.mMainModel.IntergerMode == true)
            {
                formatA = String.Format("{0:0}", mA);
                formatB = String.Format("{0:0}", mB);
                formatC = String.Format("{0:0}", mC);
            }
            else
            {
                formatA = String.Format("{0:0.00}", mA);
                formatB = String.Format("{0:0.00}", mB);
                formatC = String.Format("{0:0.00}", mC);
            }

            string str = "y = (" + formatA + ")x\x00B2 + (" + formatB + ")x + (" + formatC + ")";
            return str;
        }

        private string convertVertexToFacotrForm(float a, float h, float k)
        {
            // y = a(x + h)^2 + k
            // y/a = (x + h)^2 + k/a
            // y/a = x^2 + 2hx + (h^2 + k/a)
            // Let x^2 + 2hx + (h^2 + k/a) = 0
            // x^2 + 2hx + (h)^2 = -(xh)^2 - (h^2 + k/a)
            // (x + h)^2 = -(h)^2 - (h^2 + k/a)
            // x1 = -h + root(-(h)^2 - (h^2 + k/a))
            // x2 = -h - root(-(h)^2 - (h^2 + k/a))
            // y = a ( x - x1) ( x - x2)

            float mA, mX1, mX2;

            mA = a;
            mX1 = (float) (-h + Math.Sqrt(-1 * Math.Pow(h, 2) - (Math.Pow(h, 2) + k / a)));
            mX2 = (float) (-h - Math.Sqrt(-1 * Math.Pow(h, 2) - (Math.Pow(h, 2) + k / a)));

            string formatA, formatB, formatC;
            if (mWindow.mMainModel.IntergerMode == true)
            {
                formatA = String.Format("{0:0}", mA);
                formatB = String.Format("{0:0}", mX1);
                formatC = String.Format("{0:0}", mX2);
            }
            else
            {
                formatA = String.Format("{0:0.00}", mA);
                formatB = String.Format("{0:0.00}", mX1);
                formatC = String.Format("{0:0.00}", mX2);
            }

            string str = "y = " + formatA + " ( x - " + formatB + ") + ( x - " + formatC + ")";
            return str;
        }

        private string convertVertexToFacotrFormNonCalculated(float a, float h, float k)
        {
            // y = a(x + h)^2 + k
            // y/a = (x + h)^2 + k/a
            // y/a = x^2 + 2hx + (h^2 + k/a)
            // Let x^2 + 2hx + (h^2 + k/a) = 0
            // x^2 + 2hx + (h)^2 = -(xh)^2 - (h^2 + k/a)
            // (x + h)^2 = -(h)^2 - (h^2 + k/a)
            // x1 = -h + root(-(h)^2 - (h^2 + k/a))
            // x2 = -h - root(-(h)^2 - (h^2 + k/a))
            // y = a ( x - x1) ( x - x2)

            float mA, mH, mX1, mX2;

            mA = a;
            mH = -1 * h;
            mX1 = (float)( -1 * Math.Pow(h, 2) - (Math.Pow(h, 2) + k / a));
            mX2 = (float)( -1 * Math.Pow(h, 2) - (Math.Pow(h, 2) + k / a));

            string formatA, formatB, formatC, formatH;
            if (mWindow.mMainModel.IntergerMode == true)
            {
                formatA = String.Format("{0:0}", mA);
                formatB = String.Format("{0:0}", mX1);
                formatC = String.Format("{0:0}", mX2);
                formatH = String.Format("{0:0}", mH);
            }
            else
            {
                formatA = String.Format("{0:0.00}", mA);
                formatB = String.Format("{0:0.00}", mX1);
                formatC = String.Format("{0:0.00}", mX2);
                formatH = String.Format("{0:0.00}", mH);
            }

            string str = "y = " + formatA + " ( x - (" + formatH + " + √" + formatB + ") ) + ( x - (" + formatH + " - √" + formatC + ") )";
            return str;
        }

        private string VetexForm(float a, float h, float k)
        {
            string formatA, formatH, formatK;
            if (mWindow.mMainModel.IntergerMode == true)
            {
                formatA = String.Format("{0:0}", a);
                formatH = String.Format("{0:0}", h);
                formatK = String.Format("{0:0}", k);
            }
            else
            {
                formatA = String.Format("{0:0.00}", a);
                formatH = String.Format("{0:0.00}", h);
                formatK = String.Format("{0:0.00}", k);
            }

            string str = "y = " + formatA + "( x + " + formatH + " )\x00B2 + " + formatK;
            return str;
        }

    }
}
