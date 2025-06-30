using ConvexHull.Models;
using System.Runtime.CompilerServices;

namespace ConvexHull.Features.Geometry;

internal class GeometryCalculator
{
    internal static double CalculatePerimeter(List<Point> hull)
    {
        double perimeter = 0;
        for (int i = 0; i < hull.Count; i++)
        {
            var current = hull[i];
            var next = hull[(i + 1) % hull.Count];
            var dx = next.X - current.X;
            var dy = next.Y - current.Y;
            perimeter += Math.Sqrt(dx * dx + dy * dy);
        }
        return perimeter;
    }

    public static double CalculateArea(List<Point> hull)
    {
        double area = 0;
        for (int i = 0; i < hull.Count; i++)
        {
            var current = hull[i];
            var next = hull[(i + 1) % hull.Count];
            area += current.X * next.Y - next.X * current.Y;
        }
        return Math.Abs(area) / 2.0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Cross(Point O, Point A, Point B)
    {
        return (A.X - O.X) * (B.Y - O.Y) - (A.Y - O.Y) * (B.X - O.X);
    }
}
