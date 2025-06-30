namespace ConvexHull.Models
{
    // Adaptivní wrapper pro různé datové struktury
    public readonly ref struct PointCollection
    {
        private readonly Span<Point> _span;
        private readonly List<Point>? _list;
        private readonly bool _useSpan;

        public PointCollection(Span<Point> span)
        {
            _span = span;
            _list = null;
            _useSpan = true;
        }

        public PointCollection(List<Point> list)
        {
            _span = default;
            _list = list;
            _useSpan = false;
        }

        public int Count => _useSpan ? _span.Length : _list!.Count;
        public Point this[int index] => _useSpan ? _span[index] : _list![index];

        public Span<Point> AsSpan()
        {
            if (_useSpan) return _span;

            // Pro List vytvoříme Span - ideálně přes CollectionsMarshal v .NET 6+
            var array = new Point[_list!.Count];
            for (int i = 0; i < _list.Count; i++)
                array[i] = _list[i];
            return array.AsSpan();
        }
    }
}