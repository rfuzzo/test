using System.Windows;
using System.Windows.Media;

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