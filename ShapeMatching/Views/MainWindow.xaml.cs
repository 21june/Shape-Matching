using ShapeMatching.ViewModels;
using System;
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
using OpenCvSharp;
using OpenCvSharp.Flann;
using Microsoft.Win32;

namespace ShapeMatching
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
	{
		private MainWindowViewModel _viewModel;

		private void ListChangedModel(object sender, SelectionChangedEventArgs e)
		{
			_viewModel.UpdateSelModel();
		}

		private void ListChangedTarget(object sender, SelectionChangedEventArgs e)
		{
			_viewModel.UpdateSelTarget();
		}


		public MainWindow()
        {
            InitializeComponent();
			_viewModel = new MainWindowViewModel(); ;
			this.DataContext = _viewModel;
        }

	}
}