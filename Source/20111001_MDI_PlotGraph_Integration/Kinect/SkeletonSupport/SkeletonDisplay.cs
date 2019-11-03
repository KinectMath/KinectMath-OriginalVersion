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

using Microsoft.Research.Kinect.Nui;
using Microsoft.Research.Kinect.Audio;

namespace MDI_PlotGraph_Integration.Kinect
{
    public class SkeletonDisplay
    {
        private Canvas mCanvas;
        public SkeletonDisplayJoint Head;
        public SkeletonDisplayJoint LeftHand;
        public SkeletonDisplayJoint RightHand;
        public SkeletonDisplayJoint ElbowLeft;
        public SkeletonDisplayJoint ElbowRight;
        public SkeletonDisplayJoint ShoulderLeft;
        public SkeletonDisplayJoint ShoulderRight;
        public SkeletonDisplayJoint ShoulderCenter;
        public SkeletonDisplayJoint Spine;

        public SkeletonDisplay(Canvas c)
        {
            mCanvas = c;
            Head = new SkeletonDisplayJoint(new Point(0, 0), 10.0f, System.Windows.Media.Brushes.Lime);
            LeftHand = new SkeletonDisplayJoint(new Point(0, 0), 8.0f, System.Windows.Media.Brushes.Gray);
            RightHand = new SkeletonDisplayJoint(new Point(0, 0), 8.0f, System.Windows.Media.Brushes.Gray);
            ElbowLeft = new SkeletonDisplayJoint(new Point(0, 0), 5.0f, System.Windows.Media.Brushes.Gray);
            ElbowRight = new SkeletonDisplayJoint(new Point(0, 0), 5.0f, System.Windows.Media.Brushes.Gray);
            ShoulderLeft = new SkeletonDisplayJoint(new Point(0, 0), 5.0f, System.Windows.Media.Brushes.Gray);
            ShoulderRight = new SkeletonDisplayJoint(new Point(0, 0), 5.0f, System.Windows.Media.Brushes.Gray);
            Spine = new SkeletonDisplayJoint(new Point(0, 0), 5.0f, System.Windows.Media.Brushes.Gray);
            ShoulderCenter = new SkeletonDisplayJoint(new Point(0, 0), 4.0f, System.Windows.Media.Brushes.Gray);

            mCanvas.Children.Add(Head.GetEllipse());
            mCanvas.Children.Add(LeftHand.GetEllipse());
            mCanvas.Children.Add(RightHand.GetEllipse());
            mCanvas.Children.Add(ElbowLeft.GetEllipse());
            mCanvas.Children.Add(ElbowRight.GetEllipse());
            mCanvas.Children.Add(ShoulderLeft.GetEllipse());
            mCanvas.Children.Add(ShoulderRight.GetEllipse());
            mCanvas.Children.Add(Spine.GetEllipse());
            mCanvas.Children.Add(ShoulderCenter.GetEllipse());
        }



    }
}
