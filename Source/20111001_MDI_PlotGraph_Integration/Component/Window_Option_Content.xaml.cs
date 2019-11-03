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
using MDI_PlotGraph_Integration.Panels;

namespace MDI_PlotGraph_Integration.Component
{
    /// <summary>
    /// Interaction logic for Window_Option.xaml
    /// </summary>
    public partial class Window_Option_Content : UserControl
    {
        public Window_Option_Content()
        {
            InitializeComponent();
            this.GeneralOptionPane.Items.Add(new OptionPanelContent()
            {
                Name = "Generl_Option"
            });

            this.TrackingOptionPane.Items.Add(new ERROROptionPanelContent()
            {

            });

            this.EditingOptionPane.Items.Add(new ERROROptionPanelContent()
            {

            });

            this.MatchingOptionPane.Items.Add(new MatchingOptionPanelContent()
            {
                Name = "Matching_Option"
            });
        }
    }
}
