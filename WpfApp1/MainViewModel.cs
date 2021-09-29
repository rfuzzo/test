using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using WpfApp1.Annotations;

namespace WpfApp1
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public MainViewModel()
        {
            InterpolationTypes = Enum.GetNames<EInterpolationType>().ToList();
            RenderedPoints = new();
            InterpolationType = EInterpolationType.EIT_Linear.ToString();

            //LoadCurve();
            //RenderCurve();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<GeneralizedPoint> Curve
        {
            get => _curve;
            set
            {
                //if (Equals(value, _curve)) return;
                _curve = value;
                OnPropertyChanged();

                RenderCurve();
            }
        }

        public PointCollection RenderedPoints
        {
            get => _renderedPoints;
            set
            {
                //if (Equals(value, _renderedPoints)) return;
                _renderedPoints = value;
                OnPropertyChanged();
            }
        }

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
            }
        }

        private Point _startPoint;
        public Point StartPoint
        {
            get => _startPoint;
            set
            {
                //if (value.Equals(_startPoint)) return;
                _startPoint = value;
                OnPropertyChanged();
            }
        }

        public List<string> InterpolationTypes { get; } = new();

        private string _interpolationType;
        private ObservableCollection<GeneralizedPoint> _curve;
        private PointCollection _renderedPoints;


        public string InterpolationType
        {
            get => _interpolationType;
            set
            {
                if (value == _interpolationType) return;
                _interpolationType = value;
                OnPropertyChanged();
            }
        }

        public EInterpolationType GetInterpolationTypeEnum() => Enum.Parse<EInterpolationType>(InterpolationType);

        public void LoadCurve()
        {
            // load curve
            Curve = new ObservableCollection<GeneralizedPoint>()
            {
                new(0, 1),
                new(2.5, 1),
                new(10, 1),
                new(17.1, 0.99),
                new(25, 0.25)
            };
            //InterpolationType = EInterpolationType.EIT_Linear.ToString();
            InterpolationType = GetInterpolationTypeEnum().ToString();

            // set control points
            switch (GetInterpolationTypeEnum())
            {
                case EInterpolationType.EIT_Linear:
                    foreach (var p in _curve)
                    {
                        p.IsControlPoint = false;
                    }
                    break;
                case EInterpolationType.EIT_BezierQuadratic:
                    // if even amount of points: not a quadratic curve
                    if (_curve.Count % 2 == 0)
                    {
                        throw new NotImplementedException();
                    }

                    for (var i = 0; i < _curve.Count; i++)
                    {
                        var p = _curve[i];
                        p.IsControlPoint = i % 2 != 0;
                    }

                    break;
                case EInterpolationType.EIT_Constant:
                case EInterpolationType.EIT_BezierCubic:
                case EInterpolationType.EIT_Hermite:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool VerifyCurve() => _curve.All(x => x.Verify());
        public double GetMaxT() => _curve.Select(_ => _.T).Max();
        public double GetMaxV() => _curve.Select(_ => _.V).Max();

        private void RenderCurve()
        {
            // scale points to canvas
            var maxT = GetMaxT();
            var maxV = GetMaxV();

            foreach (var p in _curve)
            {
                var normalizedT = p.T / maxT;
                var normalizedV = 1 - (p.V / maxV);

                var scaledT = Math.Round(normalizedT * (double)Width);
                var scaledV = Math.Round(normalizedV * (double)Height);

                p.RenderPoint = new Point(scaledT, scaledV);
            }

            // verify curve integrity
            if (!VerifyCurve())
            {
                throw new ArgumentNullException();
            }

            List<Point> points;

            // render bezier segments
            switch (GetInterpolationTypeEnum())
            {
                case EInterpolationType.EIT_Linear:
                    // only take proper points (depending on LoadCurve)
                    var linearCurve = _curve.Where(x => !x.IsControlPoint).Select(x => x.RenderPoint.Value);
                    points = linearCurve.ToList();
                    break;
                case EInterpolationType.EIT_BezierQuadratic:
                    points = _curve.Select(x => x.RenderPoint.Value).ToList();
                    break;
                case EInterpolationType.EIT_Constant:
                case EInterpolationType.EIT_BezierCubic:
                case EInterpolationType.EIT_Hermite:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            StartPoint = points.First();
            RenderedPoints = new PointCollection(points.Skip(1));
        }

        //public void OnClicked(Point point) => Point1 = point;
        //public void OnClicked2(Point point) => Point2 = point;
        public void Add(Point pos)
        {


        }
    }
}