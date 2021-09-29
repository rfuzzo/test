using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
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
        private double WIDTH = 600;
        private double HEIGHT = 400;

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

        public void LoadCurve(double h, double w)
        {
            HEIGHT = h;
            WIDTH = w;

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
        }

        //public void OnClicked(Point point) => Point1 = point;
        //public void OnClicked2(Point point) => Point2 = point;
        public void Add(Point pos)
        {


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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is MainViewModel vm)
            {
                var h = this.CanvasCurve.ActualHeight;
                var w = this.CanvasCurve.ActualWidth;

                vm.LoadCurve(h, w);

                //this.PathSegmentCollection.Clear();
                //this.PathFigure.StartPoint = vm.StartPoint;

                switch (vm.GetInterpolationTypeEnum())
                {
                    case EInterpolationType.EIT_Linear:
                        //this.PathSegmentCollection.Add(new PolyLineSegment() { Points = vm.RenderedPoints });
                        break;
                    case EInterpolationType.EIT_BezierQuadratic:
                        //this.PathSegmentCollection.Add(new PolyQuadraticBezierSegment() { Points = vm.RenderedPoints });
                        break;
                    case EInterpolationType.EIT_BezierCubic:
                    case EInterpolationType.EIT_Hermite:
                    case EInterpolationType.EIT_Constant:
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // add points
                foreach (UIElement p in this.CanvasPoints.Children)
                {
                    p.MouseDown -= POnMouseDown;
                    p.MouseUp -= POnMouseUp;
                    p.MouseMove -= POnMouseMove;
                }

                CanvasPoints.Children.Clear();
                foreach (var point in vm.RenderedPoints)
                {
                    var gpoint = vm.Curve.FirstOrDefault(x => x.RenderPoint == point);
                    if (gpoint == null)
                    {

                    }


                    var p = new Ellipse
                    {
                        Stroke = System.Windows.Media.Brushes.Black,
                        Fill = System.Windows.Media.Brushes.Yellow,
                        Width = 8,
                        Height = 8,
                        Tag = gpoint
                    };
                    Canvas.SetTop(p, point.Y - 3);
                    Canvas.SetLeft(p, point.X - 3);

                    p.MouseDown += POnMouseDown;
                    p.MouseUp += POnMouseUp;
                    p.MouseMove += POnMouseMove;

                    this.CanvasPoints.Children.Add(p);
                }
                //var points = this.GeometryGroup.Children.Where(x => x is EllipseGeometry).ToList();
                //foreach (var groupChild in points)
                //{
                //    this.GeometryGroup.Children.Remove(groupChild);
                //}
                //foreach (var point in curve.RenderedPoints)
                //{
                //    var ellipse = new EllipseGeometry(point, 3, 3);

                //    this.GeometryGroup.Children.Add(ellipse);
                //}
            }

        }

        Point? dragStart = null;
        private void POnMouseMove(object sender, MouseEventArgs e)
        {
            if (dragStart != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var element = (UIElement)sender;
                var p2 = e.GetPosition(this.CanvasPoints);
                var x = p2.X - dragStart.Value.X;
                var y = p2.Y - dragStart.Value.Y;
                Canvas.SetLeft(element, x - 3);
                Canvas.SetTop(element, y - 3);

                if (element is Ellipse ell && this.DataContext is MainViewModel vm)
                {
                    var model = (GeneralizedPoint)ell.Tag;

                    // find point on curve
                    var f = vm.Curve.FirstOrDefault(_ => _ == model);
                    if (f != null)
                    {
                        // scale down
                        var nx = x / this.CanvasCurve.ActualWidth * vm.GetMaxT();
                        var ny = y / this.CanvasCurve.ActualHeight * vm.GetMaxV();


                        f.T = nx;
                        f.V = 1 - ny;
                        vm.Curve = vm.Curve;

                        
                    }
                    else
                    {
                        
                    }


                }

            }
        }
        private void POnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;
            dragStart = null;
            element.ReleaseMouseCapture();
        }
        private void POnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;
            dragStart = e.GetPosition(element);
            element.CaptureMouse();

        }



        private void Border_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (this.DataContext is not MainViewModel vm)
            {
                return;
            }
            var pos = e.GetPosition(this.CanvasPoints);
            if (e.ClickCount == 1)
            {

            }
            if (e.ClickCount == 2)
            {

                vm.Add(pos);
            }


        }

        private void Canvas_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(this.CanvasPoints);
            //var points = this.GeometryGroup.Children.Where(x => x is EllipseGeometry).ToList();
            //foreach (var groupChild in points)
            //{
            //    var rpos = groupChild.Bounds;

            //    if (pos.X > rpos.Left && pos.X < rpos.Right)
            //    {
            //        if (pos.Y < rpos.Bottom && pos.Y > rpos.Top)
            //        {
            //            OnMouseOver(groupChild);
            //        }
            //    }
            //    else
            //    {
            //        var ell = (groupChild as EllipseGeometry);
            //        ell.RadiusX = 3;
            //        ell.RadiusY = 3;
            //    }
            //}

        }

        private void OnMouseOver(Geometry groupChild)
        {
            var ell = (groupChild as EllipseGeometry);
            ell.RadiusX = 5;
            ell.RadiusY = 5;
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