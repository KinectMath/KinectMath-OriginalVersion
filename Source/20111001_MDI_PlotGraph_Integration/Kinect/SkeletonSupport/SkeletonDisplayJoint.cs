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
    public class SkeletonDisplayJoint
    {
        private Ellipse circle;
        public Point center;
        private double radius;

        public SkeletonDisplayJoint(Point cen, float rad, Brush color)
        {
            radius = rad;
            circle = new Ellipse();
            circle.Height = radius * 2;
            circle.Width = radius * 2;
            center = cen;

            circle.Fill = color;
            circle.StrokeThickness = 2;
            circle.Stroke = Brushes.Black;
        }

        public void Translate(Point p)
        {
            TranslateTransform tran = new TranslateTransform(p.X - radius, p.Y - radius);
            circle.RenderTransform = tran;
        }

        public Ellipse GetEllipse()
        {
            return circle;
        }



    }
}
