namespace ConvexHull.Extensions
{
    // Extension metoda pro kopírování List do Span
    public static class ListExtensions
    {
        public static void CopyTo<T>(this List<T> list, Span<T> destination)
        {
            for (int i = 0; i < list.Count && i < destination.Length; i++)
            {
                destination[i] = list[i];
            }
        }
    }
}