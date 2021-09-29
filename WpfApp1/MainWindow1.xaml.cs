using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp1.Annotations;

namespace WpfApp1
{
    public class GeneralizedPoint
    {
        public double T { get; set; }
        public double V { get; set; }
        public bool IsControlPoint { get; set; }

        public Point? RenderPoint { get; set; }

        public GeneralizedPoint(double t, double v)
        {
            RenderPoint = null;
            T = t;
            V = v;
        }

        public bool Verify()
        {
            return RenderPoint != null;
        }
    }

    public class RenderedCurve
    {
        public RenderedCurve(EInterpolationType eInterpolationType, Point startPoint, PointCollection renderedPoints)
        {
            EInterpolationType = eInterpolationType;
            StartPoint = startPoint;
            RenderedPoints = renderedPoints;
        }

        public EInterpolationType EInterpolationType { get; }
        public Point StartPoint { get; }
        public PointCollection RenderedPoints { get; }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private const int WIDTH = 600;
        private const int HEIGHT = 400;

        public MainViewModel()
        {
            InterpolationTypes = Enum.GetNames<EInterpolationType>().ToList();

            RenderedPoints = new PointCollection();

            //LoadCurve();
            //RenderCurve();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private List<GeneralizedPoint> _curve;

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
                if (value.Equals(_startPoint)) return;
                _startPoint = value;
                OnPropertyChanged();
            }
        }

        private PointCollection _renderedPoints;
        public System.Windows.Media.PointCollection RenderedPoints
        {
            get => _renderedPoints;
            set
            {
                if (Equals(value, _renderedPoints)) return;
                _renderedPoints = value;
                OnPropertyChanged();
            }
        }

        private bool _isLinear;
        public bool IsLinear
        {
            get => _isLinear;
            set
            {
                if (value == _isLinear) return;
                _isLinear = value;
                OnPropertyChanged();
            }
        }

        public List<string> InterpolationTypes { get; } = new();

        private string _interpolationType;


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

        private EInterpolationType GetInterpolationTypeEnum() => Enum.Parse<EInterpolationType>(InterpolationType);

        public void LoadCurve()
        {
            // load curve
            _curve = new List<GeneralizedPoint>()
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
        private double GetMaxT() => _curve.Select(_ => _.T).Max();
        private double GetMaxV() => _curve.Select(_ => _.V).Max();

        public RenderedCurve RenderCurve()
        {
            // scale points to canvas
            var maxT = GetMaxT();
            var maxV = GetMaxV();

            foreach (var p in _curve)
            {
                var normalizedT = p.T / maxT;
                var normalizedV = 1 - (p.V / maxV);

                var scaledT = Math.Round(normalizedT * WIDTH);
                var scaledV = Math.Round(normalizedV * HEIGHT);

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

            return new RenderedCurve(GetInterpolationTypeEnum(), StartPoint, RenderedPoints);


        }

        //public void OnClicked(Point point) => Point1 = point;
        //public void OnClicked2(Point point) => Point2 = point;
        public void Add(Point pos)
        {
            List<Point> points = new List<Point>();
            foreach (var point in RenderedPoints)
            {
                points.Add(point);
            }



        }
    }


    /// <summary>
    /// Interaction logic for MainWindow1.xaml
    /// </summary>
    public partial class MainWindow1 : Window
    {


        public MainWindow1()
        {
            InitializeComponent();



            //var times = new double[] { 0, 0.5, 1 };
            //var values = new double[] { 1, 0, 1 };

            //var plt = WpfPlot1.Plot;
            //plt.Style(ScottPlot.Style.Blue2);
            //plt.AddScatter(times, values, label: "X");
            //plt.XAxis.Label("Time (hours)");
            //plt.YAxis.Label("Value");
            //plt.XAxis2.Label($"REDName");

            //plt.Legend();

            //WpfPlot1.Render();


        }

        private void Border_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(this.Border);
            if (this.DataContext is MainViewModel vm)
            {
                vm.Add(pos);
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is MainViewModel vm)
            {
                vm.LoadCurve();

                var curve = vm.RenderCurve();


                this.PathSegmentCollection.Clear();
                this.PathFigure.StartPoint = curve.StartPoint;

                switch (curve.EInterpolationType)
                {
                    case EInterpolationType.EIT_Linear:
                        this.PathSegmentCollection.Add(new PolyLineSegment() { Points = curve.RenderedPoints });
                        break;
                    case EInterpolationType.EIT_BezierQuadratic:
                        this.PathSegmentCollection.Add(new PolyQuadraticBezierSegment() { Points = curve.RenderedPoints });
                        break;
                    case EInterpolationType.EIT_BezierCubic:
                    case EInterpolationType.EIT_Hermite:
                    case EInterpolationType.EIT_Constant:
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // add points
                var points = this.GeometryGroup.Children.Where(x => x is EllipseGeometry).ToList();
                foreach (var groupChild in points)
                {
                    this.GeometryGroup.Children.Remove(groupChild);
                }
                foreach (var point in curve.RenderedPoints)
                {
                    this.GeometryGroup.Children.Add(new EllipseGeometry(point, 2, 2));
                }
            }

        }
    }
}


//enum ESegmentsLinkType : Uint8
//{
//    ESLT_Normal,
//    ESLT_Smooth,
//    ESLT_SmoothSymmetric
//};

public enum EInterpolationType
{
    EIT_Constant,
    EIT_Linear,
    EIT_BezierQuadratic,
    EIT_BezierCubic,
    EIT_Hermite
};