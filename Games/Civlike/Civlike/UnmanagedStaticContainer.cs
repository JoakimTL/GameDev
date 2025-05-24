using Engine;
using System.Runtime.InteropServices;

namespace Civlike;

public sealed unsafe class UnmanagedStaticContainer<T> : DisposableIdentifiable where T : unmanaged {
	private readonly uint _count;
	private readonly T* _data;

	public UnmanagedStaticContainer( IReadOnlyList<T> data ) {
		this._count = (uint) data.Count;
		this._data = (T*) NativeMemory.Alloc( (nuint) (this._count * sizeof( T )) );
		for (int i = 0; i < this._count; i++)
			this._data[ i ] = data[ i ];
	}

	public uint Count => this._count;

	public T this[uint index] => Get( index );

	public T Get( uint index ) {
#if DEBUG
		if (index >= this._count)
			throw new ArgumentOutOfRangeException( nameof( index ), "Index is out of range." );
#endif
		return this._data[ index ];
	}

	protected override bool InternalDispose() {
		NativeMemory.Free( this._data );
		return true;
	}
}