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

            RenderControlPoints();


            


        }

        private void DrawAxes()
        {
            if (DataContext is MainViewModel vm)
            {
                double wxmax =/* vm.MaxT * */this.CanvasPoints.ActualWidth - MainViewModel.XMIN;
                double wymax = /*vm.MaxV * */this.CanvasPoints.ActualHeight - MainViewModel.YMIN;
                const double xstep = 40;
                const double ystep = 40;
                const double xtic = 5;
                const double ytic = 5;

                // Make the X axis.
                GeometryGroup xaxis_geom = new GeometryGroup();
                Point p0 = new Point(MainViewModel.XMIN, CanvasPoints.ActualHeight - MainViewModel.XMIN);
                Point p1 = new Point(wxmax, CanvasPoints.ActualHeight - MainViewModel.XMIN);
                xaxis_geom.Children.Add(new LineGeometry((p0), (p1)));

                for (double x = 2 * MainViewModel.XMIN; x <= wxmax - xstep; x += xstep)
                {
                    Point tic0 = new Point(x, CanvasPoints.ActualHeight - MainViewModel.XMIN - ytic);
                    Point tic1 = new Point(x, CanvasPoints.ActualHeight - MainViewModel.XMIN + ytic);
                    xaxis_geom.Children.Add(new LineGeometry((tic0), (tic1)));
                }

                Path xaxis_path = new Path();
                xaxis_path.StrokeThickness = 1;
                xaxis_path.Stroke = Brushes.Black;
                xaxis_path.Data = xaxis_geom;

                this.CanvasPoints.Children.Add(xaxis_path);

                // Make the Y axis.
                GeometryGroup yaxis_geom = new GeometryGroup();
                p0 = new Point(MainViewModel.XMIN, MainViewModel.YMIN);
                p1 = new Point(MainViewModel.XMIN, wymax);
                xaxis_geom.Children.Add(new LineGeometry((p0), (p1)));

                for (double y = MainViewModel.YMIN; y <= wymax - ystep; y += ystep)
                {
                    Point tic0 = new Point(MainViewModel.XMIN - xtic, y);
                    Point tic1 = new Point(MainViewModel.XMIN + xtic, y);
                    xaxis_geom.Children.Add(new LineGeometry((tic0), (tic1)));
                }

                Path yaxis_path = new Path();
                yaxis_path.StrokeThickness = 1;
                yaxis_path.Stroke = Brushes.Black;
                yaxis_path.Data = yaxis_geom;

                this.CanvasPoints.Children.Add(yaxis_path);

            }
        }

        private void RenderControlPoints()
        {
            

            if (DataContext is not MainViewModel vm) return;

            if (vm.Curve is null) return;

            

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

        private Point? dragStart;

        private void POnMouseMove(object sender, MouseEventArgs e)
        {
            if (dragStart != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var element = (UIElement)sender;
                var p2 = e.GetPosition(CanvasPoints);
                var x = Math.Min(Math.Max(p2.X - dragStart.Value.X, 40), CanvasPoints.ActualWidth - MainViewModel.XMIN);
                var y = Math.Min(Math.Max(p2.Y - dragStart.Value.Y, 40), CanvasPoints.ActualHeight - MainViewModel.YMIN);

                Canvas.SetLeft(element, x - 3);
                Canvas.SetTop(element, y - 3);

                if (element is Ellipse ell && DataContext is MainViewModel vm)
                {
                    var model = (GeneralizedPoint)ell.Tag;

                    // find point on curve
                    var generalizedPoint = vm.Curve.FirstOrDefault(_ => _ == model);
                    if (generalizedPoint != null)
                    {
                        // scale down
                        var (t, v) = vm.ScaleDown(x - MainViewModel.XMIN, y - MainViewModel.YMIN);
                        generalizedPoint.T = t;
                        generalizedPoint.V = v;

                        vm.Reload();
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

        #endregion


        private void Border_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is not MainViewModel vm) return;
            var pos = e.GetPosition(CanvasPoints);
            if (e.ClickCount == 1)
            {
                // nothing
            }

            if (e.ClickCount == 2)
                // Add new point
            {
                vm.Add(pos);
                this.RenderControlPoints();
            }
        }

        // on canvas size changed
        private void CanvasPoints_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.Height = e.NewSize.Height - MainViewModel.YMIN;
                vm.Width = e.NewSize.Width - (2 * MainViewModel.XMIN);

                if (vm.Curve is not null)
                {
                    vm.Reload();
                    RenderControlPoints();
                }
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HandleInterpolation();
        }

        private void HandleInterpolation()
        {
            if (DataContext is MainViewModel vm)
            {
                if (!vm.IsLoaded)
                {
                    return;
                }

                // handle visibility
                switch (vm.GetInterpolationTypeEnum())
                {
                    case EInterpolationType.EIT_Constant:
                    case EInterpolationType.EIT_Linear:
                        CanvasLinearCurve.Visibility = Visibility.Visible;
                        CanvasQuadraticCurve.Visibility = Visibility.Collapsed;
                        break;
                    case EInterpolationType.EIT_BezierQuadratic:
                    case EInterpolationType.EIT_BezierCubic:
                    case EInterpolationType.EIT_Hermite:
                        CanvasQuadraticCurve.Visibility = Visibility.Visible;
                        CanvasLinearCurve.Visibility = Visibility.Collapsed;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void CanvasPoints_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (DataContext is not MainViewModel vm) return;
            var pos = e.GetPosition(CanvasPoints);

            var x = Math.Round(Math.Min(Math.Max(pos.X - 40, 0), this.CanvasPoints.ActualWidth - 80));
            var y = Math.Round(Math.Min(Math.Max(this.CanvasPoints.ActualHeight - 40 - pos.Y, 0), this.CanvasPoints.ActualHeight - 80));

            vm.Cursor = new Point(x, y);

        }
    }
}


