using System.Numerics;

namespace Engine;

public static class Vector4Extensions {
	public static Vector4 ToNumerics( in this Vector4<float> vector )
		=> new( vector.X, vector.Y, vector.Z, vector.W );

	public static Vector4 ToNumerics( in this Vector4<double> vector )
		=> new( (float) vector.X, (float) vector.Y, (float) vector.Z, (float) vector.W );

	public static Vector4<TScalar> FromNumerics<TScalar>( in this Vector4 vector )
		where TScalar :
			unmanaged, INumber<TScalar>
		=> new( TScalar.CreateSaturating( vector.X ), TScalar.CreateSaturating( vector.Y ), TScalar.CreateSaturating( vector.Z ), TScalar.CreateSaturating( vector.W ) );
}