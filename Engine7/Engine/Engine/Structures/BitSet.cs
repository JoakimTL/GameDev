namespace Engine.Structures;
public sealed class BitSet {

	private readonly List<byte> _bytes;

	public readonly ReadOnlyBitSet ReadOnly;

	public BitSet( int capacity ) {
		_bytes = new List<byte>( (capacity + 7) / 8 );
		for (int i = 0; i < _bytes.Capacity; i++)
			_bytes.Add( 0 );
		ReadOnly = new( this );
	}

	/// <returns>True if the value was flipped</returns>
	public bool Set( uint index, bool value ) {
		int signedIndex = (int) index;
		int byteIndex = signedIndex / 8;
		int bitIndex = signedIndex - byteIndex * 8;

		while (byteIndex >= _bytes.Count)
			_bytes.Add( 0 );

		byte currentByte = _bytes[ byteIndex ];
		byte bitMask = (byte) (1 << bitIndex);
		bool oldValue = (currentByte & bitMask) != 0;
		if (value)
			currentByte |= bitMask;
		else
			currentByte &= (byte) ~bitMask;
		_bytes[ byteIndex ] = currentByte;
		return oldValue != value;
	}

	public bool Get( uint index ) {
		int signedIndex = (int) index;
		int byteIndex = signedIndex / 8;
		int bitIndex = signedIndex - byteIndex * 8;
		if (byteIndex >= _bytes.Count)
			return false;
		byte currentByte = _bytes[ byteIndex ];
		return (currentByte & (1 << bitIndex)) != 0;
	}

	public byte GetByte( uint byteIndex ) {
		if (byteIndex >= _bytes.Count)
			return 0;
		return _bytes[ (int) byteIndex ];
	}

	public bool this[ uint index ] {
		get => Get( index );
		set => Set( index, value );
	}

	public void SetRange( ReadOnlySpan<byte> bytes, uint byteOffset = 0 ) {
		int bytesToSet = _bytes.Count - ((int) byteOffset + 1);
		int bytesToAdd = (int) byteOffset + bytes.Length - _bytes.Count;
		for (int i = 0; i < bytesToSet; i++) {
			int index = i + (int) byteOffset;
			_bytes[ index ] = bytes[ i ];
		}
		if (bytesToAdd > 0)
			_bytes.AddRange( bytes[ ^bytesToAdd.. ] );
	}

	public int ByteCount => _bytes.Count;
	public int Count => _bytes.Count * 8;

}

public sealed class ReadOnlyBitSet {
	private readonly BitSet _bitSet;
	public ReadOnlyBitSet( BitSet bitSet ) {
		_bitSet = bitSet;
	}
	public bool this[ uint index ] => _bitSet[ index ];
	public bool Get( uint index ) => _bitSet.Get( index );
	public byte GetByte( uint byteIndex ) => _bitSet.GetByte( byteIndex );
	public int ByteCount => _bitSet.ByteCount;
	public int Count => _bitSet.Count;
}