using System.Windows;
using System.Windows.Media;

namespace WpfApp1
{
    public class GeneralizedPoint
    {
        public GeneralizedPoint(double t, double v, bool isCtrlPoint = false)
        {
            RenderPoint = null;
            IsControlPoint = isCtrlPoint;
            Vector = new(t, v);
        }

        public GeneralizedPoint(Vector vector, bool isCtrlPoint = false)
        {
            RenderPoint = null;
            IsControlPoint = isCtrlPoint;
            Vector = vector;
        }

        public bool IsControlPoint { get; set; }

        public Point? RenderPoint { get; set; }

        public Vector Vector { get; set; }

        public bool IsSelected { get; set; }

        public double T
        {
            get => Vector.X;
            set => Vector = new(value, V);
        }

        public double V
        {
            get => Vector.Y;
            set => Vector = new(T, value);
        }

        public bool Verify()
        {
            return RenderPoint != null;
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
}