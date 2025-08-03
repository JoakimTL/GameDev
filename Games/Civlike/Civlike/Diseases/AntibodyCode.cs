using System.Numerics;
using System.Runtime.InteropServices;

namespace Civlike.Diseases;

[StructLayout( LayoutKind.Sequential )]
public readonly struct AntibodyCode {
	private readonly long _value1;
	private readonly long _value2;

	private AntibodyCode( long value1, long value2 ) {
		this._value1 = value1;
		this._value2 = value2;
	}

	public static AntibodyCode Create( Random random ) => new( random.NextInt64(), random.NextInt64() );

	public AntibodyCode Mutate( Random random, int mutations ) {
		AntibodyCode antibodyCode = this;
		unsafe {
			byte* p = (byte*) &antibodyCode;
			for (int i = 0; i < mutations; i++) {
				int bit = random.Next( sizeof( AntibodyCode ) * 8 );
				byte* byteAddress = p + bit / 8;
				byte currentByteValue = *byteAddress;
				*byteAddress = (byte)(currentByteValue ^ (byte)(1 << (bit % 8)));
			}
		}
		return antibodyCode;
	}

	public int HammingDistance( in AntibodyCode other ) {
		// XOR shows which bits differ; popcount counts the 1-bits.
		ulong diff1 = (ulong) (this._value1 ^ other._value1);
		ulong diff2 = (ulong) (this._value2 ^ other._value2);

		return BitOperations.PopCount( diff1 ) + BitOperations.PopCount( diff2 );
	}

	public float CrossImmunity( in AntibodyCode other, float alpha = 0.08f ) {
		int d = HammingDistance( other );
		return MathF.Exp( -alpha * d );
	}
}
