using ConvexHull.Enums;

namespace ConvexHull.Helpers
{
    // Pomocné datové struktury pro analýzu
    public struct InputAnalysis
    {
        public bool IsInteger { get; set; }
        public DataSize DataSize { get; set; }
        public double BoundingBoxSize { get; set; }
        public int PointCount { get; set; }
        public ProcessingStrategy RecommendedStrategy { get; set; }
        public double EstimatedReduction { get; set; }
    }

    // Zbytek vašeho existujícího kódu...
    // [Zde pokračuje celá vaše implementace OptimizedQuickHull, Point struct, atd.]
}