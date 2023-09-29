using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine.Data;
public unsafe class UnmanagedList : IDisposable {

	private void* _pointer;
	private uint _sizeBytes;
	private uint _bytesUsed;

	public uint BytePosition => this._bytesUsed;

	public UnmanagedList( uint initialSizeBytes = 512 ) {
		this._bytesUsed = 0;
		this._sizeBytes = initialSizeBytes;
		this._pointer = NativeMemory.Alloc( this._sizeBytes );
	}

	~UnmanagedList() {
		if ( this._sizeBytes == 0 ) {
			Log.Error( "UnmanagedList not disposed!" );
			this.Dispose();
		}
	}

	public void Add<T>( T item ) where T : unmanaged {
		if ( this._sizeBytes == 0 ) {
			this.LogWarning( "UnmanagedList has no size. The list may have been disposed or constructed wrongly." );
			return;
		}
		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();
		if ( this._bytesUsed + typeSizeBytes > this._sizeBytes ) {
			this._sizeBytes *= 2;
			this._pointer = NativeMemory.Realloc( this._pointer, this._sizeBytes );
		}
		*(T*) ( (byte*) this._pointer + this._bytesUsed ) = item;
		this._bytesUsed += typeSizeBytes;
	}

	public void AddRange<T>( Span<T> items ) where T : unmanaged {
		if ( this._sizeBytes == 0 ) {
			this.LogWarning( "UnmanagedList has no size. The list may have been disposed or constructed wrongly." );
			return;
		}
		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();
		uint currentSizeBytes = this._sizeBytes;
		while ( this._bytesUsed + (uint) items.Length * typeSizeBytes > this._sizeBytes )
			this._sizeBytes *= 2;
		if ( this._sizeBytes > currentSizeBytes )
			this._pointer = NativeMemory.Realloc( this._pointer, this._sizeBytes );
		fixed ( void* ptr = items )
			Unsafe.CopyBlock( (byte*) this._pointer + this._bytesUsed, ptr, (uint) items.Length * typeSizeBytes );
		this._bytesUsed += (uint) items.Length * typeSizeBytes;
	}

	public void Set<T>( T item, uint offsetBytes ) where T : unmanaged {
		if ( this._sizeBytes == 0 ) {
			this.LogWarning( "UnmanagedList has no size. The list may have been disposed or constructed wrongly." );
			return;
		}
		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();
		if ( offsetBytes + typeSizeBytes > this._bytesUsed ) {
			this.LogWarning( $"Tried to set an item at offset {offsetBytes} which is outside the size of the list." );
			return;
		}
		*(T*) ( (byte*) this._pointer + offsetBytes ) = item;
	}

	public void Set<T>( Span<T> items, uint offsetBytes ) where T : unmanaged {
		if ( this._sizeBytes == 0 ) {
			this.LogWarning( "UnmanagedList has no size. The list may have been disposed or constructed wrongly." );
			return;
		}
		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();
		if ( offsetBytes + (uint) items.Length * typeSizeBytes > this._bytesUsed ) {
			this.LogWarning( $"Tried to set {items.Length} items at offset {offsetBytes} which is outside the size of the list." );
			return;
		}
		fixed ( void* ptr = items )
			Unsafe.CopyBlock( (byte*) this._pointer + offsetBytes, ptr, (uint) items.Length * typeSizeBytes );
	}

	public bool TryGet<T>( out T output, uint offsetBytes ) where T : unmanaged {
		output = default;
		if ( this._sizeBytes == 0 ) {
			this.LogWarning( "UnmanagedList has no size. The list may have been disposed or constructed wrongly." );
			return false;
		}

		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();
		if ( offsetBytes + typeSizeBytes > this._bytesUsed ) {
			this.LogWarning( $"Tried to get an item at offset {offsetBytes} which is outside the size of the list." );
			return false;
		}
		output = *(T*) ( (byte*) this._pointer + offsetBytes );
		return true;
	}

	public bool TryPopulate<T>( Span<T> output, uint offsetBytes ) where T : unmanaged {
		if ( this._sizeBytes == 0 ) {
			this.LogWarning( "UnmanagedList has no size. The list may have been disposed or constructed wrongly." );
			return false;
		}

		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();
		if ( offsetBytes + (uint) output.Length * typeSizeBytes > this._bytesUsed ) {
			this.LogWarning( $"Tried to get {output.Length} items at offset {offsetBytes} which is outside the size of the list." );
			return false;
		}
		fixed ( void* ptr = output )
			Unsafe.CopyBlock( ptr, (byte*) this._pointer + offsetBytes, (uint) output.Length * typeSizeBytes );
		return true;
	}

	public uint GetElementCount<T>() where T : unmanaged 
		=> this._bytesUsed / (uint) Unsafe.SizeOf<T>();

	public void Flush() 
		=> this._bytesUsed = 0;

	public byte[] ToArray() {
		if ( this._sizeBytes == 0 ) {
			this.LogWarning( "UnmanagedList has no size. The list may have been disposed or constructed wrongly." );
			return Array.Empty<byte>();
		}
		if ( this._bytesUsed == 0 )
			return Array.Empty<byte>();
		byte[] @returnArray = new byte[ this._bytesUsed ];
		fixed ( void* ptr = @returnArray )
			Unsafe.CopyBlock( ptr, this._pointer, this._bytesUsed );
		return @returnArray;
	}
	public T[] ToArray<T>() where T : unmanaged {
		if ( this._sizeBytes == 0 ) {
			this.LogWarning( "UnmanagedList has no size. The list may have been disposed or constructed wrongly." );
			return Array.Empty<T>();
		}
		if ( this._bytesUsed == 0 )
			return Array.Empty<T>();
		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();
		if ( this._bytesUsed % typeSizeBytes != 0 ) {
			this.LogWarning( $"UnmanagedList has a size of {this._bytesUsed} bytes which is not a multiple of {typeSizeBytes} bytes for type {typeof( T )}." );
			return Array.Empty<T>();
		}
		T[] @returnArray = new T[ this._bytesUsed / typeSizeBytes ];
		fixed ( void* ptr = @returnArray )
			Unsafe.CopyBlock( ptr, this._pointer, this._bytesUsed );
		return @returnArray;
	}

	public void Dispose() {
		this._bytesUsed = 0;
		this._sizeBytes = 0;
		NativeMemory.Free( this._pointer );
		this._pointer = null;
	}
}
