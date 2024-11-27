using System.Runtime.CompilerServices;

namespace Engine.Structures;

//public ref struct SpanSet<T>(Span<T> underlyingSpan ) where T : unmanaged {
//	public readonly Span<T> Span = underlyingSpan;

//	public void Add( T item ) {

//	}

//	public bool Contains( T item ) {

//	}

//	public bool Remove( T item ) {

//	}
//}
public class SimpleSortedList<T>() where T : IComparable<T> {

	private readonly List<T> _underlying = [];

	public int Count => _underlying.Count;

	public T this[ int index ] {
		get => _underlying[ index ];
		set => _underlying[ index ] = value;
	}

	public void Add( T item ) {
		if (Count == 0) {
			_underlying.Add( item );
			return;
		}
		int index = FindIndex( 0, _underlying.Count, item );
		_underlying.Insert( index, item );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private int FindIndex( int start, int end, T item ) {
		int middle;
		T valueAtIndex;
		int order;
		while (true) {
			middle = start + (end - start) / 2;
			valueAtIndex = _underlying[ middle ];
			order = item.CompareTo( valueAtIndex );
			if (order == 0)
				return middle;
			if (middle == start)
				return order < 0 ? start : end;
			if (order < 0) {
				end = middle;
			} else {
				start = middle;
			}
		}
	}

	public void Clear() => _underlying.Clear();
}