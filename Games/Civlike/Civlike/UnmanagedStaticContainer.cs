using Engine;
using System.Runtime.InteropServices;

namespace Civlike;

public sealed unsafe class UnmanagedStaticContainer<T> : DisposableIdentifiable where T : unmanaged {
	private readonly uint _count;
	private readonly T* _data;

	public UnmanagedStaticContainer( IReadOnlyList<T> data ) {
		_count = (uint) data.Count;
		_data = (T*) NativeMemory.Alloc( (nuint) (_count * sizeof( T )) );
		for (int i = 0; i < _count; i++)
			_data[ i ] = data[ i ];
	}

	public uint Count => _count;

	public T this[uint index] => Get( index );

	public T Get( uint index ) {
#if DEBUG
		if (index >= _count)
			throw new ArgumentOutOfRangeException( nameof( index ), "Index is out of range." );
#endif
		return _data[ index ];
	}

	protected override bool InternalDispose() {
		NativeMemory.Free( _data );
		return true;
	}
}