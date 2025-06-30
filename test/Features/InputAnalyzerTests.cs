using ConvexHull.Enums;
using ConvexHull.Features.InputAnalyzer;
using ConvexHull.Helpers;
using ConvexHull.Models;

namespace ConvexHull.Tests.Features;

public class InputAnalyzerTests
{
    [Fact]
    public void AnalyzeInputData_EmptyList_ReturnsEmptyDataSize()
    {
        // Arrange
        var points = new List<Point>();

        // Act
        var result = InputAnalyzer.AnalyzeInputData(points);

        // Assert
        Assert.False(result.IsInteger);
        Assert.Equal(DataSize.Empty, result.DataSize);
        Assert.Equal(0, result.PointCount);
    }

    [Fact]
    public void AnalyzeInputData_AllIntegers_SmallSet()
    {
        // Arrange
        var points = new List<Point>
        {
            new(1, 2),
            new(3, 4),
            new(-5, 6)
        };

        // Act
        var result = InputAnalyzer.AnalyzeInputData(points);

        // Assert
        Assert.True(result.IsInteger);
        Assert.Equal(DataSize.Small, result.DataSize);
        Assert.Equal(3, result.PointCount);
        Assert.Equal(ProcessingStrategy.JarvisMarch, result.RecommendedStrategy);
        Assert.Equal(0.0, result.EstimatedReduction, 3);
    }

    [Fact]
    public void AnalyzeInputData_RealNumbers_MediumSet()
    {
        // Arrange
        var points = new List<Point>();
        for (int i = 0; i < 200; i++)
            points.Add(new Point(i + 0.5, i * 2.1));

        // Act
        var result = InputAnalyzer.AnalyzeInputData(points);

        // Assert
        Assert.False(result.IsInteger);
        Assert.Equal(DataSize.Medium, result.DataSize);
        Assert.Equal(200, result.PointCount);
        Assert.Equal(ProcessingStrategy.SpanWithAklToussaint, result.RecommendedStrategy);
        Assert.Equal(0.85, result.EstimatedReduction, 2);
    }

    [Fact]
    public void AnalyzeInputData_LargeIntegerSet()
    {
        // Arrange
        var points = new List<Point>();
        for (int i = 0; i < 100_001; i++)
            points.Add(new Point(i, i));

        // Act
        var result = InputAnalyzer.AnalyzeInputData(points);

        // Assert
        Assert.True(result.IsInteger);
        Assert.Equal(DataSize.Large, result.DataSize);
        Assert.Equal(100_001, result.PointCount);
        Assert.Equal(ProcessingStrategy.MemoryPoolWithPreconditioning, result.RecommendedStrategy);
        Assert.Equal(0.95, result.EstimatedReduction, 2);
    }

    [Fact]
    public void AnalyzeInputData_BoundingBoxSize_Correct()
    {
        // Arrange
        var points = new List<Point>
        {
            new(1, 2),
            new(5, 10),
            new(-3, 7)
        };

        // Act
        var result = InputAnalyzer.AnalyzeInputData(points);

        // Assert
        // maxX - minX = 5 - (-3) = 8
        // maxY - minY = 10 - 2 = 8
        // boundingBoxRatio = max(8, 8) = 8
        Assert.Equal(8, result.BoundingBoxSize, 6);
    }
}
