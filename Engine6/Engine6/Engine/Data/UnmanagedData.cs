using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine.Data;

public unsafe abstract class UnmanagedData : Identifiable, IDisposable {

	protected void* Pointer { get; private set; }
	public nuint SizeBytes { get; private set; }

	public bool Disposed => Pointer == null;

	public UnmanagedData( nuint initialSizeBytes = 512 ) {
		this.SizeBytes = initialSizeBytes;
		this.Pointer = NativeMemory.Alloc( this.SizeBytes );
	}

	~UnmanagedData() {
		if (this.Pointer != null) {
			this.LogError( "Not disposed!" );
			this.Dispose();
		}
	}

	protected void Realloc( nuint newSizeBytes, bool downsize ) {
		if (!downsize && newSizeBytes <= this.SizeBytes)
			return;
		this.Pointer = NativeMemory.Realloc( this.Pointer, newSizeBytes );
		this.SizeBytes = newSizeBytes;
	}

	protected void ResizeToFit( nuint bytes ) {
		nuint newSizeBytes = this.SizeBytes;
		while (bytes > newSizeBytes)
			newSizeBytes *= 2;
		if (newSizeBytes > this.SizeBytes)
			Realloc( newSizeBytes, false );
	}

	public T Read<T>( uint offsetBytes ) where T : unmanaged {
		if (Pointer == null)
			return this.LogWarningThenReturnDefault<T>( $"Already disposed." );
		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();
		if (offsetBytes + typeSizeBytes > this.SizeBytes)
			return this.LogWarningThenReturnDefault<T>( $"Tried to access data outside the size of the unmanaged data." );
		return *(T*) ((byte*) this.Pointer + offsetBytes);
	}

	public bool TryRead<T>( uint offsetBytes, Span<T> resultStorage ) where T : unmanaged {
		if (Pointer == null)
			return this.LogWarningThenReturn( $"Already disposed.", false );
		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();
		if (offsetBytes + (typeSizeBytes * (uint) resultStorage.Length) > this.SizeBytes)
			return this.LogWarningThenReturn( $"Tried to access data outside the size of the unmanaged data.", false );
		fixed (void* dstPtr = resultStorage)
			Unsafe.CopyBlock( dstPtr, (byte*) this.Pointer + offsetBytes, (uint) resultStorage.Length * typeSizeBytes );
		return true;
	}

	public T[] ToArray<T>( nuint offsetBytes, nuint lengthBytes ) where T : unmanaged {
		if (Disposed)
			return this.LogWarningThenReturn( "Already disposed.", Array.Empty<T>() );
		if (offsetBytes + lengthBytes > this.SizeBytes)
			return this.LogWarningThenReturn( "Tried to access data outside the size of the unmanaged data.", Array.Empty<T>() );
		return new nuint( (byte*) this.Pointer + offsetBytes ).ToArray<T>( lengthBytes );
	}

	public void Dispose() {
		OnDispose();
		NativeMemory.Free( this.Pointer );
		this.Pointer = null;
		this.SizeBytes = nuint.Zero;
	}

	protected abstract void OnDispose();
}
