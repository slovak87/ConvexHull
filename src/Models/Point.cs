using System.Runtime.CompilerServices;

namespace ConvexHull.Models
{
    // Optimalizovaná Point struktura s operators
    public readonly struct Point : IEquatable<Point>
    {
        public double X { get; }
        public double Y { get; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Point other) => X == other.X && Y == other.Y;

        public override bool Equals(object? obj) => obj is Point other && Equals(other);

        public static bool operator ==(Point left, Point right) => left.Equals(right);
        public static bool operator !=(Point left, Point right) => !left.Equals(right);

        public override int GetHashCode() => HashCode.Combine(X, Y);
        public override string ToString() => $"({X:F2}, {Y:F2})";
    }
}