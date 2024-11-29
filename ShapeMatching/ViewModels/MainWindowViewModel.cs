using ShapeMatching.Models;
using ShapeMatching.ViewModels.Bases;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShapeMatching.ViewModels
{
	internal class MainWindowViewModel : ViewModelBase
	{
		private IList<ResultData> _results;
		public IList<ResultData> Results { get => _results; set => SetProperty(ref _results, value); }

		private ResultData? _selectedResult;
		public ResultData? SelectedResult { get => _selectedResult; set => SetProperty(ref _selectedResult, value); }

		public MainWindowViewModel()
		{
			Results = new List<ResultData>
			{
				new ResultData{Rect = new Rect(0, 0, 100, 100), Score = 0.8},
				new ResultData{Rect = new Rect(100, 100, 150, 175), Score = 0.75},
				new ResultData{Rect = new Rect(47, 50, 227, 320), Score = 0.16}
			};
		}
	}
}