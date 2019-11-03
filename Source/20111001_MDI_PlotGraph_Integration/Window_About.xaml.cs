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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window_About : Window
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

        public Window_About()
        {
            InitializeComponent();
        }
        public static MessageBoxResult Show(Window owner)
        {
            Window_About aboutWindow = new Window_About();
            aboutWindow.Owner = owner;
            if (false == aboutWindow.ShowDialog())
            {
                return MessageBoxResult.Cancel;
            }

            return aboutWindow.MessageBoxResult;
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
