using ConvexHull.Enums;
using ConvexHull.Helpers;
using ConvexHull.Models;

namespace ConvexHull.Features.InputAnalyzer;

internal class InputAnalyzer
{
    internal static InputAnalysis AnalyzeInputData(List<Point> points)
    {
        if (points.Count == 0)
            return new InputAnalysis { IsInteger = false, DataSize = DataSize.Empty };

        // Detekce typu souřadnic
        bool isInteger = points.All(p =>
            p.X == Math.Floor(p.X) && p.Y == Math.Floor(p.Y) &&
            p.X >= int.MinValue && p.X <= int.MaxValue &&
            p.Y >= int.MinValue && p.Y <= int.MaxValue);

        // Klasifikace velikosti dat
        var dataSize = points.Count switch
        {
            <= 100 => DataSize.Small,
            <= 50000 => DataSize.Medium,
            _ => DataSize.Large
        };

        // Analýza distribuce bodů
        var minX = points.Min(p => p.X);
        var maxX = points.Max(p => p.X);
        var minY = points.Min(p => p.Y);
        var maxY = points.Max(p => p.Y);

        var boundingBoxRatio = Math.Max(maxX - minX, maxY - minY);

        return new InputAnalysis
        {
            IsInteger = isInteger,
            DataSize = dataSize,
            BoundingBoxSize = boundingBoxRatio,
            PointCount = points.Count,
            RecommendedStrategy = DetermineStrategy(points.Count, isInteger),
            EstimatedReduction = EstimateReductionEfficiency(points.Count, isInteger)
        };
    }

    public static ProcessingStrategy DetermineStrategy(int pointCount, bool isInteger)
    {
        return pointCount switch
        {
            <= 50 => ProcessingStrategy.JarvisMarch,
            <= 100 when isInteger => ProcessingStrategy.ListWithPreconditioning,
            <= 100 => ProcessingStrategy.ListWithAklToussaint,
            <= 50000 when isInteger => ProcessingStrategy.SpanWithPreconditioning,
            <= 50000 => ProcessingStrategy.SpanWithAklToussaint,
            _ when isInteger => ProcessingStrategy.MemoryPoolWithPreconditioning,
            _ => ProcessingStrategy.MemoryPoolWithAklToussaint
        };
    }

    public static double EstimateReductionEfficiency(int pointCount, bool isInteger)
    {
        if (pointCount <= 50) return 0.0; // Žádná redukce pro malé množiny

        return isInteger switch
        {
            true when pointCount > 1000 => 0.95, // 95% redukce pro velké celočíselné množiny
            true => 0.90, // 90% redukce pro střední celočíselné množiny
            false when pointCount > 10000 => 0.92, // 92% redukce pro velké reálné množiny
            false => 0.85 // 85% redukce pro střední reálné množiny
        };
    }
}
