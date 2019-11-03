using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input.Manipulations;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Research.Kinect.Nui;
using Microsoft.Research.Kinect.Audio;
using Coding4Fun.Kinect.Wpf;

namespace MDI_PlotGraph_Integration.Kinect
{
    public class KinectCursorControl
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern void mouse_event(UInt32 dwFlags, UInt32 dx, UInt32 dy, UInt32 dwData, IntPtr dwExtraInfo);
        const UInt32 MOUSEEVENTF_LEFTDOWN = 0x2;    // Left Click Down
        const UInt32 MOUSEEVENTF_LEFTUP = 0x4;      // Left Click Up
        private int mSceenSizeX;
        private int mSceenSizeY;
        private bool mControlEnabled;
        private int mEnableTrueTimes;
        private int mDisableTrueTimes;
        private enum CursorState
        {
            ClickLeftDown,
            Free,
        }

        private CursorState mCurrentCursorState;

        public KinectCursorControl() 
        {
            mSceenSizeX = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
            mSceenSizeY = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
            mControlEnabled = false;
            mCurrentCursorState = CursorState.Free;
        }

        public void UpdateKinectCursor(SkeletonData incSkele)
        {


            if (EnableCursorControlGesture(incSkele) == true)
            {
                mEnableTrueTimes++;
                if (mEnableTrueTimes == 90)
                {
                    mControlEnabled = true;
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Hand;
                }
            }
            else
                mEnableTrueTimes = 0;

            if (DisableCursorControlGesture(incSkele) == true)
            {
                mDisableTrueTimes++;
                if (mDisableTrueTimes == 90)
                {
                    mControlEnabled = false;
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                }
            }
            else
                mDisableTrueTimes = 0;
                
            if (mControlEnabled == true)
            {
                Joint rightHand = incSkele.Joints[JointID.HandRight].ScaleTo(mSceenSizeX, mSceenSizeY, .4f, .5f);
                Joint head = incSkele.Joints[JointID.Head];

                int cursorX = (int)(rightHand.Position.X);
                int cursorY = (int)(rightHand.Position.Y);

                if (incSkele.TrackingState == SkeletonTrackingState.Tracked)
                {
                    //Move mouse
                    System.Windows.Forms.Cursor.Position = new System.Drawing.Point(cursorX, cursorY);
                    LeftClickGesture(incSkele);
                }
            }
        }

        private bool EnableCursorControlGesture(SkeletonData incSkele)
        {
            if (incSkele == null)
                return false;
            if (incSkele.Joints[JointID.HandLeft].Position.Y > incSkele.Joints[JointID.Head].Position.Y && incSkele.Joints[JointID.HandRight].Position.Y > incSkele.Joints[JointID.Head].Position.Y)
                return true;
            else
                return false;
        }

        private bool DisableCursorControlGesture(SkeletonData incSkele)
        {
            if (incSkele == null)
                return false;
            if (incSkele.Joints[JointID.HandLeft].Position.Y > incSkele.Joints[JointID.Head].Position.Y && incSkele.Joints[JointID.HandRight].Position.Z + .5f < incSkele.Joints[JointID.Head].Position.Z)
                return true;
            else
                return false;
        }

        public void LeftClickGesture(SkeletonData incSkele)
        {
            if (CursorState.Free == mCurrentCursorState && incSkele.Joints[JointID.HandLeft].Position.Y > incSkele.Joints[JointID.ShoulderCenter].Position.Y)
            {

                mCurrentCursorState = CursorState.ClickLeftDown;
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero);
            }
            else if (CursorState.ClickLeftDown == mCurrentCursorState && incSkele.Joints[JointID.HandLeft].Position.Y < incSkele.Joints[JointID.Head].Position.Y-.5f)
            {
                mCurrentCursorState = CursorState.Free;
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, IntPtr.Zero);
            }
            
        }
    }
}
