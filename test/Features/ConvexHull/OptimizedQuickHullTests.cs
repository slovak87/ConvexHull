using ConvexHull.Features.ConvexHull;
using ConvexHull.Models;

namespace ConvexHull.Tests.Features.ConvexHull
{
    public class OptimizedQuickHullTests
    {
        [Fact]
        public void ComputeConvexHull_SimpleTriangle_ReturnsAllPoints()
        {
            var points = new List<Point>
            {
                new(0, 0),
                new(1, 0),
                new(0, 1)
            };

            var hull = ConvexHullCalculator.ComputeConvexHull(points);
            
            Assert.Equal(3, hull.Count);
            Assert.Contains(new Point(0, 0), hull);
            Assert.Contains(new Point(1, 0), hull);
            Assert.Contains(new Point(0, 1), hull);
        }

        [Fact]
        public void ComputeConvexHull_ColinearPoints_ReturnsEndpoints()
        {
            var points = new List<Point>
            {
                new(0, 0),
                new(1, 1),
                new(2, 2)
            };

            var hull = ConvexHullCalculator.ComputeConvexHull(points);

            Assert.Equal(2, hull.Count);
            Assert.Contains(new Point(0, 0), hull);
            Assert.Contains(new Point(2, 2), hull);
        }
    }
}