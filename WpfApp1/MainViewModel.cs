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
        public const double XMIN = 40;
        public const double YMIN = 40;


















        #region fields

        private string _interpolationType;
        private ObservableCollection<GeneralizedPoint> _curve;
        private PointCollection _renderedPoints;
        private string _text;
        private Point _startPoint;
        private Point _cursor;

        #endregion

        public MainViewModel()
        {
            InterpolationTypes = Enum.GetNames<EInterpolationType>().ToList();
            RenderedPoints = new PointCollection();
            InterpolationType = EInterpolationType.EIT_Linear.ToString();














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

        public Point Cursor
        {
            get => _cursor;
            set
            {
                if (value.Equals(_cursor)) return;
                _cursor = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CursorPos));
            }
        }

        public ObservableCollection<GeneralizedPoint> Curve
        {
            get => _curve;
            set
            {
                //if (Equals(value, _curve)) return;
                _curve = value;
                OnPropertyChanged();

                OnPropertyChanged(nameof(MaxT));
                OnPropertyChanged(nameof(MaxV));
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
            RecalculateControlPoints();

            IsLoaded = true;
        }

        private void RecalculateControlPoints()
        {
            switch (GetInterpolationTypeEnum())
            {
                case EInterpolationType.EIT_Linear:
                    foreach (var p in Curve)
                    {
                        p.IsControlPoint = false;
                    }

                    break;
                case EInterpolationType.EIT_BezierQuadratic:
                    // if even amount of points: not a quadratic curve
                    for (var i = 0; i < Curve.Count; i++)
                    {
                        Curve[i].IsControlPoint = i % 2 != 0;
                    }

                    break;
                case EInterpolationType.EIT_Constant:
                case EInterpolationType.EIT_BezierCubic:
                case EInterpolationType.EIT_Hermite:
                default:
                    throw new ArgumentOutOfRangeException();
            }
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

        public double MaxT
        {
            get { return _curve.Select(_ => _.T).Max(); }
        }

        public double MaxV
        {
            get { return _curve.Select(_ => _.V).Max(); }
        }

        public string CursorPos
        {
            get
            {
                var (nx, ny) = ScaleDown(Cursor.X, Cursor.Y);
                return $"{Cursor.X} - {Cursor.Y} / {Math.Round(nx, 2)} - {Math.Round(ny, 2)}";
            }
        }

        private void RenderCurve()
        {
            // scale points to canvas
            var maxT = MaxT;
            var maxV = MaxV;

            foreach (var p in _curve)
            {
                var normalizedT = p.T / maxT;
                var normalizedV = 1 - p.V / maxV;

                var scaledT = Math.Round(normalizedT * Width) + XMIN;
                var scaledV = Math.Round(normalizedV * Height) + YMIN;

                p.RenderPoint = new Point(scaledT, scaledV);
            }

            // verify curve integrity
            if (!VerifyCurve()) throw new ArgumentNullException();

            RecalculateControlPoints();

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
            // Add single point
            var (t, v) = ScaleDown(pos.X - XMIN, pos.Y - YMIN);
            var point = new GeneralizedPoint(t, v);

            // insert
            var idx = Curve.Count;
            for (int i = 0; i < Curve.Count; i++)
            {
                var p = Curve[i];
                var thisT = p.T;
                if (t < thisT)
                {
                    idx = i;
                    break;
                }
            }

            Curve.Insert(idx, point);

            switch (GetInterpolationTypeEnum())
            {
                case EInterpolationType.EIT_Constant:
                case EInterpolationType.EIT_Linear:
                    break;
                case EInterpolationType.EIT_BezierQuadratic:
                    {
                        var previousPoint = Curve[idx - 1];
                        var nextPoint = Curve[idx + 1];
                        if (!previousPoint.IsControlPoint && nextPoint.IsControlPoint)
                        {
                            // add one control point before
                            var ct = Math.Max(0, (t + previousPoint.T) / 2);
                            var cv = Math.Max(0, (v + previousPoint.V) / 2);

                            var ctrlPoint = new GeneralizedPoint(ct, cv, true);
                            Curve.Insert(idx, ctrlPoint);
                        }
                        else if (previousPoint.IsControlPoint && !nextPoint.IsControlPoint)
                        {
                            // add one control point after
                            var ct = Math.Max(0, (nextPoint.T + t) / 2);
                            var cv = Math.Max(0, (nextPoint.V + v) / 2);

                            var ctrlPoint = new GeneralizedPoint(ct, cv, true);
                            Curve.Insert(idx + 1, ctrlPoint);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                        break;
                    }
                case EInterpolationType.EIT_BezierCubic:
                    break;
                case EInterpolationType.EIT_Hermite:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Reload();

        }

        public (double t, double v) ScaleDown(double x, double y)
        {
            var t = x / Width * MaxT;
            var v = 1 - (y / Height * MaxV);
            return (t, v);
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