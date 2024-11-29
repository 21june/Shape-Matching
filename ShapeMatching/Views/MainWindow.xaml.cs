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
		Mat inModel, inTarget;
		public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
			InitWidget();
        }

		private void InitWidget()
		{
			inModel = Mat.Zeros(480, 640, MatType.CV_8UC1);
			inTarget = Mat.Zeros(480, 640, MatType.CV_8UC1);

			image_model.Source = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(inModel);
			image_target.Source = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(inTarget);
		}

		private string PickImageFile()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "PNG files (*.png)|*.png|BMP files (*.bmp)|*.bmp|JPG files (*.jpg)|*.jpg|JPEG files (*.jpeg)|*.jpeg|All files (*.*)|*.*";
			if (openFileDialog.ShowDialog() == true)
			{
				return openFileDialog.FileName;
			}
			return null;
		}

		/*
		 * Widget Event
		 */
		private void Btn_Click(object sender, RoutedEventArgs e)
		{
			// Model
			if (sender.Equals(btn_model_start))
			{
			}
			else if (sender.Equals(btn_model_search))
			{
				string path = PickImageFile();
				if (path != null)
				{
					inModel = Cv2.ImRead(path, ImreadModes.Grayscale);
					image_model.Source = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(inModel);
				}
			}

			// Target
			else if (sender.Equals(btn_target_start))
			{
			}
			else if (sender.Equals(btn_target_search))
			{
				string path = PickImageFile();
				if (path != null)
				{
					inTarget = Cv2.ImRead(path, ImreadModes.Grayscale);
					image_target.Source = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(inTarget);
				}
			}
		}
	}
}