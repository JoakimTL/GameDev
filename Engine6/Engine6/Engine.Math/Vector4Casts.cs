namespace Engine.Math;

/// <summary>
/// Casting methods for <see cref="Vector4{T}"/>. Return types may vary.
/// </summary>
public static class Vector4Casts {
	public static Vector4<byte> ToBytes( Vector4<double> l ) => new( (byte) l.X, (byte) l.Y, (byte) l.Z, (byte) l.W );
	public static Vector4<byte> ToBytes( Vector4<float> l ) => new( (byte) l.X, (byte) l.Y, (byte) l.Z, (byte) l.W );
	public static Vector4<byte> ToBytes( Vector4<int> l ) => new( (byte) l.X, (byte) l.Y, (byte) l.Z, (byte) l.W );
	public static Vector4<byte> ToBytes( Vector4<long> l ) => new( (byte) l.X, (byte) l.Y, (byte) l.Z, (byte) l.W );
	public static Vector4<byte> ToBytes( Vector4<uint> l ) => new( (byte) l.X, (byte) l.Y, (byte) l.Z, (byte) l.W );
	public static Vector4<byte> ToBytes( Vector4<ulong> l ) => new( (byte) l.X, (byte) l.Y, (byte) l.Z, (byte) l.W );
	public static Vector4<byte> FromNormalizedTo8Bits( Vector4<double> l, double min = 0, double max = 1 ) {
		double space = max - min;
		if (space == 0)
			throw new ArgumentException( $"{nameof( min )} - {nameof( max )} is equal to 0, meaning the space can't be normalized." );
		double spaceInverse = 1d / (max - min);
		return new(
			(byte) System.Math.Round( (l.X - min) * spaceInverse * byte.MaxValue ),
			(byte) System.Math.Round( (l.Y - min) * spaceInverse * byte.MaxValue ),
			(byte) System.Math.Round( (l.Z - min) * spaceInverse * byte.MaxValue ),
			(byte) System.Math.Round( (l.W - min) * spaceInverse * byte.MaxValue ) );
	}

	public static Vector4<byte> FromNormalizedTo8Bits( Vector4<float> l, float min = 0, float max = 1 ) {
		float space = max - min;
		if (space == 0)
			throw new ArgumentException( $"{nameof( min )} - {nameof( max )} is equal to 0, meaning the space can't be normalized." );
		float spaceInverse = 1f / (max - min);
		return new(
			(byte) MathF.Round( (l.X - min) * spaceInverse * byte.MaxValue ),
			(byte) MathF.Round( (l.Y - min) * spaceInverse * byte.MaxValue ),
			(byte) MathF.Round( (l.Z - min) * spaceInverse * byte.MaxValue ),
			(byte) MathF.Round( (l.W - min) * spaceInverse * byte.MaxValue ) );
	}
	public unsafe static Vector4<byte> FromXYZWToVector( int l ) => new( *((byte*) &l), *((byte*) &l + 1), *((byte*) &l + 2), *((byte*) &l + 3) );
	public unsafe static Vector4<byte> FromXYZWToVector( uint l ) => new( *((byte*) &l), *((byte*) &l + 1), *((byte*) &l + 2), *((byte*) &l + 3) );
	public unsafe static Vector4<byte> FromWZYXToVector( int l ) => new( *((byte*) &l + 3), *((byte*) &l + 2), *((byte*) &l + 1), *((byte*) &l + 0) );
	public unsafe static Vector4<byte> FromWZYXToVector( uint l ) => new( *((byte*) &l + 3), *((byte*) &l + 2), *((byte*) &l + 1), *((byte*) &l + 0) );

}