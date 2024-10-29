namespace Engine;

public ref struct SpanQueue<T>( Span<T> underlyingSpan ) where T : unmanaged {
	public readonly Span<T> UnderlyingSpan = underlyingSpan;
	private int _currentDequeueIndex = 0;
	private int _currentQueueIndex = 0;

	public void Enqueue( T item ) {
		this.UnderlyingSpan[ this._currentQueueIndex++ ] = item;
		if (this._currentQueueIndex == this.UnderlyingSpan.Length)
			this._currentQueueIndex = 0;
	}

	public bool TryDequeue( out T item ) {
		item = default;
		if (this._currentDequeueIndex == this._currentQueueIndex)
			return false;
		item = this.UnderlyingSpan[ this._currentDequeueIndex++ ];
		if (this._currentDequeueIndex == this.UnderlyingSpan.Length)
			this._currentDequeueIndex = 0;
		return true;
	}
}

//public ref struct SpanSet<T>(Span<T> underlyingSpan ) where T : unmanaged {
//	public readonly Span<T> Span = underlyingSpan;

//	public void Add( T item ) {

//	}

//	public bool Contains( T item ) {
		
//	}

//	public bool Remove( T item ) {

//	}
//}