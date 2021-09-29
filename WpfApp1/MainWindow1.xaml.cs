using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

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
                vm.LoadCurve();
            }

            RenderControlPoints();

        }

        private void RenderControlPoints()
        {
            if (this.DataContext is not MainViewModel vm)
            {
                return;
            }

            // unsubscribe from existing points
            foreach (UIElement p in this.CanvasPoints.Children)
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
                    Stroke = System.Windows.Media.Brushes.Black,
                    Fill = System.Windows.Media.Brushes.Yellow,
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

                this.CanvasPoints.Children.Add(p);
            }
        }


        Point? dragStart = null;
        private void POnMouseMove(object sender, MouseEventArgs e)
        {
            if (dragStart != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var element = (UIElement)sender;
                var p2 = e.GetPosition(this.CanvasPoints);
                var x = Math.Min(Math.Max(p2.X - dragStart.Value.X, 0), this.CanvasPoints.ActualWidth);
                var y = Math.Min(Math.Max(p2.Y - dragStart.Value.Y, 0), this.CanvasPoints.ActualHeight);

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


        private void CanvasPoints_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.Height = e.NewSize.Height;
                vm.Width = e.NewSize.Width;
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