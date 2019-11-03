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
    /// Interaction logic for IndicatorPanel.xaml
    /// </summary>
    public partial class IndicatorPanelContent : DockableContent
    {
        public IndicatorPanelContent()
        {
            InitializeComponent();
        }

        private void TrackingButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow m = (MainWindow)Application.Current.MainWindow;
            m.mMainModel.UpdateMode((int)MDI_PlotGraph_Integration.Model.MainModel.ModeState.TrackingMode);
            ResetButtons();
            TrackingButton.IsChecked = true;
        }

        private void EditingButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow m = (MainWindow)Application.Current.MainWindow;
            m.mMainModel.UpdateMode((int)MDI_PlotGraph_Integration.Model.MainModel.ModeState.EditingMode);
            ResetButtons();
            EditingButton.IsChecked = true;
        }

        private void MatchingButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow m = (MainWindow)Application.Current.MainWindow;
            m.mMainModel.UpdateMode((int)MDI_PlotGraph_Integration.Model.MainModel.ModeState.MatchingMode);
            ResetButtons();
            MatchingButton.IsChecked = true;
        }

        private void ResetButtons()
        {
            this.EditingButton.IsChecked = false;
            this.MatchingButton.IsChecked = false;
            this.TrackingButton.IsChecked = false;
        }


    }
}
