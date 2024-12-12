using ShapeMatching.Models;
using ShapeMatching.ViewModels.Bases;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using OpenCvSharp;
using System.Windows.Media.Imaging;
using OpenCvSharp.WpfExtensions;
using System.Collections.ObjectModel;
using ShapeMatching.Util;
using static System.Net.WebRequestMethods;
using System.DirectoryServices;

namespace ShapeMatching.ViewModels
{
	internal class MainWindowViewModel : ViewModelBase
	{
		// Binding List View
		private ObservableCollection<ResultData> _results;
		public ObservableCollection<ResultData> Results { get => _results; set => SetProperty(ref _results, value); }

		private ResultData? _sel_result;
		public ResultData? Sel_Result { get => _sel_result; set => SetProperty(ref _sel_result, value); }

		private ObservableCollection<ContourData> _cntrmodel;
		public ObservableCollection<ContourData> CntrModel { get => _cntrmodel; set => SetProperty(ref _cntrmodel, value); }

		// Target
		OpenCvSharp.Point[][] CntrTarget;
		HierarchyIndex[] HierarchiesTarget;


		private ContourData? _sel_cntrmodel;
		public ContourData? Sel_CntrModel { get => _sel_cntrmodel; set => SetProperty(ref _sel_cntrmodel, value); }

		// Binding Image
		private Mat _modelMat;
		private Mat _modelPreproMat;
		private BitmapSource _model;
		public BitmapSource Model { get => _model; set => SetProperty(ref _model, value); }

		private Mat _targetMat;
		private Mat _targetPreproMat;
		private BitmapSource _target;
		public BitmapSource Target { get => _target; set => SetProperty(ref _target, value); }

		// Binding etc...
		private int _modelsize = 0;
		public int ModelSize { get => _modelsize; set => SetProperty(ref _modelsize, value); }
		private int _targetsize = 0;
		public int TargetSize { get => _targetsize; set => SetProperty(ref _targetsize, value); }
		private double _mismatch = 0.01;
		public double Mismatch { get => _mismatch; set => SetProperty(ref _mismatch, value); }


		private bool _modelPrecCheck = false;
		public bool ModelPrecCheck { get => _modelPrecCheck; set => SetProperty(ref _modelPrecCheck, value); }

		private bool _targetPrecCheck = false;
		public bool TargetPrecCheck { get => _targetPrecCheck; set => SetProperty(ref _targetPrecCheck, value); }

		// Binding Command...
		public ICommand Func_SelectModel { get; }
		public ICommand Func_StartModel { get; }
		public ICommand Func_SelectTarget { get; }
		public ICommand Func_StartTarget { get; }


		public MainWindowViewModel()
		{
			Results = new ObservableCollection<ResultData> {};
			CntrModel = new ObservableCollection<ContourData> {};

			Mat m = Mat.Zeros(480, 640, MatType.CV_8UC1);
			Model = m.ToBitmapSource();
			Target = m.ToBitmapSource();

			Func_SelectModel = new RelayCommand(SelectModel);
			Func_StartModel = new RelayCommand(StartModel);
			Func_SelectTarget = new RelayCommand(SelectTarget);
			Func_StartTarget = new RelayCommand(StartTarget);
		}

		private void SelectModel()
		{
			string path = Utils.LoadFile();
			if (path == null) return;
			CntrModel.Clear();

			_modelMat = Cv2.ImRead(path, ImreadModes.Grayscale);
			Model = _modelMat.ToBitmapSource();
		}

		private void StartModel()
		{
			CntrModel.Clear();
			Mat src = _modelMat.Clone();
			Cv2.GaussianBlur(src, src, new OpenCvSharp.Size(5, 5), 0);
			Cv2.Threshold(src, src, 60, 255, ThresholdTypes.Binary);
			Cv2.AdaptiveThreshold(src, src, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.BinaryInv, 5, 2);
			_modelPreproMat = src.Clone();

			OpenCvSharp.Point[][] contours;
			HierarchyIndex[] hierarchy;

			Cv2.FindContours(src, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxTC89KCOS);
			Cv2.CvtColor(src, src, ColorConversionCodes.GRAY2RGB);
			List<OpenCvSharp.Point[]> new_contours = new List<OpenCvSharp.Point[]>();
			foreach (OpenCvSharp.Point[] p in contours)
			{
				OpenCvSharp.Rect boundbox = Cv2.BoundingRect(p);
				double arclen = Cv2.ArcLength(p.ToList(), true);
				if (arclen > ModelSize)
				{
					Random rnd = new Random();
					Color randomColor = Color.FromArgb((byte)rnd.Next(0), (byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256));
					Scalar randomScalar = Scalar.FromRgb(randomColor.R, randomColor.G, randomColor.B);
					foreach (var item in p)
					{
						Cv2.Circle(src, item, 5, randomScalar);
					}
					new_contours.Add(p);
					CntrModel.Add(new ContourData { Rect = boundbox, ArcLen = arclen, Contours = p.ToList() }); ;
				}
			}
			Model = src.ToBitmapSource();
		}

		private void SelectTarget()
		{
			Results.Clear();
			string path = Utils.LoadFile();
			if (path == null) return;

			_targetMat = Cv2.ImRead(path, ImreadModes.Grayscale);
			Target = _targetMat.ToBitmapSource();

		}

		private void StartTarget()
		{
			Results.Clear();

			if (Sel_CntrModel == null)
				return;

			Mat src = _targetMat.Clone();
			Cv2.GaussianBlur(src, src, new OpenCvSharp.Size(5, 5), 0);
			Cv2.Threshold(src, src, 60, 255, ThresholdTypes.Binary);
			Cv2.AdaptiveThreshold(src, src, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.BinaryInv, 5, 2);
			_targetPreproMat = src.Clone();

			Cv2.FindContours(src, out CntrTarget, out HierarchiesTarget, RetrievalModes.External, ContourApproximationModes.ApproxTC89KCOS);
			Cv2.CvtColor(src, src, ColorConversionCodes.GRAY2RGB);
			for (int i = 0; i < CntrTarget.Length; i++)
			{
				// Matching Shapes...
				double mis = Cv2.MatchShapes(CntrTarget[i], Sel_CntrModel.Contours, ShapeMatchModes.I1);
				double arclen = Cv2.ArcLength(CntrTarget[i].ToList(), true);

				// 임계값을 초과한 Contour 강조 표시
				if (mis < Mismatch && arclen > TargetSize)
				{
					OpenCvSharp.Point? pt = Utils.CalculateCentroid(CntrTarget[i]);
					if (pt.HasValue)
					{
						double arclen_mdl = Cv2.ArcLength(Sel_CntrModel.Contours, true);
						double scale = Utils.CalculateScale(arclen_mdl, arclen);
						string str_scale = string.Format("Scale: {0:F3}", scale);
						Cv2.Circle(src, pt.Value, 5, Scalar.Red, 3);
						Cv2.PutText(src, str_scale, pt.Value, HersheyFonts.HersheyTriplex, 0.5, Scalar.Blue);
					}

					OpenCvSharp.Rect boundingRect = Cv2.BoundingRect(CntrTarget[i]);
					Cv2.Rectangle(src, boundingRect, Scalar.Red, 2); // 빨간 네모로 표시
					Results.Add(new ResultData { Rect = boundingRect, Mismatch = mis });
				}

			}

			Target = src.ToBitmapSource();
		}

		public void UpdateSelModel()
		{
			if (Sel_CntrModel == null)
				return;

			Mat src;

			if (ModelPrecCheck)
				src = _modelPreproMat.Clone();
			else
				src = _modelMat.Clone();

			Cv2.Rectangle(src, Sel_CntrModel.Rect, new Scalar(255, 255, 255), 5);
			Model = src.ToBitmapSource();
		}
		public void UpdateSelTarget()
		{
			if (Sel_Result == null)
				return;

			Mat src;

			if (TargetPrecCheck)
				src = _targetPreproMat.Clone();
			else
				src = _targetMat.Clone();

			Cv2.Rectangle(src, Sel_Result.Rect, new Scalar(255, 255, 255), 5);
			Target = src.ToBitmapSource();
		}


	}

	public class RelayCommand : ICommand
	{
		private readonly Action _execute;
		private readonly Func<bool> _canExecute;

		public RelayCommand(Action execute, Func<bool> canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter) => _canExecute == null || _canExecute();
		public void Execute(object parameter) => _execute();

		public event EventHandler CanExecuteChanged;
		public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
	}
}