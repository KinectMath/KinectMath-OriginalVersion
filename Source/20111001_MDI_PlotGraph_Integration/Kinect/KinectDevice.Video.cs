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
    public partial class KinectDevice
    {
        public double mVideoX, mVideoY;
        
        // Video Camera EventHandler
        void mKinectNUI_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            PlanarImage frame = e.ImageFrame.Image;
            BitmapSource temp = BitmapSource.Create(frame.Width, frame.Height, 96, 96, PixelFormats.Bgr32, null, frame.Bits, frame.Width * frame.BytesPerPixel);
            mWindow.KinectCameraPanelInMainWindow.VideoCameraWindow.Source = Resize(temp, frame.Width, frame.Height, mVideoX, mVideoY);
        }
        
        BitmapSource Resize(BitmapSource original, double oldX, double oldY, double newX, double newY)
        {
            return new TransformedBitmap(original, new ScaleTransform(newX / oldX, newY / oldY));
        }
    }
}
