using ConvexHull.Features.Geometry;
using ConvexHull.Models;
using System.Buffers;

namespace ConvexHull.Features.ConvexHull;

public static class ConvexHullCalculator
{
    private const int ALGORITHM_SWITCH = 50;
    private const int STRUCT_THRESHOLD = 100;
    private const int PARALLEL_THRESHOLD = 1000;
    private const int MEMORYPOOL_THRESHOLD = 50000;

    public static List<Point> ComputeConvexHull(List<Point> points)
    {
        if (points == null || points.Count == 0)
            return new List<Point>();

        if (points.Count < ALGORITHM_SWITCH)
        {
            // Jarvis March - vhodný pro velmi malé množiny bodů
            return JarvisMarch(points);
        }
        else if (points.Count < STRUCT_THRESHOLD)
        {
            // Malé množiny - QuickHull na List<Point> bez paralelizace
            return QuickHull(points);
        }
        else if (points.Count < MEMORYPOOL_THRESHOLD)
        {
            // Střední množiny - použijeme Span a případně paralelizaci
            if (points.Count >= PARALLEL_THRESHOLD)
            {
                return QuickHullParallel(points);
            }
            else
            {
                return QuickHull(points.ToArray().AsSpan());
            }
        }
        else
        {
            // Velké množiny - použijeme ArrayPool a paralelizaci
            return QuickHullWithMemoryPool(points);
        }
    }

    private static List<Point> JarvisMarch(List<Point> points)
    {
        if (points.Count < 3)
            return [.. points];

        List<Point> hull = [];

        // Najdi levý nejnižší bod
        int leftMost = 0;
        for (int i = 1; i < points.Count; i++)
            if (points[i].X < points[leftMost].X || (points[i].X == points[leftMost].X && points[i].Y < points[leftMost].Y))
                leftMost = i;

        int p = leftMost;
        do
        {
            hull.Add(points[p]);
            int q = -1;

            for (int i = 0; i < points.Count; i++)
            {
                if (i == p)
                    continue;

                if (q == -1)
                {
                    q = i;
                    continue;
                }

                double cross = GeometryCalculator.Cross(points[p], points[q], points[i]);

                if (cross > 0)
                {
                    q = i;
                }
                else if (cross == 0)
                {
                    // Pokud jsou kolineární, vezmeme ten nejvzdálenější bod
                    double dist1 = GeometryCalculator.DistanceSquared(points[p], points[q]);
                    double dist2 = GeometryCalculator.DistanceSquared(points[p], points[i]);
                    if (dist2 > dist1)
                        q = i;
                }
            }

            p = q;

        } while (p != leftMost);

        return hull;
    }


    private static List<Point> QuickHull(List<Point> points)
    {
        var span = points.ToArray().AsSpan();
        return QuickHull(span);
    }

    private static List<Point> QuickHull(ReadOnlySpan<Point> points)
    {
        if (points.Length < 3)
            return points.ToArray().ToList();

        int minPoint = 0, maxPoint = 0;
        for (int i = 1; i < points.Length; i++)
        {
            if (points[i].X < points[minPoint].X)
                minPoint = i;
            if (points[i].X > points[maxPoint].X)
                maxPoint = i;
        }

        List<Point> hull = new();

        Point A = points[minPoint];
        Point B = points[maxPoint];

        // Pro rozdělení bodů podle polohy vzhledem k úsečce AB
        var leftSet = new List<Point>();
        var rightSet = new List<Point>();

        for (int i = 0; i < points.Length; i++)
        {
            if (i == minPoint || i == maxPoint) continue;

            double cross = GeometryCalculator.Cross(A, B, points[i]);
            if (cross > 0)
                leftSet.Add(points[i]);
            else if (cross < 0)
                rightSet.Add(points[i]);
            // kolineární body ignoruj
        }

        FindHull(leftSet, A, B, hull);
        FindHull(rightSet, B, A, hull);

        // Přidej extrémní body na začátek a konec výsledného obalu
        // Přidáme je až nakonec, aby nevznikaly duplicity
        hull.Insert(0, A);
        hull.Add(B);

        return hull;
    }

    private static void FindHull(List<Point> points, Point P, Point Q, List<Point> hull)
    {
        if (points.Count == 0) return;

        // Najdi bod C s největší vzdáleností od úsečky PQ
        double maxDistance = double.MinValue;
        int furthestIndex = -1;
        for (int i = 0; i < points.Count; i++)
        {
            double distance = DistancePointToLine(points[i], P, Q);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthestIndex = i;
            }
        }

        if (furthestIndex == -1) return;

        Point C = points[furthestIndex];

        // Rozděl body na dvě podmnožiny, které jsou nalevo od PC a CQ
        var leftSetPC = new List<Point>();
        var leftSetCQ = new List<Point>();

        for (int i = 0; i < points.Count; i++)
        {
            if (i == furthestIndex) continue;

            double crossPC = GeometryCalculator.Cross(P, C, points[i]);
            if (crossPC > 0)
                leftSetPC.Add(points[i]);

            double crossCQ = GeometryCalculator.Cross(C, Q, points[i]);
            if (crossCQ > 0)
                leftSetCQ.Add(points[i]);
        }

        FindHull(leftSetPC, P, C, hull);
        hull.Add(C);  // Přidávej bod C až po rekurzi, aby byl hull správně uspořádaný
        FindHull(leftSetCQ, C, Q, hull);
    }

    private static double DistancePointToLine(Point pt, Point A, Point B)
    {
        // Vzdálenost bodu pt od úsečky AB
        double area = Math.Abs(GeometryCalculator.Cross(A, B, pt));
        double AB_length = Math.Sqrt((B.X - A.X) * (B.X - A.X) + (B.Y - A.Y) * (B.Y - A.Y));
        return area / AB_length;
    }

    private static List<Point> QuickHullParallel(List<Point> points)
    {
        // Rozděl data na bloky (např. 4 bloky)
        int blockCount = Environment.ProcessorCount;
        int blockSize = points.Count / blockCount;

        var tasks = new Task<List<Point>>[blockCount];
        for (int i = 0; i < blockCount; i++)
        {
            int start = i * blockSize;
            int end = (i == blockCount - 1) ? points.Count : start + blockSize;
            var block = points.GetRange(start, end - start);

            tasks[i] = Task.Run(() => QuickHull(block));
        }

        Task.WaitAll(tasks);

        // Sloučíme všechny částečné hully a spustíme QuickHull znovu
        List<Point> combinedPoints = tasks.SelectMany(t => t.Result).Distinct().ToList();

        // Re-run QuickHull nad kombinovanými body
        return QuickHull(combinedPoints);
    }

    private static List<Point> QuickHullWithMemoryPool(List<Point> points)
    {
        // Příklad použití ArrayPool pro velké množiny
        var pool = ArrayPool<Point>.Shared;

        Point[] rentedArray = pool.Rent(points.Count);
        try
        {
            points.CopyTo(rentedArray, 0);
            var span = new ReadOnlySpan<Point>(rentedArray, 0, points.Count);

            // Stejná metoda jako s Span
            return QuickHull(span);
        }
        finally
        {
            pool.Return(rentedArray, clearArray: true);
        }
    }
}