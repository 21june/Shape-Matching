using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace ShapeMatching.Models
{
	public class ResultData : ObservableObject
	{
		// Result Rectangle
		private OpenCvSharp.Rect _rect;
		public OpenCvSharp.Rect Rect { get { return _rect; } set { SetProperty(ref _rect, value); } }
		// Score
		private double _mismatch;
		public double Mismatch { get { return _mismatch; } set { SetProperty(ref _mismatch, value); } }
	}
}
