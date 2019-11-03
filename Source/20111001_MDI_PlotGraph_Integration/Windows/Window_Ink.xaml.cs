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
using System.Windows.Shapes;

namespace MDI_PlotGraph_Integration.Windows
{
    public partial class Window_Ink : Window
    {
        bool isResizing = false;
        Point startPt;

        public Window_Ink()
        {
            InitializeComponent();
            this.grip.MouseDown += new MouseButtonEventHandler(grip_MouseDown);
            this.grip.MouseUp += new MouseButtonEventHandler(grip_MouseUp);
        }

        private void DragMove(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Exit(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        private void Reset(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DrawArea.Strokes.Clear();
        }

        private void grip_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Mouse.Capture(this.grip);
                startPt = e.GetPosition(this.grip);
                isResizing = true;
            }
        }
        private void grip_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isResizing)
            {
                Point newPt = e.GetPosition(this.grip);
                this.Width += newPt.X - startPt.X;
                this.Height += newPt.Y - startPt.Y;
                isResizing = false;
                this.grip.ReleaseMouseCapture();
            }
        }
    }
}
