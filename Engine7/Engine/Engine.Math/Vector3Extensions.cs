using System.Numerics;

namespace Engine;

public static class Vector3Extensions {
	public static Vector3<TScalar> Cross<TScalar>( this Vector3<TScalar> l, in Vector3<TScalar> r )
		where TScalar :
			unmanaged, INumber<TScalar> {
		TScalar x = (l.Y * r.Z) - (l.Z * r.Y);
		TScalar y = (l.Z * r.X) - (l.X * r.Z);
		TScalar z = (l.X * r.Y) - (l.Y * r.X);
		return new( x, y, z ); //l.Wedge( r ) * -Trivector3<TScalar>.One;
	}

	public static Vector2<TScalar> ToNormalizedPolar<TScalar>( in this Vector3<TScalar> vector )
		where TScalar :
			unmanaged, IFloatingPointIeee754<TScalar> {
		Vector3<TScalar> vNormalized = vector.Normalize<Vector3<TScalar>, TScalar>();
		TScalar theta = TScalar.Atan2( vNormalized.Z, vNormalized.X );
		TScalar phi = TScalar.Acos( vNormalized.Y );
		return new( theta, phi );
	}

	/// <summary>
	/// Finds the approximate magnitude of a vector using a method that is faster than the standard Euclidean norm. Expect around 5% error in the result.
	/// </summary>
	public static float ApproximateMagnitude( in this Vector3<float> vector ) {
		float a = vector.X.FastAbs(), b = vector.Y.FastAbs(), c = vector.Z.FastAbs();

		float t;
		if (a < b) { t = a; a = b; b = t; }
		if (b < c) { t = b; b = c; c = t; }
		if (a < b) { t = a; a = b; b = t; }

		return 0.96043387f * a + 0.39782473f * b + 0.28714703f * c;
	}

	public static Vector3 ToNumerics<TScalar>( in this Vector3<TScalar> vector )
		where TScalar :
			unmanaged, INumber<TScalar>
		=> new( float.CreateSaturating( vector.X ), float.CreateSaturating( vector.Y ), float.CreateSaturating( vector.Z ) );

	public static Vector3 ToNumerics( in this Vector3<double> vector )
		=> new( (float) vector.X, (float) vector.Y, (float) vector.Z );

	public static Vector3<TScalar> FromNumerics<TScalar>( in this Vector3 vector )
		where TScalar :
			unmanaged, INumber<TScalar>
		=> new( TScalar.CreateSaturating( vector.X ), TScalar.CreateSaturating( vector.Y ), TScalar.CreateSaturating( vector.Z ) );
}