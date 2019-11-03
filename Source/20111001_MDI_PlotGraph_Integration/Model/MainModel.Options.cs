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
using MDI_PlotGraph_Integration.Functions;

namespace MDI_PlotGraph_Integration.Model
{
    public partial class MainModel
    {
        //General
        public bool SmoothCurve = false;
        public bool DistanceVisable = true;
        public bool VelocityVisable = true;
        public bool AccelerationVisable = false;
        public bool ShowEquation = true;
        public bool IntergerMode = false;
        public bool NaturalLineEditing = false;
        private bool mParabolaCurveSelected = false;
        public bool ParabolaCurveSelected
        {
            get { return mParabolaCurveSelected; }
        }
        public void DisplayEquation(bool show)
        {
            if (show ==true)
                mWindow.theGraphPlot.EquationText.Opacity = 1;
            else
                mWindow.theGraphPlot.EquationText.Opacity = 0;
        }
        public void SetParabolaEquationFormat(ParabolaGraphing.EquationForm ef)
        {
            mParabolaGraphing.EquationFormat = ef;
        }
    }
}
