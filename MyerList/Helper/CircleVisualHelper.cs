using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace MyerList.Helper
{
    public static class CircleVisualHelper
    {

        public static double GetLongestRadius(Point startPoint)
        {
            var finalBounds = Window.Current.Bounds;
            var distanceTo00 = Calculate2PointDistance(startPoint, new Point(0, 0));
            var distanceTo01 = Calculate2PointDistance(startPoint, new Point(0, finalBounds.Height));
            var distanceTo11 = Calculate2PointDistance(startPoint, new Point(finalBounds.Width, finalBounds.Height));
            var distanceTo10 = Calculate2PointDistance(startPoint, new Point(finalBounds.Width, 0));

            return Math.Max(Math.Max(distanceTo00, distanceTo01), Math.Max(distanceTo10, distanceTo11));
        }

        public static double Calculate2PointDistance(Point point1, Point point2)
        {
            return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
        }

        public static CubicBezierEasingFunction CreateCubicEasingFunc(Compositor compositor)
        {
            return compositor.CreateCubicBezierEasingFunction(  //these numbers seem to give a consistent circle even on small sized windows
                new Vector2(0.99f, 0.65f),
                new Vector2(0.1f, 0.4f)
                );
        }

        public static ScalarKeyFrameAnimation CreateBoomAnim(Compositor compositor, bool isIn, float? radiusRatioForIn)
        {
            var animation = compositor.CreateScalarKeyFrameAnimation();
            animation.InsertKeyFrame(1f, isIn ? (radiusRatioForIn == null ? 40f : radiusRatioForIn.Value) : 0f, CreateCubicEasingFunc(compositor));
            animation.Duration = TimeSpan.FromMilliseconds(300);

            return animation;
        }

    }
}
