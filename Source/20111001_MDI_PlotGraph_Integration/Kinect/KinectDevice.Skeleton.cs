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
        // Users skeleton
        public SkeletonData mSkeleton;
        private int mCurrentSkeletonID;
        
        private SkeletonDisplay mSkeletonDisplay;
        
        // Skeleton EventHandler
        void mKinectNUI_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame allSkeletons = e.SkeletonFrame;

            if (allSkeletons == null)
                return;

            //get the first tracked skeleton
            //mSkeleton = (from s in allSkeletons.Skeletons
            //             where s.TrackingState == SkeletonTrackingState.Tracked
            //             select s).FirstOrDefault();
            
            //Chooses prime user and handles switching between users
            for (int i = 0; i < allSkeletons.Skeletons.Count(); i++)
            {
                if (allSkeletons.Skeletons.ElementAt(i).TrackingState == SkeletonTrackingState.Tracked 
                    && (EnableNewSkeletonGesture(allSkeletons.Skeletons.ElementAt(i)) || mCurrentSkeletonID == -1 || allSkeletons.Skeletons.ElementAt(i).TrackingID == mCurrentSkeletonID))
                {
                    mSkeleton = allSkeletons.Skeletons.ElementAt(i);
                    mCurrentSkeletonID = mSkeleton.TrackingID;
                }
            }

            if (mSkeletonDisplay == null)
                mSkeletonDisplay = new SkeletonDisplay(mWindow.KinectCameraPanelInMainWindow.SkeletonWindow);
            if (mSkeleton != null)
            {
                SetJointPositions(ref mKinectNUI, mSkeletonDisplay, mSkeleton);
                mWindow.mMainModel.Update_Skeleton();
            }
            else 
            {
                mCurrentSkeletonID = -1;
            }
        }

        //Returns true if both hands are above the users head
        private bool EnableNewSkeletonGesture(SkeletonData incSkele)
        {
            if (incSkele == null)
                return false;
            if (incSkele.Joints[JointID.HandLeft].Position.Y > incSkele.Joints[JointID.Head].Position.Y && incSkele.Joints[JointID.HandRight].Position.Y > incSkele.Joints[JointID.Head].Position.Y)
                return true;
            else
                return false;
        }

        //Maps all circles to a joint positions
        public void SetJointPositions(ref Runtime nui, SkeletonDisplay incSD, SkeletonData incSkeleton)
        {
            try
            {
                SetJointPosition(ref nui, incSD.Head.GetEllipse(), incSkeleton.Joints[JointID.Head]);
                SetJointPosition(ref nui, incSD.RightHand.GetEllipse(), incSkeleton.Joints[JointID.HandRight]);
                SetJointPosition(ref nui, incSD.LeftHand.GetEllipse(), incSkeleton.Joints[JointID.HandLeft]);
                SetJointPosition(ref nui, incSD.ElbowLeft.GetEllipse(), incSkeleton.Joints[JointID.ElbowLeft]);
                SetJointPosition(ref nui, incSD.ElbowRight.GetEllipse(), incSkeleton.Joints[JointID.ElbowRight]);
                SetJointPosition(ref nui, incSD.ShoulderLeft.GetEllipse(), incSkeleton.Joints[JointID.ShoulderLeft]);
                SetJointPosition(ref nui, incSD.ShoulderRight.GetEllipse(), incSkeleton.Joints[JointID.ShoulderRight]);
                SetJointPosition(ref nui, incSD.Spine.GetEllipse(), incSkeleton.Joints[JointID.Spine]);
                SetJointPosition(ref nui, incSD.ShoulderCenter.GetEllipse(), incSkeleton.Joints[JointID.ShoulderCenter]);
            }
            catch (NullReferenceException)
            {
            }
        }

        //Maps a circle to a joint position
        public void SetJointPosition(ref Runtime nui, FrameworkElement ellipse, Joint incJoint)
        {
            float depthX, depthY;
            nui.SkeletonEngine.SkeletonToDepthImage(incJoint.Position, out depthX, out depthY);
            depthX = Math.Max(0, Math.Min(depthX * (float)mVideoX, (float)mVideoX));  //convert to 320, 240 space
            depthY = Math.Max(0, Math.Min(depthY * (float)mVideoY, (float)mVideoY));  //convert to 320, 240 space

            int colorX, colorY;
            ImageViewArea iv = new ImageViewArea();
            // only ImageResolution.Resolution640x480 is supported at this point
            nui.NuiCamera.GetColorPixelCoordinatesFromDepthPixel(ImageResolution.Resolution640x480, iv, (int)depthX, (int)depthY, (short)0, out colorX, out colorY);

            Canvas.SetLeft(ellipse, (int)(320 * colorX / 640.0) - ellipse.Width / 2);
            Canvas.SetTop(ellipse, (int)(240 * colorY / 480) - ellipse.Width / 2);
        }

        public void ChangeTrackedJointColor(string joint)
        {
            mSkeletonDisplay.Head.GetEllipse().Fill = System.Windows.Media.Brushes.Gray;
            mSkeletonDisplay.RightHand.GetEllipse().Fill = System.Windows.Media.Brushes.Gray;
            mSkeletonDisplay.LeftHand.GetEllipse().Fill = System.Windows.Media.Brushes.Gray;
            switch (joint)
            {
                case "Head":
                    mSkeletonDisplay.Head.GetEllipse().Fill = System.Windows.Media.Brushes.Lime;
                    break;
                case "RightHand":
                    mSkeletonDisplay.RightHand.GetEllipse().Fill = System.Windows.Media.Brushes.Lime;
                    break;
                case "LeftHand":
                    mSkeletonDisplay.LeftHand.GetEllipse().Fill = System.Windows.Media.Brushes.Lime;
                    break;
            }
        }

    }
}
