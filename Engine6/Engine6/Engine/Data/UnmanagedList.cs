using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine.Data;
public unsafe struct UnmanagedList : IDisposable {

	private void* _pointer;
	private uint _sizeBytes;
	private uint _bytesUsed;

	public uint BytePosition => _bytesUsed;

	public UnmanagedList( uint initialSizeBytes = 512 ) {
		_bytesUsed = 0;
		_sizeBytes = initialSizeBytes;
		_pointer = NativeMemory.Alloc( _sizeBytes );
	}

	public void Add<T>( T item ) where T : unmanaged {
		if ( _sizeBytes == 0 ) {
			this.LogWarning( "UnmanagedList has no size. The list may have been disposed or constructed wrongly." );
			return;
		}
		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();
		if ( _bytesUsed + typeSizeBytes > _sizeBytes ) {
			_sizeBytes *= 2;
			_pointer = NativeMemory.Realloc( _pointer, _sizeBytes );
		}
		*(T*) ( (byte*) _pointer + _bytesUsed ) = item;
		_bytesUsed += typeSizeBytes;
	}

	public void AddRange<T>( Span<T> items ) where T : unmanaged {
		if ( _sizeBytes == 0 ) {
			this.LogWarning( "UnmanagedList has no size. The list may have been disposed or constructed wrongly." );
			return;
		}
		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();
		uint currentSizeBytes = _sizeBytes;
		while ( _bytesUsed + (uint) items.Length * typeSizeBytes > _sizeBytes )
			_sizeBytes *= 2;
		if ( _sizeBytes > currentSizeBytes ) 
			_pointer = NativeMemory.Realloc( _pointer, _sizeBytes );
		fixed( void* ptr = items )
			Unsafe.CopyBlock( (byte*) _pointer + _bytesUsed, ptr, (uint) items.Length * typeSizeBytes );
		_bytesUsed += (uint) items.Length * typeSizeBytes;
	}

	public void Set<T>( T item, uint offsetBytes ) where T : unmanaged {
		if ( _sizeBytes == 0 ) {
			this.LogWarning( "UnmanagedList has no size. The list may have been disposed or constructed wrongly." );
			return;
		}
		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();
		if ( offsetBytes + typeSizeBytes > _bytesUsed ) {
			this.LogWarning( $"Tried to set an item at offset {offsetBytes} which is outside the size of the list." );
			return;
		}
		*(T*) ( (byte*) _pointer + offsetBytes ) = item;
	}

	public void Set<T>( Span<T> items, uint offsetBytes ) where T : unmanaged {
		if ( _sizeBytes == 0 ) {
			this.LogWarning( "UnmanagedList has no size. The list may have been disposed or constructed wrongly." );
			return;
		}
		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();
		if ( offsetBytes + (uint) items.Length * typeSizeBytes > _bytesUsed ) {
			this.LogWarning( $"Tried to set {items.Length} items at offset {offsetBytes} which is outside the size of the list." );
			return;
		}
		fixed( void* ptr = items )
			Unsafe.CopyBlock( (byte*) _pointer + offsetBytes, ptr, (uint) items.Length * typeSizeBytes );
	}

	public byte[] ToArray() {
		byte[] @returnArray = new byte[ _bytesUsed ];
		fixed ( void* ptr = @returnArray )
			Unsafe.CopyBlock( ptr, _pointer, _bytesUsed );
		return @returnArray;
	}

	public void Dispose() {
		_bytesUsed = 0;
		_sizeBytes = 0;
		NativeMemory.Free( _pointer );
		_pointer = null;
	}
}
