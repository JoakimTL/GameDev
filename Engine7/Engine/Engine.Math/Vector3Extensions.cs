using System.Numerics;

namespace Engine;

public static class Vector3Extensions {
	public static Vector3<TScalar> Cross<TScalar>( this Vector3<TScalar> l, in Vector3<TScalar> r )
		where TScalar :
			unmanaged, INumber<TScalar>
		=> l.Wedge( r ) * -Trivector3<TScalar>.One;

	public static Vector2<TScalar> ToNormalizedPolar<TScalar>( in this Vector3<TScalar> vector )
		where TScalar :
			unmanaged, IFloatingPointIeee754<TScalar> {
		Vector3<TScalar> vNormalized = vector.Normalize<Vector3<TScalar>, TScalar>();
		TScalar theta = TScalar.Atan2( vNormalized.Z, vNormalized.X );
		TScalar phi = TScalar.Asin( vNormalized.Y );
		return new( theta, phi );
	}
}