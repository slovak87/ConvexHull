using ConvexHull.Features.ConsoleRender;
using ConvexHull.Features.ConvexHull;
using ConvexHull.Features.Data;
using ConvexHull.Features.InputAnalyzer;
using ConvexHull.Features.InputParser;
using System.Diagnostics;

namespace ConvexHull
{
    class Program
    {
        static void Main(string[] args)
        {
            args = new[] { @"0.102345,0.756234
0.532876,0.120984
0.845213,0.943512
0.234512,0.453218
0.653421,0.234521" };
            try
            {
                Console.WriteLine("=== Optimalizovaná konvexní obálka ===");
                var points = InputParser.ParseInputArguments(args);

                if (points.Count == 0)
                {
                    Console.WriteLine("Použití: ConvexEnvelope.exe \"x1,y1 x2,y2 x3,y3 ...\"");
                    Console.WriteLine("Příklad: ConvexEnvelope.exe \"0,0 1,1 2,0 1,2 0.5,0.5\"");

                    // Demo data pro ukázku
                    points = DataGenerator.GenerateDemoData(1000);
                    Console.WriteLine($"Použitá demo data: {points.Count} náhodných bodů");
                }

                var analysis = InputAnalyzer.AnalyzeInputData(points);
                ConsoleRender.DisplayAnalysis(analysis, points.Count);

                // Měření výkonu
                var stopwatch = Stopwatch.StartNew();

                // Výpočet konvexní obálky s optimalizovanou strategií
                var hull = ConvexHullCalculator.ComputeConvexHull(points);

                stopwatch.Stop();

                ConsoleRender.DisplayResults(hull, stopwatch.ElapsedMilliseconds, analysis);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba: {ex.Message}");
                Environment.Exit(1);
            }
        }
    }
}