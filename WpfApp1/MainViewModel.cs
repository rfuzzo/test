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
        #region fields

        private string _interpolationType;
        private ObservableCollection<GeneralizedPoint> _curve;
        private PointCollection _renderedPoints;
        private string _text;
        private Point _startPoint;

        #endregion

        public MainViewModel()
        {
            InterpolationTypes = Enum.GetNames<EInterpolationType>().ToList();
            RenderedPoints = new PointCollection();
            InterpolationType = EInterpolationType.EIT_Linear.ToString();

            //LoadCurve();
            //RenderCurve();
        }

        #region properties

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsLoaded { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

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

        #endregion

        /// <summary>
        ///     Load a curve into the editor
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void LoadCurve()
        {
            // load curve
            Curve = new ObservableCollection<GeneralizedPoint>
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
                    foreach (var p in _curve) p.IsControlPoint = false;
                    break;
                case EInterpolationType.EIT_BezierQuadratic:
                    // if even amount of points: not a quadratic curve
                    if (_curve.Count % 2 == 0) throw new NotImplementedException();

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

            IsLoaded = true;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public EInterpolationType GetInterpolationTypeEnum()
        {
            return Enum.Parse<EInterpolationType>(InterpolationType);
        }

        private bool VerifyCurve()
        {
            return _curve.All(x => x.Verify());
        }

        public double GetMaxT()
        {
            return _curve.Select(_ => _.T).Max();
        }

        public double GetMaxV()
        {
            return _curve.Select(_ => _.V).Max();
        }

        private void RenderCurve()
        {
            // scale points to canvas
            var maxT = GetMaxT();
            var maxV = GetMaxV();

            foreach (var p in _curve)
            {
                var normalizedT = p.T / maxT;
                var normalizedV = 1 - p.V / maxV;

                var scaledT = Math.Round(normalizedT * Width);
                var scaledV = Math.Round(normalizedV * Height);

                p.RenderPoint = new Point(scaledT, scaledV);
            }

            // verify curve integrity
            if (!VerifyCurve()) throw new ArgumentNullException();

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

        /// <summary>
        ///     Add a point to the curve
        /// </summary>
        /// <param name="pos"></param>
        public void Add(Point pos)
        {
            switch (GetInterpolationTypeEnum())
            {
                case EInterpolationType.EIT_Constant:
                case EInterpolationType.EIT_Linear:
                    // Add single point

                    break;
                case EInterpolationType.EIT_BezierQuadratic:
                    break;
                case EInterpolationType.EIT_BezierCubic:
                    break;
                case EInterpolationType.EIT_Hermite:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Re-renders the curve
        /// </summary>
        public void Reload()
        {
            if (Curve is null) return;
            OnPropertyChanged(nameof(Curve));
            RenderCurve();
        }
    }
}