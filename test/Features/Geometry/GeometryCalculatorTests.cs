using ConvexHull.Features.Geometry;
using ConvexHull.Models;

namespace ConvexHull.Tests.Features.Geometry;

public class GeometryCalculatorTests
{
    [Fact]
    public void CalculatePerimeter_Triangle_ReturnsCorrectValue()
    {
        var hull = new List<Point>
        {
            new(0, 0),
            new(3, 0),
            new(0, 4)
        };

        var perimeter = GeometryCalculator.CalculatePerimeter(hull);

        Assert.Equal(12, perimeter, 3);
    }

    [Fact]
    public void CalculateArea_Triangle_ReturnsCorrectValue()
    {
        var hull = new List<Point>
        {
            new(0, 0),
            new(3, 0),
            new(0, 4)
        };

        var area = GeometryCalculator.CalculateArea(hull);

        Assert.Equal(6, area, 3);
    }
}