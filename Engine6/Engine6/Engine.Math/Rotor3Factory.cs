using System.Numerics;
using Engine.Math.Operations;

namespace Engine.Math;

public static class Rotor3Factory {
	/// <summary>
	/// Creates a rotor from an axis and an angle.
	/// </summary>
	/// <exception cref="ArgumentException">If <paramref name="axis"/> can't be normalized</exception>
	public static Rotor3<T> FromAxisAngle<T>( in Vector3<T> axis, T angle ) where T : unmanaged, IFloatingPointIeee754<T> {
		if (!axis.TryNormalize( out Vector3<T> axisNormalized ))
			throw new ArgumentException( $"{nameof( axis )} can't be normalized." );
		angle /= T.One + T.One;
		return new( T.Cos( angle ), -new Trivector3<T>( T.Sin( angle ) ) * axisNormalized );
	}

	/// <summary>
	/// Creates a rotor from an axis and an angle.
	/// </summary>
	/// <returns>True if the rotor could be created. False if <paramref name="axis"/> can't be normalized</returns>
	public static bool TryFromAxisAngle<T>( in Vector3<T> axis, T angle, out Rotor3<T> result ) where T : unmanaged, IFloatingPointIeee754<T> {
		if (!axis.TryNormalize( out Vector3<T> axisNormalized )) {
			result = Rotor3<T>.MultiplicativeIdentity;
			return false;
		}
		angle /= T.One + T.One;
		result = new( T.Cos( angle ), -new Trivector3<T>( T.Sin( angle ) ) * axisNormalized );
		return true;
	}

	/// <summary>
	/// Creates a rotor. This rotor rotates vectors along the plane formed by <paramref name="from"/> and <paramref name="to"/> by the angle separating them on the plane.
	/// </summary>
	/// <exception cref="ArgumentException">If <paramref name="from"/> or <paramref name="to"/> vectors can't be normalized, or if the vectors are parallel</exception>
	public static Rotor3<T> FromVectors<T>( in Vector3<T> from, in Vector3<T> to ) where T : unmanaged, IFloatingPointIeee754<T> {
		if (!from.TryNormalize( out Vector3<T> fromNormalized ) || !to.TryNormalize( out Vector3<T> toNormalized ))
			throw new ArgumentException( $"{nameof( from )} or {nameof( to )} vectors can't be normalized." );
		if (fromNormalized.Dot(toNormalized) == -T.One)
			throw new ArgumentException( $"{nameof( from )} and {nameof( to )} vectors are parallel. Unable to form a plane of rotation." );
		return FromNormalizedVectors( fromNormalized, toNormalized );
	}

	/// <summary>
	/// Attempts to create a rotor. This rotor rotates vectors along the plane formed by <paramref name="from"/> and <paramref name="to"/> by the angle separating them on the plane.
	/// </summary>
	/// <returns>True if the rotor could be created. False if any of the input vectors can't be normalized, or if the vectors are parallel and unable to form a plane of rotation</returns>
	public static bool TryFromVectors<T>( in Vector3<T> from, in Vector3<T> to, out Rotor3<T> result ) where T : unmanaged, IFloatingPointIeee754<T> {
		if (!from.TryNormalize( out Vector3<T> fromNormalized ) || !to.TryNormalize( out Vector3<T> toNormalized )) {
			result = Rotor3<T>.MultiplicativeIdentity;
			return false;
		}
		if (fromNormalized.Dot(toNormalized) == -T.One) {
			result = Rotor3<T>.MultiplicativeIdentity;
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
	public static Rotor3<T> FromNormalizedVectors<T>( in Vector3<T> from, in Vector3<T> to ) where T : unmanaged, IFloatingPointIeee754<T>
		=> (to.Multiply( from ) + new Rotor3<T>( T.One, Bivector3<T>.Zero )) / T.Sqrt( (T.One + T.One) * (T.One + from.Dot( to )) );
}