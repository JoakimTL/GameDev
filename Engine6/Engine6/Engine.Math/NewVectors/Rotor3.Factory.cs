using System.Numerics;

namespace Engine.Math.NewVectors;

public static class Rotor3 {
	/// <summary>
	/// Creates a rotor from an axis and an angle.
	/// </summary>
	/// <exception cref="ArgumentException">If <paramref name="axis"/> can't be normalized</exception>
	public static Rotor3<TScalar> FromAxisAngle<TScalar>( in Vector3<TScalar> axis, TScalar angle ) where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {
		if (!axis.TryNormalize<Vector3<TScalar>, TScalar>( out Vector3<TScalar> axisNormalized, out _ ))
			throw new ArgumentException( $"{nameof( axis )} can't be normalized." );
		angle /= TScalar.One + TScalar.One;
		return new( TScalar.Cos( angle ), -new Trivector3<TScalar>( TScalar.Sin( angle ) ).Multiply( axisNormalized ) );
	}

	/// <summary>
	/// Creates a rotor from an axis and an angle.
	/// </summary>
	/// <returns>True if the rotor could be created. False if <paramref name="axis"/> can't be normalized</returns>
	public static bool TryFromAxisAngle<TScalar>( in Vector3<TScalar> axis, TScalar angle, out Rotor3<TScalar> result ) where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {
		if (!axis.TryNormalize<Vector3<TScalar>, TScalar>( out Vector3<TScalar> axisNormalized, out _ )) {
			result = Rotor3<TScalar>.MultiplicativeIdentity;
			return false;
		}
		angle /= TScalar.One + TScalar.One;
		result = new( TScalar.Cos( angle ), -new Trivector3<TScalar>( TScalar.Sin( angle ) ).Multiply( axisNormalized ) );
		return true;
	}

	/// <summary>
	/// Creates a rotor. This rotor rotates vectors along the plane formed by <paramref name="from"/> and <paramref name="to"/> by the angle separating them on the plane.
	/// </summary>
	/// <exception cref="ArgumentException">If <paramref name="from"/> or <paramref name="to"/> vectors can't be normalized, or if the vectors are parallel</exception>
	public static Rotor3<TScalar> FromVectors<TScalar>( in Vector3<TScalar> from, in Vector3<TScalar> to ) where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {
		if (!from.TryNormalize<Vector3<TScalar>, TScalar>( out Vector3<TScalar> fromNormalized, out _ ) || !to.TryNormalize<Vector3<TScalar>, TScalar>( out Vector3<TScalar> toNormalized, out _ ))
			throw new ArgumentException( $"{nameof( from )} or {nameof( to )} vectors can't be normalized." );
		if (fromNormalized.Dot( toNormalized ) == -TScalar.One)
			throw new ArgumentException( $"{nameof( from )} and {nameof( to )} vectors are parallel. Unable to form a plane of rotation." );
		return FromNormalizedVectors( fromNormalized, toNormalized );
	}

	/// <summary>
	/// Attempts to create a rotor. This rotor rotates vectors along the plane formed by <paramref name="from"/> and <paramref name="to"/> by the angle separating them on the plane.
	/// </summary>
	/// <returns>True if the rotor could be created. False if any of the input vectors can't be normalized, or if the vectors are parallel and unable to form a plane of rotation</returns>
	public static bool TryFromVectors<TScalar>( in Vector3<TScalar> from, in Vector3<TScalar> to, out Rotor3<TScalar> result ) where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {
		if (!from.TryNormalize<Vector3<TScalar>, TScalar>( out Vector3<TScalar> fromNormalized, out _ ) || !to.TryNormalize<Vector3<TScalar>, TScalar>( out Vector3<TScalar> toNormalized, out _ )) {
			result = Rotor3<TScalar>.MultiplicativeIdentity;
			return false;
		}
		if (fromNormalized.Dot( toNormalized ) == -TScalar.One) {
			result = Rotor3<TScalar>.MultiplicativeIdentity;
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
	public static Rotor3<TScalar> FromNormalizedVectors<TScalar>( in Vector3<TScalar> from, in Vector3<TScalar> to ) where TScalar : unmanaged, IFloatingPointIeee754<TScalar>
		=> (to.Multiply( from ) + new Rotor3<TScalar>( TScalar.One, Bivector3<TScalar>.Zero )) / TScalar.Sqrt( (TScalar.One + TScalar.One) * (TScalar.One + from.Dot( to )) );
}
