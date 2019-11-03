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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class FunctionTablePane : DockableContent
    {
        public FunctionTablePane()
        {
            InitializeComponent();
        }

        public void Add(String s)
        {
            this.TableListBox.Items.Add(s);
        }

        public void Clear()
        {
            for (int i = this.TableListBox.Items.Count - 1; i >= 1; i--)
            {
                this.TableListBox.Items.RemoveAt(i);
            }
        }
    }
}
