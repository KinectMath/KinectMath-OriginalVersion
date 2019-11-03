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

using AvalonDock;
using MDI_PlotGraph_Integration.Kinect;

namespace MDI_PlotGraph_Integration.Panels
{
    /// <summary>
    /// Interaction logic for KinectCamera.xaml
    /// </summary>
    public partial class KinectCameraPanelContent : DockableContent
    {
        private KinectDevice mKinect;

        public KinectCameraPanelContent()
        {
            InitializeComponent();
        }

        private void KinectCamera_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (mKinect != null)
            {
                mKinect.mVideoX = e.NewSize.Width;
                mKinect.mVideoY = e.NewSize.Height;
            }

            //this.Width = e.NewSize.Width;
            //this.Height = e.NewSize.Height;
        }

        public void Load_KinectCamera()
        {
            mKinect = KinectDevice.Instance;
            this.FloatingWindowSize = new Size(640, 480);
            //this.ShowAsFloatingWindow(false);
        }
    }
}
