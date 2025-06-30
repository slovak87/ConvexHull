using ConvexHull.Models;

namespace ConvexHull.Features.InputParser;

internal class InputParser
{
    internal static List<Point> ParseInputArguments(string[] args)
    {
        var points = new List<Point>();

        if (args.Length == 0) return points;

        try
        {
            // Kontrola, zda je první argument CSV soubor
            if (args.Length == 1 && args[0].EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                if (File.Exists(args[0]))
                {
                    return ParseCsvFile(args[0]);
                }
                else
                {
                    throw new FileNotFoundException($"CSV soubor '{args[0]}' nebyl nalezen.");
                }
            }

            // Spojení všech argumentů do jednoho řetězce
            var input = string.Join(" ", args);

            // Kontrola různých formátů vstupních dat
            if (ContainsSemicolonFormat(input))
            {
                return ParseSemicolonFormat(input);
            }
            else if (ContainsCommaFormat(input))
            {
                return ParseCommaFormat(input);
            }
            else
            {
                return ParseSpaceFormat(args);
            }
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Chyba při parsování vstupních dat: {ex.Message}");
        }
    }

    private static List<Point> ParseCsvFile(string csvFilePath)
    {
        var points = new List<Point>();

        try
        {
            var lines = File.ReadAllLines(csvFilePath);

            foreach (var line in lines)
            {
                // Přeskočení prázdných řádků a komentářů
                if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#"))
                    continue;

                var coords = line.Split(';', StringSplitOptions.RemoveEmptyEntries);

                // Každý řádek může obsahovat více bodů (x;y;x;y;...) nebo jeden bod (x;y)
                for (int i = 0; i < coords.Length - 1; i += 2)
                {
                    if (double.TryParse(coords[i].Trim(), out double x) &&
                        double.TryParse(coords[i + 1].Trim(), out double y))
                    {
                        points.Add(new Point(x, y));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Chyba při čtení CSV souboru: {ex.Message}");
        }

        return points;
    }

    private static List<Point> ParseSemicolonFormat(string input)
    {
        var points = new List<Point>();

        // Pro inline středníkový formát "x;y x;y x;y"
        var pointStrings = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var pointStr in pointStrings)
        {
            var coords = pointStr.Split(';');
            if (coords.Length == 2)
            {
                if (double.TryParse(coords[0].Trim(), out double x) &&
                    double.TryParse(coords[1].Trim(), out double y))
                {
                    points.Add(new Point(x, y));
                }
            }
        }

        return points;
    }

    private static bool ContainsSemicolonFormat(string input)
    {
        return input.Contains(";");
    }

    // Pro původní formát s čárkami "x,y x,y x,y"
    private static List<Point> ParseCommaFormat(string input)
    {
        var points = new List<Point>();
        var pointStrings = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var pointStr in pointStrings)
        {
            var coords = pointStr.Split(',');
            if (coords.Length == 2)
            {
                if (double.TryParse(coords[0], out double x) &&
                    double.TryParse(coords[1], out double y))
                {
                    points.Add(new Point(x, y));
                }
            }
        }

        return points;
    }

    // Pro formát kde každý argument je samostatná souřadnice "x y x y"
    private static List<Point> ParseSpaceFormat(string[] args)
    {
        var points = new List<Point>();

        // Každé dva argumenty tvoří jeden bod
        for (int i = 0; i < args.Length - 1; i += 2)
        {
            if (double.TryParse(args[i], out double x) &&
                double.TryParse(args[i + 1], out double y))
            {
                points.Add(new Point(x, y));
            }
        }

        return points;
    }

    private static bool ContainsCommaFormat(string input)
    {
        return input.Contains(',');
    }
}
