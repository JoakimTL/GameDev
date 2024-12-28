namespace Engine.Module.Render.Input;

public sealed class EventChain<T>( uint eventCount ) where T : unmanaged {
	private readonly T[] _items = new T[ eventCount ];
	private int _currentIndex = 0;
	public IReadOnlyList<T> Chain => this._items[ ^this._currentIndex..this._currentIndex ];

	public void Add( T item ) {
		this._items[ this._currentIndex ] = item;
		this._currentIndex++;
		if (this._currentIndex >= this._items.Length)
			this._currentIndex = 0;
	}
}
