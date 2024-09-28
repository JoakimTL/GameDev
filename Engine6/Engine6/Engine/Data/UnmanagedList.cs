using System.Runtime.CompilerServices;

namespace Engine.Data;

public unsafe sealed class UnmanagedList( uint initialSizeBytes = 512 ) : UnmanagedDataBase( initialSizeBytes ) {
	public nuint BytesUsed { get; private set; } = 0;

	public void Add<T>( T item ) where T : unmanaged {
		if (Disposed) {
			this.LogWarning( "Already disposed." );
			return;
		}
		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();

		ResizeToFit( this.BytesUsed + typeSizeBytes );

		*(T*) ((byte*) this.Pointer + this.BytesUsed) = item;
		this.BytesUsed += typeSizeBytes;
	}

	public void AddRange<T>( Span<T> items ) where T : unmanaged {
		if (Disposed) {
			this.LogWarning( "Already disposed." );
			return;
		}
		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();

		ResizeToFit( this.BytesUsed + ((uint) items.Length * typeSizeBytes) );

		fixed (void* ptr = items)
			Unsafe.CopyBlock( (byte*) this.Pointer + this.BytesUsed, ptr, (uint) items.Length * typeSizeBytes );
		this.BytesUsed += (uint) items.Length * typeSizeBytes;
	}

	public void Set<T>( T item, uint offsetBytes ) where T : unmanaged {
		if (Disposed) {
			this.LogWarning( "Already disposed." );
			return;
		}
		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();
		if (offsetBytes + typeSizeBytes > this.BytesUsed) {
			this.LogWarning( $"Tried to set an item at offset {offsetBytes} which is outside the size of the list." );
			return;
		}
		*(T*) ((byte*) this.Pointer + offsetBytes) = item;
	}

	public void Set<T>( Span<T> items, uint offsetBytes ) where T : unmanaged {
		if (Disposed) {
			this.LogWarning( "Already disposed." );
			return;
		}
		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();
		if (offsetBytes + ((uint) items.Length * typeSizeBytes) > this.BytesUsed) {
			this.LogWarning( $"Tried to set {items.Length} items at offset {offsetBytes} which is outside the size of the list." );
			return;
		}
		fixed (void* ptr = items)
			Unsafe.CopyBlock( (byte*) this.Pointer + offsetBytes, ptr, (uint) items.Length * typeSizeBytes );
	}

	public nuint GetElementCount<T>() where T : unmanaged
		=> this.BytesUsed / (uint) Unsafe.SizeOf<T>();

	public void Flush()
		=> this.BytesUsed = 0;

	protected override void OnDispose() {
		this.BytesUsed = 0;
	}
}
