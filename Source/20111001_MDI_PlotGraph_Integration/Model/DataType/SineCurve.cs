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
    public class SineCurve : Curve
    {
        public float a;
        public float w;
        public float o;
        public float d;

        public SineCurve(String inTitle, OxyColor inColor, GraphPlotModel incPlot)
        {
            mWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            this.Color = inColor;
            this.MarkerFill = inColor;
            this.MarkerType = MarkerType.Circle;
            this.MarkerStrokeThickness = 2;
            this.MarkerSize = 4;
            incPlot.AddCurve(this);
            Title = inTitle;
            mCurveType = CurveType.Sine;
            mWindow.FunctionListBox.Items.Add(inTitle);
        }
    }
}