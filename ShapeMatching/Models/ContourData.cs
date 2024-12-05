using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OpenCvSharp;
using System.Collections.Generic;

namespace ShapeMatching.Models
{
	internal class ContourData : ObservableObject
	{
		// Result Rectangle
		private OpenCvSharp.Rect _rect;
		public OpenCvSharp.Rect Rect { get { return _rect; } set { SetProperty(ref _rect, value); } }
		// Score
		private double _arclen;
		public double ArcLen { get { return _arclen; } set { SetProperty(ref _arclen, value); } }
		
		// Contour
		private List<OpenCvSharp.Point> _contours;
		public List<OpenCvSharp.Point> Contours { get { return _contours; } set { SetProperty(ref _contours, value); } }

		private List<HierarchyIndex> _hierarchies;
		public List<HierarchyIndex> Hierarchies { get { return _hierarchies; } set { SetProperty(ref _hierarchies, value); } }

	}
}
