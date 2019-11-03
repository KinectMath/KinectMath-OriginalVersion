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

namespace MDI_PlotGraph_Integration
{
    /// <summary>
    /// Interaction logic for Window_Option.xaml
    /// </summary>
    public partial class Window_Option : Window
    {
        private MessageBoxResult _result = MessageBoxResult.None;
        private Button _close;
        private FrameworkElement _title;

        public MessageBoxResult MessageBoxResult
        {
            get { return this._result; }
            private set
            {
                this._result = value;
                if (MessageBoxResult.Cancel == this._result)
                {
                    this.DialogResult = false;
                }
                else
                {
                    this.DialogResult = true;
                }
            }
        }

        public Window_Option()
        {
            InitializeComponent();
        }

        public Window_Option(String s)
        {
            InitializeComponent();
            switch (s)
            {
                case "General":
                    this.Win_Option_Content.General.IsSelected = true;
                    break;
                case "TrackingMode":
                    this.Win_Option_Content.TrackingMode.IsSelected = true;
                    break;
                case "EditingMode":
                    this.Win_Option_Content.EditingMode.IsSelected = true;
                    break;
                case "MatchingMode":
                    this.Win_Option_Content.MatchingMode.IsSelected = true;
                    break;
            }
        }

        public static MessageBoxResult Show(Window owner, string s)
        {
            MainWindow m = (MainWindow)Application.Current.MainWindow;
            m.mOptionWindow = new Window_Option();
            m.mOptionWindow.Owner = owner;

            switch (s)
            {
                case "General":
                    m.mOptionWindow.Win_Option_Content.General.IsSelected = true;
                    break;
                case "TrackingMode":
                    m.mOptionWindow.Win_Option_Content.TrackingMode.IsSelected = true;
                    break;
                case "EditingMode":
                    m.mOptionWindow.Win_Option_Content.EditingMode.IsSelected = true;
                    break;
                case "MatchingMode":
                    m.mOptionWindow.Win_Option_Content.MatchingMode.IsSelected = true;
                    break;
            }

            if (false == m.mOptionWindow.ShowDialog())
            {
                return MessageBoxResult.Cancel;
            }

            return m.mOptionWindow.MessageBoxResult;
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            this.MessageBoxResult = MessageBoxResult.OK;
        }

        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            this._close = (Button)this.Template.FindName("PART_Close", this);
            if (null != this._close)
            {
                this._close.MouseLeftButtonDown += new MouseButtonEventHandler(ok_Click);
            }

            this._title = (FrameworkElement)this.Template.FindName("PART_Title", this);
            if (null != this._title)
            {
                this._title.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);
            }
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
