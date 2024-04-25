using System.Numerics;

namespace Engine.Math.NewVectors;

public static class Rotor2 {
	/// <summary>
	/// Creates a rotor from an axis and an angle.
	/// </summary>
	/// <exception cref="ArgumentException">If <paramref name="axis"/> can't be normalized</exception>
	public static Rotor2<TScalar> FromAngle<TScalar>( TScalar angle ) where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {
		angle /= TScalar.One + TScalar.One;
		return new( TScalar.Cos( angle ), -new Bivector2<TScalar>( TScalar.Sin( angle ) ) );
	}

	/// <summary>
	/// Creates a rotor. This rotor rotates vectors along the plane formed by <paramref name="from"/> and <paramref name="to"/> by the angle separating them on the plane.
	/// </summary>
	/// <exception cref="ArgumentException">If <paramref name="from"/> or <paramref name="to"/> vectors can't be normalized</exception>
	public static Rotor2<TScalar> FromVectors<TScalar>( in Vector2<TScalar> from, in Vector2<TScalar> to ) where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {
		if (!from.TryNormalize<Vector2<TScalar>, TScalar>( out Vector2<TScalar> fromNormalized, out _ ) || !to.TryNormalize<Vector2<TScalar>, TScalar>( out Vector2<TScalar> toNormalized, out _ ))
			throw new ArgumentException( $"{nameof( from )} or {nameof( to )} vectors can't be normalized." );
		return FromNormalizedVectors( fromNormalized, toNormalized );
	}

	/// <summary>
	/// Attempts to create a rotor. This rotor rotates vectors along the plane formed by <paramref name="from"/> and <paramref name="to"/> by the angle separating them on the plane.
	/// </summary>
	/// <returns>True if the rotor could be created. False if any of the input vectors can't be normalized</returns>
	public static bool TryFromVectors<TScalar>( in Vector2<TScalar> from, in Vector2<TScalar> to, out Rotor2<TScalar> result ) where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {
		if (!from.TryNormalize<Vector2<TScalar>, TScalar>( out Vector2<TScalar> fromNormalized, out _ ) || !to.TryNormalize<Vector2<TScalar>, TScalar>( out Vector2<TScalar> toNormalized, out _ )) {
			result = Rotor2<TScalar>.MultiplicativeIdentity;
			return false;
		}
		result = FromNormalizedVectors( fromNormalized, toNormalized );
		return true;
	}

	/// <summary>
	/// Creates a rotor. This rotor rotates vectors along the plane formed by <paramref name="from"/> and <paramref name="to"/> by the angle separating them on the plane.<br/>
	/// <b>Assumes the vectors are normalized.</b><br/><br/>
	/// Returns NaN if the vectors can't form a plane of rotation.
	/// </summary>
	public static Rotor2<TScalar> FromNormalizedVectors<TScalar>( in Vector2<TScalar> from, in Vector2<TScalar> to ) where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {
		//We always know the plane of rotation, it's the XY plane. We can then calculate the angle between the vectors and create the rotor from that angle.
		TScalar dot = from.Dot( to );
		TScalar wedge = TScalar.CreateChecked( TScalar.Sign( from.Wedge( to ).XY ) );
		if (TScalar.IsZero( wedge ))
			wedge = TScalar.One;
		return FromAngle( TScalar.Acos( dot ) * wedge );
	}
}