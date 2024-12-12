using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace ShapeMatching.Util
{
    class Utils
    {
		// Load Image Files
		static public string LoadFile()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "PNG files (*.png)|*.png|BMP files (*.bmp)|*.bmp|JPG files (*.jpg)|*.jpg|JPEG files (*.jpeg)|*.jpeg|All files (*.*)|*.*";
			if (openFileDialog.ShowDialog() == true)
			{
				return openFileDialog.FileName;
			}
			return null;
		}



		// get scale..
		static public double CalculateScale(double A, double B)
		{
			if (B == 0)
				return 0;
			return A / B;
		}
		
		// get center..
		static public OpenCvSharp.Point? CalculateCentroid(OpenCvSharp.Point[] A)
		{
			Moments moments = Cv2.Moments(A);

			// 2. 중앙점 계산
			if (moments.M00 == 0)
				return null;

			double cx = (moments.M10 / moments.M00);
			double cy = (moments.M01 / moments.M00);

			return new OpenCvSharp.Point(cx, cy);
		}
	}
}
