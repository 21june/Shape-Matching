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
		private Rect _rect;
		public Rect Rect { get { return _rect; } set { SetProperty(ref _rect, value); } }
		// Score
		private double _score;
		public double Score { get { return _score; } set { SetProperty(ref _score, value); } }

		public override string ToString()
		{
			return "Rect: ( " + Rect + " )" + " / Accuracy: " + ( Score * 100 ) + " %";
		}
	}
}
