using System;
using System.ComponentModel;
using System.Diagnostics;
using OxyPlot;
using MDI_PlotGraph_Integration.Model.DataType;

namespace MDI_PlotGraph_Integration.Model
{
    public class GraphPlotModel : INotifyPropertyChanged
    {
        MainWindow mWindow;
        public static readonly byte C_MARKERFILL = 0x70;
        OxyPlot.LinearAxis horizontal;
        OxyPlot.LinearAxis vertical;
        public int mCurrentXMin;
        public int mCurrentYMin;
        public int mCurrentXMax;
        public int mCurrentYMax;

        public GraphPlotModel()
        {
            mWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            SetupModel();
        }

        void SetupModel()
        {
            PlotModel = new PlotModel();
            PlotModel.PlotMargins = new OxyThickness(10);
            PlotModel.AutoAdjustPlotMargins = true;
            
            vertical = new OxyPlot.LinearAxis();
            vertical.Minimum = -5;
            vertical.Maximum = 5;
            vertical.Position = AxisPosition.Left;
            vertical.TickStyle = TickStyle.Crossing;
            vertical.PositionAtZeroCrossing = true;

            horizontal = new OxyPlot.LinearAxis();
            horizontal.Minimum = 0;
            horizontal.Maximum = 10;
            horizontal.Position = AxisPosition.Bottom;
            horizontal.TickStyle = TickStyle.Crossing;
            horizontal.PositionAtZeroCrossing = true;

            PlotModel.Axes.Add(vertical);
            PlotModel.Axes.Add(horizontal);

        }

        public void RemovePoint()
        { 
        }

        public void AddCurve(Curve inCurce)
        {
                     
            PlotModel.Series.Add(inCurce);
            RaisePropertyChanged("PlotModel");
        }

        public void AddCurve(String inTitle)
        {
            LineSeries temp = new LineSeries(inTitle);
            PlotModel.Series.Add(temp);
            RaisePropertyChanged("PlotModel");
        }

        public void ClearGraph()
        {
            PlotModel.Series.Clear();
            mWindow.theGraphPlot.Refresh(true);
        }

        public LineSeries FindLinebyTitle(String incTitle)
        {
            for (int i = 0; i < PlotModel.Series.Count; i++)
            {
                if (PlotModel.Series[i].Title == incTitle)
                    return PlotModel.Series[i] as LineSeries;
            }
            return null;
        }

        private int FindLinebyIndex(String incTitle)
        {
            for (int i = 0; i < PlotModel.Series.Count; i++)
            {
                if (PlotModel.Series[i].Title == incTitle)
                    return i;
            }
            return -1;
        }

        public void HighlightSelectedLine(String incTitle)
        {
            LineSeries temp = FindLinebyTitle(incTitle);
            if (temp != null)
            {
                //temp.MarkerStrokeThickness = 4;
                temp.MarkerSize = 7;
                temp.StrokeThickness = 5;
                
                mWindow.theGraphPlot.Refresh(true);
            }
        }

        public void DeselectLines()
        {
            for (int i = 0; i < PlotModel.Series.Count; i++)
            {
                LineSeries temp = PlotModel.Series[i] as LineSeries;
                
                //temp.MarkerStrokeThickness = 1;
                temp.MarkerSize = 4;
                temp.StrokeThickness = 2;
            }
            mWindow.theGraphPlot.Refresh(true);
        }

        public void SetLineVisiable(String incTitle, bool b)
        {
            LineSeries temp = FindLinebyTitle(incTitle);
            if (temp != null)
            {
                if(b)
                    temp.Color.A = 0xFF;
                else
                    temp.Color.A = 0x00;

                mWindow.theGraphPlot.Refresh(true);
            }
        }

        public void SetLineMarkerVisiable(String incTitle, bool b)
        {
            LineSeries temp = FindLinebyTitle(incTitle);
            if (temp != null)
            {
                if (b)
                    temp.MarkerFill.A = GraphPlotModel.C_MARKERFILL;
                else
                    temp.MarkerFill.A = 0x00;

                mWindow.theGraphPlot.Refresh(true);
            }
        }

        public void SetLineAndMarkerVisiable(String incTitle, bool b)
        {
            LineSeries temp = FindLinebyTitle(incTitle);
            if (temp != null)
            {
                if (b)
                {
                    temp.Color.A = 0xFF;
                    //temp.MarkerFill.A = GraphPlotModel.C_MARKERFILL;
                }
                else
                {
                    temp.Color.A = 0x00;
                    temp.MarkerFill.A = 0x00;
                }
                mWindow.theGraphPlot.Refresh(true);
            }
        }

        public void ChangeAxes(int Xmin, int Xmax, int Ymin, int Ymax)
        {
            mCurrentXMin = Xmin;
            mCurrentXMax = Xmax;
            mCurrentYMin = Ymin;
            mCurrentYMax = Ymax;
            vertical = new OxyPlot.LinearAxis();
            vertical.Minimum = Ymin;
            vertical.Maximum = Ymax;
            vertical.Position = AxisPosition.Left;
            vertical.TickStyle = TickStyle.Crossing;
            vertical.PositionAtZeroCrossing = true;
            //vertical.MajorGridlineStyle = OxyPlot.LineStyle.Solid;
            //vertical.MinorGridlineStyle = OxyPlot.LineStyle.Dot;

            horizontal = new OxyPlot.LinearAxis();
            horizontal.Minimum = Xmin;
            horizontal.Maximum = Xmax;
            horizontal.Position = AxisPosition.Bottom;
            horizontal.TickStyle = TickStyle.Crossing;
            horizontal.PositionAtZeroCrossing = true;
            //horizontal.MajorGridlineStyle = OxyPlot.LineStyle.Solid;
            //horizontal.MinorGridlineStyle = OxyPlot.LineStyle.Dot;
            
            PlotModel.Axes.Clear();
            PlotModel.Axes.Add(vertical);
            PlotModel.Axes.Add(horizontal);
            mWindow.theGraphPlot.Refresh(true);
        }        

        public int TotalNumberOfPoints { get; set; }

        private Func<double, double, double, double> Function { get; set; }

        public PlotModel PlotModel { get; set; }

        public void Test()
        {
            var s = new LineSeries();
            s.Points.Add(new DataPoint(1, 1));
            s.Points.Add(new DataPoint(3, 5));
            s.Points.Add(new DataPoint(7, 7));
            
            s.MarkerType = MarkerType.Circle;
            s.Color = OxyColor.FromArgb(0xFF, 154, 6, 78);
            s.MarkerStroke = s.Color;
            s.MarkerFill = OxyColor.FromAColor(0x70, s.Color);
            s.MarkerStrokeThickness = 2;
            s.MarkerSize = 4;
            s.Smooth = false;

            var s2 = new LineSeries();
            s2.Points.Add(new DataPoint(1, 1));
            s2.Points.Add(new DataPoint(3, 5));
            s2.Points.Add(new DataPoint(7, 7));

            s2.Smooth = true;

            PlotModel.Series.Add(s);
            PlotModel.Series.Add(s2);


            RaisePropertyChanged("PlotModel");
        }

        #region PropertyChanged Block

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}
