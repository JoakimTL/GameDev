namespace Engine.Modules.Networking;

public class Class1 {

}

public sealed class InputPacket {

	public int InputCode { get; set; }
	public InputData InputValue { get; set; }

}
public unsafe struct InputData {
	private fixed byte _data[ 8 ];

	public InputData( ulong value ) {
		fixed (byte* ptr = _data)
			*(ulong*) ptr = value;
	}

	public InputData( double value ) {
		fixed (byte* ptr = _data)
			*(double*) ptr = value;
	}

	public InputData( bool value ) {
		fixed (byte* ptr = _data)
			*(bool*) ptr = value;
	}

	public InputData( ReadOnlySpan<byte> value ) {
		if (value.Length != 8)
			throw new ArgumentException( "InputData must be 8 bytes long" );

		fixed (byte* ptr = _data)
			for (int i = 0; i < 8; i++)
				ptr[ i ] = value[ i ];
	}

	public readonly ulong AsLong() {
		fixed (byte* ptr = _data)
			return *(ulong*) ptr;
	}

	public readonly double AsDouble() {
		fixed (byte* ptr = _data)
			return *(double*) ptr;
	}

	public readonly bool AsBool() {
		fixed (byte* ptr = _data)
			return *(bool*) ptr;
	}
}