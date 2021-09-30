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
        public const double YMIN = XMIN;


        #region fields

        private string _interpolationType;
        private ObservableCollection<GeneralizedPoint> _curve;
        private PointCollection _renderedPoints;
        private string _text;
        private Point _startPoint;
        private Point _cursor;
        private double _minX;
        private double _maxX;

        #endregion

        public MainViewModel()
        {
            InterpolationTypes = Enum.GetNames<EInterpolationType>().ToList();
            RenderedPoints = new PointCollection();
            InterpolationType = EInterpolationType.EIT_Linear.ToString();

        }

        #region properties

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler CurveReloaded;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnCurveReloaded()
        {
            CurveReloaded?.Invoke(this, EventArgs.Empty);
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

        public double MinT { get; set; }
        public double MaxT { get; set; }

        public double MinV { get; set; }
        public double MaxV { get; set; }


        public string CursorPos
        {
            get
            {
                var (nx, ny) = ScaleDown(Cursor.X, Cursor.Y);
                //return $"{Cursor.X} - {Cursor.Y} / {Math.Round(nx, 2)} - {Math.Round(MaxV - ny, 2)}";
                return $"T : {Math.Round(nx, 2)} - V : {Math.Round(MaxV - ny, 2)}";
            }
        }

        public double MinX
        {
            get => _minX;
            set
            {
                var calculatedValue = Curve is null ? value : Math.Min(value, GetCurveMinT());
                _minX = calculatedValue;
                OnPropertyChanged();

                MinT = _minX;
                Reload();
            }
        }

        public double MaxX
        {
            get => _maxX;
            set
            {
                var calculatedValue = Curve is null ? value : Math.Max(value, GetCurveMaxT());

                _maxX = calculatedValue;
                OnPropertyChanged();

                MaxT = _maxX;
                Reload();
            }
        }

        public double TransX
        {
            get
            {
                //if (CanvasGrid != null)
                {
                    return (/*Width  zoom - */Width) / 2;
                }
                //return 0;
            }
        }

        public double TransY
        {
            get
            {
                // if (CanvasGrid != null)
                {
                    return (/* CanvasGrid.Height  zoom -*/ Height) / 2;
                }
                return 0;
            }
        }

        #endregion

        private double GetCurveMinT() => Curve.Min(_ => _.T);
        private double GetCurveMaxT() => Curve.Max(_ => _.T);
        private double GetCurveMinV() => Curve.Min(_ => _.V);
        private double GetCurveMaxV() => Curve.Max(_ => _.V);

        /// <summary>
        ///     Load a curve into the editor
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void LoadCurve()
        {
            // load curve
            var c = new List<GeneralizedPoint>
            {
                new(0, 1.5),
                new(2.5, 1),
                new(10, 1),
                new(17.1, 0.99),
                new(25, 0.25)
            };

            MinT = c.Min(_ => _.T);
            MaxT = c.Max(_ => _.T);
            MinX = MinT;
            MaxX = MaxT;

            MinV = c.Min(_ => _.V);
            MaxV = c.Max(_ => _.V);


            Curve = new ObservableCollection<GeneralizedPoint>(c);

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
            var v = MaxV - (y / Height * MaxV);
            return (t, v);
        }

        /// <summary>
        ///     Re-renders the curve
        /// </summary>
        public void Reload(bool raiseEvent = true)
        {
            if (Curve is null)
            {
                return;
            }

            OnPropertyChanged(nameof(Curve));
            RenderCurve();

            if (raiseEvent)
            {
                OnCurveReloaded();
            }
        }
    }
}