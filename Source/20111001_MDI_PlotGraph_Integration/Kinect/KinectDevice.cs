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
        #region Singleton instance of the KinectDevice
        // Singleton
        static readonly KinectDevice instance = new KinectDevice();
        
        static KinectDevice() { }
        
        public static KinectDevice Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion
        //The MainWindow
        MainWindow mWindow;
        
        // Kinect Natrual User Interface
        private Runtime mKinectNUI;

        KinectDevice()
        {
            mKinectNUI = Runtime.Kinects[0];
            
            //Initialize the device with color and depth
            mKinectNUI.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseColor | RuntimeOptions.UseSkeletalTracking);

            mKinectNUI.SkeletonEngine.TransformSmooth = true;

            // reduces jitter
            var parameters = new TransformSmoothParameters
            {
                Smoothing = 0.5f,
                Correction = 0.0f,
                Prediction = 0.0f,
                JitterRadius = 0.1f,
                MaxDeviationRadius = 0.08f
            };
            mKinectNUI.SkeletonEngine.SmoothParameters = parameters;

            //EventHandlers
            mKinectNUI.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(mKinectNUI_VideoFrameReady);
            mKinectNUI.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(mKinectNUI_DepthFrameReady);
            mKinectNUI.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(mKinectNUI_SkeletonFrameReady);


            //Open streams for use
            mKinectNUI.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
            mKinectNUI.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.DepthAndPlayerIndex);
            mWindow = (MainWindow)Application.Current.MainWindow;
            mSkeletonDisplay = new SkeletonDisplay(mWindow.KinectCameraPanelInMainWindow.SkeletonWindow);

            //Used to prioritize a user (-1 = null)
            mCurrentSkeletonID = -1;

            //defualt video size
            mVideoX = 320;
            mVideoY = 240;
        }

        public void AddTilt(int num)
        {
            try
            {
                mKinectNUI.NuiCamera.ElevationAngle = mKinectNUI.NuiCamera.ElevationAngle + num;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (ArgumentOutOfRangeException outOfRangeException)
            {
                //Elevation angle must be between Elevation Minimum/Maximum"

                MessageBox.Show(outOfRangeException.Message);
            }
        }

        
    }

}
