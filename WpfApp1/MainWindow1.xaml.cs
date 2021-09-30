using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    ///     Interaction logic for MainWindow1.xaml
    /// </summary>
    public partial class MainWindow1 : Window
    {
        public MainWindow1()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.LoadCurve();

                HandleInterpolation();
            }

            RenderPoints();
        }

        private void DrawAxes()
        {
            if (DataContext is MainViewModel vm)
            {
                var wxmax = /* vm.MaxT * */CanvasPoints.ActualWidth - MainViewModel.XMIN;
                var wymax = /*vm.MaxV * */CanvasPoints.ActualHeight - MainViewModel.YMIN;
                var wxmin = /* vm.MaxT * */CanvasPoints.ActualWidth - MainViewModel.XMIN;
                var wymin = /*vm.MaxV * */CanvasPoints.ActualHeight - MainViewModel.YMIN;
                const double xstep = 40;
                const double ystep = 40;
                const double xtic = 5;
                const double ytic = 5;

                // Make the X axis.
                var xaxisGeom = new GeometryGroup();
                var p0 = new Point(MainViewModel.XMIN, CanvasPoints.ActualHeight - MainViewModel.XMIN);
                var p1 = new Point(wxmax, CanvasPoints.ActualHeight - MainViewModel.XMIN);
                xaxisGeom.Children.Add(new LineGeometry(p0, p1));
                var p01 = new Point(MainViewModel.XMIN, MainViewModel.XMIN);
                var p11 = new Point(wxmax, MainViewModel.XMIN);
                xaxisGeom.Children.Add(new LineGeometry(p01, p11));


                for (var x = 2 * MainViewModel.XMIN; x <= wxmax; x += xstep)
                {
                    var tic0 = new Point(x, CanvasPoints.ActualHeight - MainViewModel.XMIN - ytic);
                    var tic1 = new Point(x, CanvasPoints.ActualHeight - MainViewModel.XMIN + ytic);
                    xaxisGeom.Children.Add(new LineGeometry(tic0, tic1));

                    // Label the tic mark's X coordinate.
                    var t = Math.Round(vm.ToWorldCoordinateX(x), 2);
                    DrawLabels(CanvasPoints, t.ToString(),
                        new Point(tic0.X, tic0.Y + 5), 12,
                        HorizontalAlignment.Center,
                        VerticalAlignment.Top);

                    var tic01 = new Point(x, MainViewModel.XMIN - ytic);
                    var tic11 = new Point(x, MainViewModel.XMIN + ytic);
                    xaxisGeom.Children.Add(new LineGeometry(tic01, tic11));

                    // Label the tic mark's X coordinate.
                    DrawLabels(CanvasPoints, t.ToString(),
                        new Point(tic01.X, tic01.Y - 25), 12,
                        HorizontalAlignment.Center,
                        VerticalAlignment.Top);
                }

                var xaxis_path = new Path
                {
                    StrokeThickness = 1,
                    Stroke = Brushes.Black,
                    Data = xaxisGeom
                };

                CanvasPoints.Children.Add(xaxis_path);

                // Make the Y axis.
                var yaxisGeom = new GeometryGroup();
                p0 = new Point(MainViewModel.XMIN, MainViewModel.YMIN);
                p1 = new Point(MainViewModel.XMIN, wymax);
                xaxisGeom.Children.Add(new LineGeometry(p0, p1));
                p01 = new Point(wxmax, MainViewModel.YMIN);
                p11 = new Point(wxmax, wymax);
                xaxisGeom.Children.Add(new LineGeometry(p01, p11));

                // total ticks
                for (var y = wymax; y >= MainViewModel.YMIN; y -= ystep)
                {
                    var tic0 = new Point(MainViewModel.XMIN - xtic, y);
                    var tic1 = new Point(MainViewModel.XMIN + xtic, y);
                    xaxisGeom.Children.Add(new LineGeometry(tic0, tic1));

                    // Label the tic mark's Y coordinate.
                    var v = Math.Round(vm.ToWorldCoordinateY(y), 2);
                    DrawLabels(CanvasPoints, v.ToString(),
                        new Point(tic0.X - 15, tic0.Y), 12,
                        HorizontalAlignment.Center,
                        VerticalAlignment.Center);

                    var tic01 = new Point(wxmax - xtic, y);
                    var tic11 = new Point(wxmax + xtic, y);
                    xaxisGeom.Children.Add(new LineGeometry(tic0, tic1));

                    // Label the tic mark's Y coordinate.
                    DrawLabels(CanvasPoints, v.ToString(),
                        new Point(tic01.X + 25, tic01.Y), 12,
                        HorizontalAlignment.Center,
                        VerticalAlignment.Center);
                }

                var yaxis_path = new Path
                {
                    StrokeThickness = 1,
                    Stroke = Brushes.Black,
                    Data = yaxisGeom
                };

                CanvasPoints.Children.Add(yaxis_path);
            }
        }

        private static void DrawLabels(Canvas can, string text, Point location, double fontSize, HorizontalAlignment halign, VerticalAlignment valign)
        {
            // Make the label.
            var label = new Label
            {
                Content = text,
                FontSize = fontSize,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent
            };
            can.Children.Add(label);

            // Position the label.
            label.Measure(new Size(double.MaxValue, double.MaxValue));

            double x = location.X;
            switch (halign)
            {
                case HorizontalAlignment.Center:
                    x -= label.DesiredSize.Width / 2;
                    break;
                case HorizontalAlignment.Right:
                    x -= label.DesiredSize.Width;
                    break;
            }
            Canvas.SetLeft(label, x);

            double y = location.Y;
            switch (valign)
            {
                case VerticalAlignment.Center:
                    y -= label.DesiredSize.Height / 2;
                    break;
                case VerticalAlignment.Bottom:
                    y -= label.DesiredSize.Height;
                    break;
            }
            Canvas.SetTop(label, y);
        }

        private void RenderPoints()
        {
            if (DataContext is not MainViewModel vm)
            {
                return;
            }
            if (vm.Curve is null)
            {
                return;
            }

            // unsubscribe from existing points
            foreach (UIElement p in CanvasPoints.Children)
            {
                p.MouseDown -= POnMouseDown;
                p.MouseUp -= POnMouseUp;
                p.MouseMove -= POnMouseMove;
            }

            // clear existing points
            CanvasPoints.Children.Clear();
            DrawAxes();

            // add points
            foreach (var generalizedPoint in vm.Curve)
            {
                var p = new Ellipse
                {
                    Stroke = Brushes.Black,
                    Fill = generalizedPoint.IsControlPoint ? Brushes.OrangeRed : Brushes.Yellow,
                    Width = 8,
                    Height = 8,
                    Tag = generalizedPoint
                };
                Canvas.SetLeft(p, generalizedPoint.RenderPoint.Value.X - 3);
                Canvas.SetTop(p, generalizedPoint.RenderPoint.Value.Y - 3);


                // subscribe
                p.MouseDown += POnMouseDown;
                p.MouseUp += POnMouseUp;
                p.MouseMove += POnMouseMove;

                CanvasPoints.Children.Add(p);
            }
        }


        #region drag and drop

        private Point? _dragStart;

        private void POnMouseMove(object sender, MouseEventArgs e)
        {
            if (DataContext is not MainViewModel vm) return;
            if (_dragStart != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var element = (UIElement)sender;
                var pos = e.GetPosition(CanvasPoints);

                var cpoint = vm.ClampToCanvas(pos);

                Canvas.SetLeft(element, cpoint.X - 3);
                Canvas.SetTop(element, cpoint.Y - 3);

                if (element is Ellipse ell)
                {
                    var model = (GeneralizedPoint)ell.Tag;

                    // find point on curve
                    var generalizedPoint = vm.Curve.FirstOrDefault(_ => _ == model);
                    if (generalizedPoint != null)
                    {
                        var (t, v) = vm.ToWorldCoordinates(cpoint.X, cpoint.Y);
                        generalizedPoint.T = t;
                        generalizedPoint.V = v;

                        vm.Reload(false);
                    }
                }
            }
        }

        private void POnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;
            _dragStart = null;
            element.ReleaseMouseCapture();
        }

        private void POnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;
            _dragStart = e.GetPosition(element);
            element.CaptureMouse();
        }

        #endregion


        private void Border_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is not MainViewModel vm)
            {
                return;
            }

            var pos = e.GetPosition(CanvasPoints);
            switch (e.ClickCount)
            {
                case 1:
                    // nothing
                    break;
                // Add new point
                case 2:
                    vm.AddPoint(pos);
                    this.RenderPoints();
                    break;
            }
        }

        private void CanvasPoints_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.Height = e.NewSize.Height - (2 * MainViewModel.YMIN);
                vm.Width = e.NewSize.Width - (2 * MainViewModel.XMIN);

                if (vm.Curve is not null)
                {
                    vm.Reload();
                }
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HandleInterpolation();
            if (DataContext is MainViewModel vm)
            {
                vm.Reload();
            }
        }

        private void HandleInterpolation()
        {
            if (DataContext is not MainViewModel vm) return;
            if (!vm.IsLoaded)
            {
                return;
            }

            // handle visibility
            switch (vm.GetInterpolationTypeEnum())
            {
                    
                case EInterpolationType.EIT_Linear:
                    CanvasLinearCurve.Visibility = Visibility.Visible;
                    CanvasQuadraticCurve.Visibility = Visibility.Collapsed;
                    CanvasCubicCurve.Visibility = Visibility.Collapsed;
                    break;
                case EInterpolationType.EIT_BezierQuadratic:
                    CanvasLinearCurve.Visibility = Visibility.Collapsed;
                    CanvasQuadraticCurve.Visibility = Visibility.Visible;
                    CanvasCubicCurve.Visibility = Visibility.Collapsed;
                    break;
                case EInterpolationType.EIT_BezierCubic:
                    CanvasLinearCurve.Visibility = Visibility.Collapsed;
                    CanvasQuadraticCurve.Visibility = Visibility.Collapsed;
                    CanvasCubicCurve.Visibility = Visibility.Visible;
                    break;
                case EInterpolationType.EIT_Constant:
                case EInterpolationType.EIT_Hermite:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CanvasPoints_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (DataContext is not MainViewModel vm) return;
            vm.Cursor = vm.ClampToCanvas(e.GetPosition(CanvasPoints));
        }

        private void MainWindow1_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.CurveReloaded += VmOnCurveReloaded;
            }
        }

        private void VmOnCurveReloaded(object sender, EventArgs e) => RenderPoints();

        
    }
}


