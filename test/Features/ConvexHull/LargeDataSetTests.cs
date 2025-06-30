using ConvexHull.Features.ConvexHull;
using ConvexHull.Models;
using System.Globalization;

namespace ConvexHull.Tests.Features.ConvexHull;

public class LargeDatasetTests
{
    public static IEnumerable<object[]> TestCases => new List<object[]>
    {
        new object[] { 1500 },
        new object[] { 5000 },
        new object[] { 65000 }
    };

    [Theory]
    [MemberData(nameof(TestCases))]
    public void ComputeConvexHull_LargeDataset_MatchesExpected(int pointCount)
    {
        // Nastavení invariantní kultury pro celý test
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        try
        {
            var (inputPoints, expectedHullIndices) = LoadTestData(pointCount);
            var computedHull = ConvexHullCalculator.ComputeConvexHull(inputPoints);
            VerifyHullResults(inputPoints, computedHull, expectedHullIndices, pointCount);
        }
        finally
        {
            // Obnovení původní kultury
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
    }

    // Hlavní metoda pro načítání testovacích dat
    private (List<Point> points, List<int> expectedIndices) LoadTestData(int pointCount)
    {
        // 1. Získání základního adresáře (kde běží assembly)
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;

        // 2. Možné cesty k testovacím datům
        var possiblePaths = new[]
        {
            // Cesta v build výstupu
            Path.Combine(baseDir, "TestData", "datasets"),
            
            // Cesta ve zdrojovém kódu (pro přímé spouštění z IDE)
            FindProjectRootDirectory()
        };

        // 3. Hledání existujících souborů
        var pointsFile = $"{pointCount}_points.csv";
        var indicesFile = $"{pointCount}_points_indices.txt";

        var foundPointsPath = string.Empty;
        var foundIndicesPath = string.Empty;

        foreach (var path in possiblePaths)
        {
            if (path == null) continue;

            var pointsPath = Path.Combine(path, pointsFile);
            var indicesPath = Path.Combine(path, indicesFile);

            if (File.Exists(pointsPath) && File.Exists(indicesPath))
            {
                foundPointsPath = pointsPath;
                foundIndicesPath = indicesPath;
                break;
            }
        }

        // 4. Kontrola nalezení souborů
        if (foundPointsPath == null || foundIndicesPath == null)
        {
            throw new FileNotFoundException(
                $"Testovací data pro {pointCount} bodů nebyla nalezena. Hledané cesty:\n" +
                string.Join("\n", possiblePaths.Select(p => $"- {p}")));
        }

        // 5. Načítání dat
        var points = ReadPointsFromCsv(foundPointsPath);
        var expectedIndices = File.ReadLines(foundIndicesPath)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(int.Parse)
            .ToList();

        return (points, expectedIndices);
    }

    // Metoda pro nalezení kořene projektu
    private string FindProjectRootDirectory()
    {
        try
        {
            // Start z adresáře, kde běží assembly
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            // Procházení nahoru, dokud nenajdeme .csproj soubor
            while (dir != null)
            {
                if (dir.GetFiles("*.csproj").Any())
                {
                    return Path.Combine(dir.FullName, "TestData", "datasets");
                }
                dir = dir.Parent;
            }
        }
        catch
        {
            // Ignorovat chyby při hledání
        }
        return string.Empty;
    }

    // Metoda pro čtení bodů z CSV s podporou různých formátů
    private List<Point> ReadPointsFromCsv(string filePath)
    {
        var points = new List<Point>();
        var culture = CultureInfo.InvariantCulture;

        foreach (var line in File.ReadLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Podpora různých oddělovačů
            var parts = line.Split(new[] { ';', ',', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) continue;

            // Normalizace čísel
            var xStr = parts[0].Replace(",", ".");
            var yStr = parts[1].Replace(",", ".");

            // Parsování
            if (double.TryParse(xStr, NumberStyles.Any, culture, out double x) &&
                double.TryParse(yStr, NumberStyles.Any, culture, out double y))
            {
                points.Add(new Point(x, y));
            }
        }
        return points;
    }

    private void VerifyHullResults(
        List<Point> inputPoints,
        List<Point> computedHull,
        List<int> expectedIndices,
        int pointCount)
    {
        // Ověření počtu bodů
        Assert.Equal(expectedIndices.Count, computedHull.Count);

        // Vytvoření očekávané obálky
        var expectedHull = expectedIndices
            .Select(index => inputPoints[index])
            .ToList();

        // Kontrola všech bodů
        foreach (var expectedPoint in expectedHull)
        {
            Assert.Contains(expectedPoint, computedHull);
        }
    }
}