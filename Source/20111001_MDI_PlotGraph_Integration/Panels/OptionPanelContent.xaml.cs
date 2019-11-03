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
    /// Interaction logic for ControlPanel.xaml
    /// </summary>
    public partial class OptionPanelContent : DockableContent
    {
        MainWindow m;

        public OptionPanelContent()
        {
            m = (MainWindow)Application.Current.MainWindow;

            InitializeComponent();

            this.AllowDrop = false;

            this.DisplayDistance.IsChecked = m.mMainModel.DistanceVisable;
            this.DisplayVelocity.IsChecked = m.mMainModel.VelocityVisable;
            this.DisplayAcceleration.IsChecked = m.mMainModel.AccelerationVisable;
            this.IntegerMode.IsChecked = m.mMainModel.IntergerMode;
            this.NaturalLineEditing.IsChecked = m.mMainModel.NaturalLineEditing;
            
            if (m.mMainModel.speechRecognizer != null)
            {
                VocieAccuracySlider.Value = m.mMainModel.speechRecognizer.confiLevel;
                VoiceAccuracyText.Text = String.Format("{0:0.00}", m.mMainModel.speechRecognizer.confiLevel);
            }
        }

        private void Display_Click(object sender, RoutedEventArgs e)
        {
            CheckBox temp = (CheckBox)sender;
            switch (temp.IsChecked)
            {
                case true:
                    m.mMainModel.mGraphPlot.SetLineAndMarkerVisiable(temp.Content.ToString(), true);
                    break;
                case false:
                    m.mMainModel.mGraphPlot.SetLineAndMarkerVisiable(temp.Content.ToString(), false);
                    break;
                default:
                    break;
            }
            if (temp.IsChecked == true || temp.IsChecked == false)
            {
                switch (temp.Content.ToString())
                {
                    case "Distance":
                        m.mMainModel.DistanceVisable = !m.mMainModel.DistanceVisable;
                        break;
                    case "Velocity":
                        m.mMainModel.VelocityVisable = !m.mMainModel.VelocityVisable;
                        break;
                    case "Acceleration":
                        m.mMainModel.AccelerationVisable = !m.mMainModel.AccelerationVisable;
                        break;
                }
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            string joint = ((RadioButton)sender).Content.ToString();

            if (MDI_PlotGraph_Integration.Kinect.KinectDevice.Instance != null)
                MDI_PlotGraph_Integration.Kinect.KinectDevice.Instance.ChangeTrackedJointColor(joint);

            if (m.mMainModel != null)
                m.mMainModel.SetJoint(joint);
        }

        private void RadioButtonHead_Click(object sender, RoutedEventArgs e)
        {
            this.RadioButtonHead.IsChecked = true;
            if (MDI_PlotGraph_Integration.Kinect.KinectDevice.Instance != null)
                MDI_PlotGraph_Integration.Kinect.KinectDevice.Instance.ChangeTrackedJointColor("Head");

            if (m.mMainModel != null)
                m.mMainModel.SetJoint("Head");
        }

        private void RadioButtonRightHand_Click(object sender, RoutedEventArgs e)
        {
            this.RadioButtonRightHand.IsChecked = true;
            if (MDI_PlotGraph_Integration.Kinect.KinectDevice.Instance != null)
                MDI_PlotGraph_Integration.Kinect.KinectDevice.Instance.ChangeTrackedJointColor("RightHand");

            if (m.mMainModel != null)
                m.mMainModel.SetJoint("RightHand");
        }

        private void RadioButtonLeftHand_Click(object sender, RoutedEventArgs e)
        {
            this.RadioButtonLeftHand.IsChecked = true;
            if (MDI_PlotGraph_Integration.Kinect.KinectDevice.Instance != null)
                MDI_PlotGraph_Integration.Kinect.KinectDevice.Instance.ChangeTrackedJointColor("LeftHand");

            if (m.mMainModel != null)
                m.mMainModel.SetJoint("LeftHand");
        }

        private void RandomLineSmooth_Click(object sender, RoutedEventArgs e)
        {
            m.mMainModel.InterpolateRandomLine();

        }

        private void AddTilt_Click(object sender, RoutedEventArgs e)
        {
            KinectDevice.Instance.AddTilt(5);
        }

        private void MinusTilt_Click(object sender, RoutedEventArgs e)
        {
            KinectDevice.Instance.AddTilt(-5);
        }

        private void VocieAccuracySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VoiceAccuracyText.Text = String.Format("{0:0.00}", e.NewValue);
            if (m.mMainModel.speechRecognizer != null)
            {
                m.mMainModel.speechRecognizer.confiLevel = Convert.ToSingle(VoiceAccuracyText.Text);
            }
        }

        private void IntegerMode_Click(object sender, RoutedEventArgs e)
        {
            m.mMainModel.IntergerMode = !m.mMainModel.IntergerMode;
        }

        private void NaturalLineEditing_Click(object sender, RoutedEventArgs e)
        {
            m.mMainModel.NaturalLineEditing = !m.mMainModel.NaturalLineEditing;
        }
    }
}
