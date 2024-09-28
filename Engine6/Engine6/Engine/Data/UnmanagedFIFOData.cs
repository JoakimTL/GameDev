using System.Runtime.CompilerServices;

namespace Engine.Data;

/// <summary>
/// When adding a new item, the oldest item is removed if the buffer would overflow.
/// </summary>
public unsafe sealed class UnmanagedFIFOData<T> : UnmanagedDataBase
	where T : unmanaged {

	private uint _maxElements;
	public nuint BytesUsed { get; private set; } = 0;
	public uint MaxElements { get => _maxElements; set => SetMaxElements( value ); }


	public UnmanagedFIFOData( uint maxElements ) : base( (uint) Unsafe.SizeOf<T>() * maxElements ) {
		BytesUsed = SizeBytes;
		_maxElements = maxElements;
	}

	private void SetMaxElements( uint value ) {
		if (Disposed) {
			this.LogWarning( "Already disposed." );
			return;
		}

		if (value < _maxElements) {
			this.LogWarning( "Cannot assign a lower max element count than current count." );
			return;
		}

		_maxElements = value;
		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();
		BytesUsed = _maxElements * typeSizeBytes;
		ResizeToFit( BytesUsed );
	}

	public void Add( T item ) {
		if (Disposed) {
			this.LogWarning( "Already disposed." );
			return;
		}

		uint typeSizeBytes = (uint) Unsafe.SizeOf<T>();

		Unsafe.CopyBlock( (byte*) this.Pointer + typeSizeBytes, this.Pointer, (uint) BytesUsed - typeSizeBytes );

		*(T*) ((byte*) this.Pointer) = item;
	}

	public void Set( T item, uint offsetBytes ) {
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

	public nuint GetElementCount()
		=> this.BytesUsed / (uint) Unsafe.SizeOf<T>();

	public void Flush()
		=> this.BytesUsed = 0;

	protected override void OnDispose() {
		this.BytesUsed = 0;
	}
}