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
            if (DataContext is MainViewModel vm)
            {
                vm.LoadCurve();

                HandleInterpolation();
            }

            RenderControlPoints();
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

            // add points
            foreach (var generalizedPoint in vm.Curve)
            {
                var p = new Ellipse
                {
                    Stroke = Brushes.Black,
                    Fill = Brushes.Yellow,
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
                var x = Math.Min(Math.Max(p2.X - dragStart.Value.X, 0), CanvasPoints.ActualWidth);
                var y = Math.Min(Math.Max(p2.Y - dragStart.Value.Y, 0), CanvasPoints.ActualHeight);

                Canvas.SetLeft(element, x - 3);
                Canvas.SetTop(element, y - 3);

                if (element is Ellipse ell && DataContext is MainViewModel vm)
                {
                    var model = (GeneralizedPoint)ell.Tag;

                    // find point on curve
                    var f = vm.Curve.FirstOrDefault(_ => _ == model);
                    if (f != null)
                    {
                        // scale down
                        var nx = x / CanvasPoints.ActualWidth * vm.GetMaxT();
                        var ny = y / CanvasPoints.ActualHeight * vm.GetMaxV();

                        f.T = nx;
                        f.V = 1 - ny;
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
                vm.Add(pos);
        }

        // on canvas size changed
        private void CanvasPoints_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.Height = e.NewSize.Height;
                vm.Width = e.NewSize.Width;

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
    }
}


