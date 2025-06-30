using ConvexHull.Enums;
using ConvexHull.Features.Geometry;
using ConvexHull.Helpers;
using ConvexHull.Models;

namespace ConvexHull.Features.ConsoleRender;

internal class ConsoleRender
{
    private static string GetStrategyDescription(ProcessingStrategy strategy) => strategy switch
    {
        ProcessingStrategy.JarvisMarch => "Jarvis March (optimální pro malé množiny)",
        ProcessingStrategy.ListWithAklToussaint => "List<Point> + Akl-Toussaint redukce",
        ProcessingStrategy.ListWithPreconditioning => "List<Point> + Preconditioning (celočíselné)",
        ProcessingStrategy.SpanWithAklToussaint => "Span<Point> + Akl-Toussaint redukce",
        ProcessingStrategy.SpanWithPreconditioning => "Span<Point> + Preconditioning (celočíselné)",
        ProcessingStrategy.MemoryPoolWithAklToussaint => "MemoryPool + Akl-Toussaint + Paralelizace",
        ProcessingStrategy.MemoryPoolWithPreconditioning => "MemoryPool + Preconditioning + Paralelizace",
        _ => "Neznámá strategie"
    };

    private static string GetDataSizeDescription(DataSize size) => size switch
    {
        DataSize.Small => "Malá (≤ 100 bodů)",
        DataSize.Medium => "Střední (100 - 50 000 bodů)",
        DataSize.Large => "Velká (> 50 000 bodů)",
        _ => "Neznámá"
    };

    internal static void DisplayAnalysis(InputAnalysis analysis, int pointCount)
    {
        Console.WriteLine($"\n--- Analýza vstupních dat ---");
        Console.WriteLine($"Počet bodů: {pointCount:N0}");
        Console.WriteLine($"Typ souřadnic: {(analysis.IsInteger ? "Celočíselné" : "Reálné (double)")}");
        Console.WriteLine($"Velikost dat: {GetDataSizeDescription(analysis.DataSize)}");
        Console.WriteLine($"Doporučená strategie: {GetStrategyDescription(analysis.RecommendedStrategy)}");

        if (analysis.EstimatedReduction > 0)
        {
            Console.WriteLine($"Očekávaná redukce dat: {analysis.EstimatedReduction:P1}");
        }

        Console.WriteLine($"Ohraničující box: {analysis.BoundingBoxSize:F2}");
    }

    internal static void DisplayResults(List<Point> hull, long elapsedMs, InputAnalysis analysis)
    {
        Console.WriteLine($"\n--- Výsledky ---");
        Console.WriteLine($"Čas výpočtu: {elapsedMs} ms");
        Console.WriteLine($"Vrcholy konvexní obálky: {hull.Count}");
        Console.WriteLine($"Použitá strategie: {GetStrategyDescription(analysis.RecommendedStrategy)}");

        // Geometrické vlastnosti
        if (hull.Count >= 3)
        {
            var perimeter = GeometryCalculator.CalculatePerimeter(hull);
            var area = GeometryCalculator.CalculateArea(hull);
            Console.WriteLine($"Obvod obálky: {perimeter:F3}");
            Console.WriteLine($"Plocha obálky: {area:F3}");
        }

        Console.WriteLine("\nVrcholy obálky:");
        foreach (var point in hull)
        {
            Console.WriteLine($"  {point}");
        }
    }
}
