namespace Engine.Standard.Render.Text.Fonts;

public unsafe sealed class FontDataReader( byte* dataPtr, int dataLength, bool dataIsLittleEndian ) : DisposableIdentifiable {
	private readonly byte* _dataPtr = dataPtr;
	private readonly int _dataLength = dataLength;
	private readonly bool _dataIsLittleEndian = dataIsLittleEndian;
	private readonly bool _archIsLittleEndian = BitConverter.IsLittleEndian;

	public T Read<T>( in nint offset, bool ensureEndianessMatchesArch = true ) where T : unmanaged {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		if (offset < 0)
			throw new Exception( "Reading before start of data" );
		if (offset + sizeof( T ) > this._dataLength)
			throw new Exception( "Reading past end of data" );
		T value = *(T*) (this._dataPtr + offset);
		return ensureEndianessMatchesArch ? EnsureEndianess( value ) : value;
	}

	protected override bool InternalDispose() {
		return true;
	}

	private T EnsureEndianess<T>( T value ) where T : unmanaged {
		if (this._dataIsLittleEndian == this._archIsLittleEndian)
			return value;
		T returnValue = default;
		byte* srcPtr = (byte*) &value;
		byte* dstPtr = (byte*) &returnValue;
		for (int i = 0; i < sizeof( T ); i++)
			dstPtr[ sizeof( T ) - i - 1 ] = srcPtr[ i ];
		return *(T*) dstPtr;
	}
}
