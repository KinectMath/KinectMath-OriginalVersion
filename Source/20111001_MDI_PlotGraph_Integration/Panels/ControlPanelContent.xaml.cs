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

namespace MDI_PlotGraph_Integration.Panels
{
    /// <summary>
    /// Interaction logic for ControlPanel.xaml
    /// </summary>
    public partial class ControlPanelContent : DockableContent
    {
        public ControlPanelContent()
        {
            InitializeComponent();
            this.AllowDrop = false;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow m = (MainWindow)Application.Current.MainWindow;
            m.mMainModel.Start();


        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow m = (MainWindow)Application.Current.MainWindow;
            m.mMainModel.Clear();
        }

        private void AddLine_Click(object sender, RoutedEventArgs e)
        {
            MainWindow m = (MainWindow)Application.Current.MainWindow;
            m.mMainModel.AddLine();
        }
        
        private void AddParabola_Click(object sender, RoutedEventArgs e)
        {
            MainWindow m = (MainWindow)Application.Current.MainWindow;
            m.mMainModel.AddParabola();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow m = (MainWindow)Application.Current.MainWindow;
            m.mMainModel.Stop();
        }

        private void AddSine_Click(object sender, RoutedEventArgs e)
        {
            MainWindow m = (MainWindow)Application.Current.MainWindow;
            m.mMainModel.AddSine();
        }

        
    }
}
