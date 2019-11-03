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
using OxyPlot;
using OxyPlot.Wpf;

using AvalonDock;
using MDI_PlotGraph_Integration.Model;
using MDI_PlotGraph_Integration.Functions;

namespace MDI_PlotGraph_Integration.Panels
{
    /// <summary>
    /// Interaction logic for GraphPlot.xaml
    /// </summary>
    public partial class GraphPlot : DocumentContent
    {
        MainWindow m;
        public readonly GraphPlotModel vm = new GraphPlotModel();
        private Rect mGraph_Rect;
        public Rect Graph_Rect
        {
            get{ return mGraph_Rect;}
        }

        public GraphPlot()
        {
            InitializeComponent();
            DataContext = vm;
            //CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        //private void CompositionTarget_Rendering(object sender, EventArgs e)
        //{
        //    plot1.RefreshPlot(true);
        //}

        public void Refresh(bool control)
        {
            plot1.RefreshPlot(control);
            
        }

        public void test()
        {
            vm.Test();
        }

        private void GraphPlotCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateGraphPlotCanvasSize();
        }

        private void UpdateGraphPlotCanvasSize()
        {
            mGraph_Rect.X = 0;
            mGraph_Rect.Y = 0;
            mGraph_Rect.Width = GraphPlotCanvas.ActualWidth;
            mGraph_Rect.Height = GraphPlotCanvas.ActualHeight;
        }

        private void ShowEquation_Click(object sender, RoutedEventArgs e)
        {
            m = (MainWindow)Application.Current.MainWindow;
            m.mMainModel.ShowEquation = !m.mMainModel.ShowEquation;
            ShowEquation.IsChecked = m.mMainModel.ShowEquation;
            m.mMainModel.DisplayEquation(m.mMainModel.ShowEquation);
        }

        private void radioBt_VertexForm_Checked(object sender, RoutedEventArgs e)
        {
            m = (MainWindow)Application.Current.MainWindow;
            if(m.mMainModel != null)
                m.mMainModel.SetParabolaEquationFormat(ParabolaGraphing.EquationForm.VertexForm);
        }

        private void radioBt_StandardForm_Checked(object sender, RoutedEventArgs e)
        {
            m = (MainWindow)Application.Current.MainWindow;
            if (m.mMainModel != null)
                m.mMainModel.SetParabolaEquationFormat(ParabolaGraphing.EquationForm.StandardForm);
        }

        private void radioBt_FactoredForm1_Checked(object sender, RoutedEventArgs e)
        {
            m = (MainWindow)Application.Current.MainWindow;
            if (m.mMainModel != null)
                m.mMainModel.SetParabolaEquationFormat(ParabolaGraphing.EquationForm.FactoredForm);
        }

        private void radioBt_FactoredForm2_Checked(object sender, RoutedEventArgs e)
        {
            m = (MainWindow)Application.Current.MainWindow;
            if (m.mMainModel != null)
                m.mMainModel.SetParabolaEquationFormat(ParabolaGraphing.EquationForm.FactoredFormNonCalcuated);
        }

        public void DisplayEquationFormConvertion(bool isParabola)
        {
            radioBt_StandardForm.Visibility = isParabola ? Visibility.Visible : Visibility.Hidden;
            radioBt_VertexForm.Visibility = isParabola ? Visibility.Visible : Visibility.Hidden;
            radioBt_FactoredForm1.Visibility = isParabola ? Visibility.Visible : Visibility.Hidden;
            radioBt_FactoredForm2.Visibility = isParabola ? Visibility.Visible : Visibility.Hidden;
        }


    }
}
