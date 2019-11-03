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
using System.IO;

using Microsoft.Research.Kinect.Nui;
using Microsoft.Research.Kinect.Audio;

namespace MDI_PlotGraph_Integration.Kinect
{
    //Currently NOT being used
    public partial class KinectDevice
    {        
        // Depth Camera EventHandler
        void mKinectNUI_DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
        }

    }
}
