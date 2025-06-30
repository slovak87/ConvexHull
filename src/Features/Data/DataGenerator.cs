using ConvexHull.Models;

namespace ConvexHull.Features.Data;

internal class DataGenerator
{
    internal static List<Point> GenerateDemoData(int count)
    {
        var random = new Random(42); // Fixní seed pro reprodukovatelnost
        var points = new List<Point>();

        for (int i = 0; i < count; i++)
        {
            // Generování bodů v kruhu s několika outliers
            if (i < count * 0.9)
            {
                var angle = random.NextDouble() * 2 * Math.PI;
                var radius = random.NextDouble() * 100;
                points.Add(new Point(
                    Math.Cos(angle) * radius,
                    Math.Sin(angle) * radius
                ));
            }
            else
            {
                // Outlier body pro komplexnější obálku
                points.Add(new Point(
                    random.NextDouble() * 400 - 200,
                    random.NextDouble() * 400 - 200
                ));
            }
        }

        return points;
    }
}
