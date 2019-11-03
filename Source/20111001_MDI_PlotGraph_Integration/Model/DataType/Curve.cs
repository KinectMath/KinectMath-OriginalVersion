using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxyPlot;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace MDI_PlotGraph_Integration.Model.DataType
{
    public class Curve: LineSeries
    {
        protected MainWindow mWindow;
        public int mNumTicksPerSec;
        public CurveType mCurveType;
        public enum CurveType
        {
            None = 0,
            Line = 1,
            Parabola = 2,
            Sine = 3
        }

        public Curve(){}

        public Curve(String inTitle, OxyColor inColor, GraphPlotModel incPlot)
        {
            mWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            this.Color = inColor;
            this.MarkerFill = inColor;
            this.MarkerType = MarkerType.Circle;
            this.MarkerStrokeThickness = 2;
            this.MarkerSize = 4;
     
            incPlot.AddCurve(this);
            this.Title = inTitle;
            mCurveType = CurveType.None;
            mWindow.FunctionListBox.Items.Add(inTitle);
        }

        public void AddPoint(Point incPoint, GraphPlotModel incPlot)
        {
            this.Points.Add(new DataPoint(incPoint.X, incPoint.Y));
            //incPlot.AddPoint(this.Title, new DataPoint(incPoint.X, incPoint.Y));
        }

        public void SetY(String incTitle, int xCount, double valY, int incNumTicksPerSec)
        {
            if (this != null)
            {
                DataPoint pt = new DataPoint(this.Points[xCount].X, valY);
                this.Points[xCount] = pt;
            }
        }

        public void Remove(GraphPlotModel incPlot)
        {
            incPlot.PlotModel.Series.Remove(this);
        }

    }
}
