using System.Numerics;

namespace Engine;
public static class GeometricAlgebraMath3 {

	/*
	 * 0 - s - scalar
	 * 1 - v - vector3
	 * 2 - b - bivector3
	 * 3 - t - trivector3
	 * 4 - r - rotor3
	 * 5 - m - multivector3
	 * _________________________________
	 * | * |   | s | v | b | t | r | m |
	 * |_______________________________|
	 * | s |   | s | v | b | t | r | m |
	 * | v |   |   | r | m | b | m | m |
	 * | b |   |   |   | r | v | r | m |
	 * | t |   |   |   |   | s | m | m |
	 * | r |   |   |   |   |   | r | m |
	 * | m |   |   |   |   |   |   | m |
	 * _________________________________
	 * 
	 * _________________________________
	 * | *     | 0 | 1 | 2 | 3 | 4 | 5 |
	 * |       ________________________|
	 * | 0 |   | 0 | 1 | 2 | 3 | 4 | 5 |
	 * | 1 |   | 1 | 4 | 5 | 2 | 5 | 5 |
	 * | 2 |   | 2 | 5 | 4 | 1 | 4 | 5 |
	 * | 3 |   | 3 | 2 | 1 | 0 | 5 | 5 |
	 * | 4 |   | 4 | 5 | 4 | 5 | 4 | 5 |
	 * | 5 |   | 5 | 5 | 5 | 5 | 5 | 5 |
	 * _________________________________
	 */

	public static Rotor3<TScalar> Multiply<TScalar>( in Vector3<TScalar> l, in Vector3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			l.X * r.X + l.Y * r.Y + l.Z * r.Z,
			l.Y * r.Z - l.Z * r.Y,
			l.Z * r.X - l.X * r.Z,
			l.X * r.Y - l.Y * r.X
		);

	public static Multivector3<TScalar> Multiply<TScalar>( in Vector3<TScalar> l, in Bivector3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			TScalar.AdditiveIdentity,
			l.Z * r.ZX - l.Y * r.XY,
			l.X * r.XY - l.Z * r.YZ,
			l.Y * r.YZ - l.X * r.ZX,
			TScalar.AdditiveIdentity,
			TScalar.AdditiveIdentity,
			TScalar.AdditiveIdentity,
			l.X * r.YZ + l.Y * r.ZX + l.Z * r.XY
		);

	public static Bivector3<TScalar> Multiply<TScalar>( in Vector3<TScalar> l, in Trivector3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			l.X * r.XYZ,
			l.Y * r.XYZ,
			l.Z * r.XYZ
		);

	public static Multivector3<TScalar> Multiply<TScalar>( in Vector3<TScalar> l, in Rotor3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			TScalar.AdditiveIdentity,
			l.Z * r.Bivector.ZX + l.X * r.Scalar - l.Y * r.Bivector.XY,
			l.X * r.Bivector.XY + l.Y * r.Scalar - l.Z * r.Bivector.YZ,
			l.Y * r.Bivector.YZ + l.Z * r.Scalar - l.X * r.Bivector.ZX,
			TScalar.AdditiveIdentity,
			TScalar.AdditiveIdentity,
			TScalar.AdditiveIdentity,
			l.X * r.Bivector.YZ + l.Y * r.Bivector.ZX + l.Z * r.Bivector.XY
		);

	public static Multivector3<TScalar> Multiply<TScalar>( in Vector3<TScalar> l, in Multivector3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			l.X * r.Vector.X + l.Y * r.Vector.Y + l.Z * r.Vector.Z,
			l.Z * r.Bivector.ZX + l.X * r.Scalar - l.Y * r.Bivector.XY,
			l.X * r.Bivector.XY + l.Y * r.Scalar - l.Z * r.Bivector.YZ,
			l.Y * r.Bivector.YZ + l.Z * r.Scalar - l.X * r.Bivector.ZX,
			l.Y * r.Vector.Z + l.X * r.Trivector.XYZ - l.Z * r.Vector.Y,
			l.Z * r.Vector.X + l.Y * r.Trivector.XYZ - l.X * r.Vector.Z,
			l.X * r.Vector.Y + l.Z * r.Trivector.XYZ - l.Y * r.Vector.X,
			l.X * r.Bivector.YZ + l.Y * r.Bivector.ZX + l.Z * r.Bivector.XY
		);

	public static Multivector3<TScalar> Multiply<TScalar>( in Bivector3<TScalar> l, in Vector3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			TScalar.AdditiveIdentity,
			l.XY * r.Y - l.ZX * r.Z,
			l.YZ * r.Z - l.XY * r.X,
			l.ZX * r.X - l.YZ * r.Y,
			TScalar.AdditiveIdentity,
			TScalar.AdditiveIdentity,
			TScalar.AdditiveIdentity,
			l.XY * r.Z + l.YZ * r.X + l.ZX * r.Y
		);

	public static Rotor3<TScalar> Multiply<TScalar>( in Bivector3<TScalar> l, in Bivector3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			-(l.XY * r.XY) - l.YZ * r.YZ - l.ZX * r.ZX,
			l.XY * r.ZX - l.ZX * r.XY,
			l.YZ * r.XY - l.XY * r.YZ,
			l.ZX * r.YZ - l.YZ * r.ZX
		);

	public static Vector3<TScalar> Multiply<TScalar>( in Bivector3<TScalar> l, in Trivector3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			-(l.YZ * r.XYZ),
			-(l.ZX * r.XYZ),
			-(l.XY * r.XYZ)
		);

	public static Rotor3<TScalar> Multiply<TScalar>( in Bivector3<TScalar> l, in Rotor3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			-(l.XY * r.Bivector.XY) - l.YZ * r.Bivector.YZ - l.ZX * r.Bivector.ZX,
			l.XY * r.Bivector.ZX + l.YZ * r.Scalar - l.ZX * r.Bivector.XY,
			l.YZ * r.Bivector.XY + l.ZX * r.Scalar - l.XY * r.Bivector.YZ,
			l.ZX * r.Bivector.YZ + l.XY * r.Scalar - l.YZ * r.Bivector.ZX
		);

	public static Multivector3<TScalar> Multiply<TScalar>( in Bivector3<TScalar> l, in Multivector3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			-(l.XY * r.Bivector.XY) - l.YZ * r.Bivector.YZ - l.ZX * r.Bivector.ZX,
			l.XY * r.Vector.Y - l.YZ * r.Trivector.XYZ - l.ZX * r.Vector.Z,
			l.YZ * r.Vector.Z - l.ZX * r.Trivector.XYZ - l.XY * r.Vector.X,
			l.ZX * r.Vector.X - l.XY * r.Trivector.XYZ - l.YZ * r.Vector.Y,
			l.XY * r.Bivector.ZX + l.YZ * r.Scalar - l.ZX * r.Bivector.XY,
			l.YZ * r.Bivector.XY + l.ZX * r.Scalar - l.XY * r.Bivector.YZ,
			l.ZX * r.Bivector.YZ + l.XY * r.Scalar - l.YZ * r.Bivector.ZX,
			l.XY * r.Vector.Z + l.YZ * r.Vector.X + l.ZX * r.Vector.Y
		);

	public static Bivector3<TScalar> Multiply<TScalar>( in Trivector3<TScalar> l, in Vector3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			l.XYZ * r.X,
			l.XYZ * r.Y,
			l.XYZ * r.Z
		);

	public static Vector3<TScalar> Multiply<TScalar>( in Trivector3<TScalar> l, in Bivector3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			-(l.XYZ * r.YZ),
			-(l.XYZ * r.ZX),
			-(l.XYZ * r.XY)
		);

	public static TScalar Multiply<TScalar>( in Trivector3<TScalar> l, in Trivector3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> -(l.XYZ * r.XYZ);

	public static Multivector3<TScalar> Multiply<TScalar>( in Trivector3<TScalar> l, in Rotor3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			TScalar.AdditiveIdentity,
			-(l.XYZ * r.Bivector.YZ),
			-(l.XYZ * r.Bivector.ZX),
			-(l.XYZ * r.Bivector.XY),
			TScalar.AdditiveIdentity,
			TScalar.AdditiveIdentity,
			TScalar.AdditiveIdentity,
			l.XYZ * r.Scalar
		);

	public static Multivector3<TScalar> Multiply<TScalar>( in Trivector3<TScalar> l, in Multivector3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			-(l.XYZ * r.Trivector.XYZ),
			-(l.XYZ * r.Bivector.YZ),
			-(l.XYZ * r.Bivector.ZX),
			-(l.XYZ * r.Bivector.XY),
			l.XYZ * r.Vector.X,
			l.XYZ * r.Vector.Y,
			l.XYZ * r.Vector.Z,
			l.XYZ * r.Scalar
		);

	public static Multivector3<TScalar> Multiply<TScalar>( in Rotor3<TScalar> l, in Vector3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			TScalar.AdditiveIdentity,
			l.Scalar * r.X + l.Bivector.XY * r.Y - l.Bivector.ZX * r.Z,
			l.Scalar * r.Y + l.Bivector.YZ * r.Z - l.Bivector.XY * r.X,
			l.Scalar * r.Z + l.Bivector.ZX * r.X - l.Bivector.YZ * r.Y,
			TScalar.AdditiveIdentity,
			TScalar.AdditiveIdentity,
			TScalar.AdditiveIdentity,
			l.Bivector.XY * r.Z + l.Bivector.YZ * r.X + l.Bivector.ZX * r.Y
		);

	public static Rotor3<TScalar> Multiply<TScalar>( in Rotor3<TScalar> l, in Bivector3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			-(l.Bivector.XY * r.XY) - l.Bivector.YZ * r.YZ - l.Bivector.ZX * r.ZX,
			l.Scalar * r.YZ + l.Bivector.XY * r.ZX - l.Bivector.ZX * r.XY,
			l.Scalar * r.ZX + l.Bivector.YZ * r.XY - l.Bivector.XY * r.YZ,
			l.Scalar * r.XY + l.Bivector.ZX * r.YZ - l.Bivector.YZ * r.ZX
		);

	public static Multivector3<TScalar> Multiply<TScalar>( in Rotor3<TScalar> l, in Trivector3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			TScalar.AdditiveIdentity,
			-(l.Bivector.YZ * r.XYZ),
			-(l.Bivector.ZX * r.XYZ),
			-(l.Bivector.XY * r.XYZ),
			TScalar.AdditiveIdentity,
			TScalar.AdditiveIdentity,
			TScalar.AdditiveIdentity,
			l.Scalar * r.XYZ
		);

	public static Rotor3<TScalar> Multiply<TScalar>( in Rotor3<TScalar> l, in Rotor3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			l.Scalar * r.Scalar - l.Bivector.XY * r.Bivector.XY - l.Bivector.YZ * r.Bivector.YZ - l.Bivector.ZX * r.Bivector.ZX,
			l.Scalar * r.Bivector.YZ + l.Bivector.XY * r.Bivector.ZX + l.Bivector.YZ * r.Scalar - l.Bivector.ZX * r.Bivector.XY,
			l.Scalar * r.Bivector.ZX + l.Bivector.YZ * r.Bivector.XY + l.Bivector.ZX * r.Scalar - l.Bivector.XY * r.Bivector.YZ,
			l.Scalar * r.Bivector.XY + l.Bivector.ZX * r.Bivector.YZ + l.Bivector.XY * r.Scalar - l.Bivector.YZ * r.Bivector.ZX
		);

	public static Multivector3<TScalar> Multiply<TScalar>( in Rotor3<TScalar> l, in Multivector3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			l.Scalar * r.Scalar - l.Bivector.XY * r.Bivector.XY - l.Bivector.YZ * r.Bivector.YZ - l.Bivector.ZX * r.Bivector.ZX,
			l.Scalar * r.Vector.X + l.Bivector.XY * r.Vector.Y - l.Bivector.YZ * r.Trivector.XYZ - l.Bivector.ZX * r.Vector.Z,
			l.Scalar * r.Vector.Y + l.Bivector.YZ * r.Vector.Z - l.Bivector.ZX * r.Trivector.XYZ - l.Bivector.XY * r.Vector.X,
			l.Scalar * r.Vector.Z + l.Bivector.ZX * r.Vector.X - l.Bivector.XY * r.Trivector.XYZ - l.Bivector.YZ * r.Vector.Y,
			l.Scalar * r.Bivector.YZ + l.Bivector.XY * r.Bivector.ZX + l.Bivector.YZ * r.Scalar - l.Bivector.ZX * r.Bivector.XY,
			l.Scalar * r.Bivector.ZX + l.Bivector.YZ * r.Bivector.XY + l.Bivector.ZX * r.Scalar - l.Bivector.XY * r.Bivector.YZ,
			l.Scalar * r.Bivector.XY + l.Bivector.ZX * r.Bivector.YZ + l.Bivector.XY * r.Scalar - l.Bivector.YZ * r.Bivector.ZX,
			l.Scalar * r.Trivector.XYZ + l.Bivector.XY * r.Vector.Z + l.Bivector.YZ * r.Vector.X + l.Bivector.ZX * r.Vector.Y
		);

	public static Multivector3<TScalar> Multiply<TScalar>( in Multivector3<TScalar> l, in Vector3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			l.Vector.X * r.X + l.Vector.Y * r.Y + l.Vector.Z * r.Z,
			l.Scalar * r.X + l.Bivector.XY * r.Y - l.Bivector.ZX * r.Z,
			l.Scalar * r.Y + l.Bivector.YZ * r.Z - l.Bivector.XY * r.X,
			l.Scalar * r.Z + l.Bivector.ZX * r.X - l.Bivector.YZ * r.Y,
			l.Vector.Y * r.Z + l.Trivector.XYZ * r.X - l.Vector.Z * r.Y,
			l.Vector.Z * r.X + l.Trivector.XYZ * r.Y - l.Vector.X * r.Z,
			l.Vector.X * r.Y + l.Trivector.XYZ * r.Z - l.Vector.Y * r.X,
			l.Bivector.XY * r.Z + l.Bivector.YZ * r.X + l.Bivector.ZX * r.Y
		);

	public static Multivector3<TScalar> Multiply<TScalar>( in Multivector3<TScalar> l, in Bivector3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			-(l.Bivector.XY * r.XY) - l.Bivector.YZ * r.YZ - l.Bivector.ZX * r.ZX,
			l.Vector.Z * r.ZX - l.Vector.Y * r.XY - l.Trivector.XYZ * r.YZ,
			l.Vector.X * r.XY - l.Vector.Z * r.YZ - l.Trivector.XYZ * r.ZX,
			l.Vector.Y * r.YZ - l.Vector.X * r.ZX - l.Trivector.XYZ * r.XY,
			l.Scalar * r.YZ + l.Bivector.XY * r.ZX - l.Bivector.ZX * r.XY,
			l.Scalar * r.ZX + l.Bivector.YZ * r.XY - l.Bivector.XY * r.YZ,
			l.Scalar * r.XY + l.Bivector.ZX * r.YZ - l.Bivector.YZ * r.ZX,
			l.Vector.X * r.YZ + l.Vector.Y * r.ZX + l.Vector.Z * r.XY
		);

	public static Multivector3<TScalar> Multiply<TScalar>( in Multivector3<TScalar> l, in Trivector3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			-(l.Trivector.XYZ * r.XYZ),
			-(l.Bivector.YZ * r.XYZ),
			-(l.Bivector.ZX * r.XYZ),
			-(l.Bivector.XY * r.XYZ),
			l.Vector.X * r.XYZ,
			l.Vector.Y * r.XYZ,
			l.Vector.Z * r.XYZ,
			l.Scalar * r.XYZ
		);

	public static Multivector3<TScalar> Multiply<TScalar>( in Multivector3<TScalar> l, in Rotor3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			l.Scalar * r.Scalar - l.Bivector.XY * r.Bivector.XY - l.Bivector.YZ * r.Bivector.YZ - l.Bivector.ZX * r.Bivector.ZX,
			l.Vector.Z * r.Bivector.ZX + l.Vector.X * r.Scalar - l.Vector.Y * r.Bivector.XY - l.Trivector.XYZ * r.Bivector.YZ,
			l.Vector.X * r.Bivector.XY + l.Vector.Y * r.Scalar - l.Vector.Z * r.Bivector.YZ - l.Trivector.XYZ * r.Bivector.ZX,
			l.Vector.Y * r.Bivector.YZ + l.Vector.Z * r.Scalar - l.Vector.X * r.Bivector.ZX - l.Trivector.XYZ * r.Bivector.XY,
			l.Scalar * r.Bivector.YZ + l.Bivector.XY * r.Bivector.ZX + l.Bivector.YZ * r.Scalar - l.Bivector.ZX * r.Bivector.XY,
			l.Scalar * r.Bivector.ZX + l.Bivector.YZ * r.Bivector.XY + l.Bivector.ZX * r.Scalar - l.Bivector.XY * r.Bivector.YZ,
			l.Scalar * r.Bivector.XY + l.Bivector.ZX * r.Bivector.YZ + l.Bivector.XY * r.Scalar - l.Bivector.YZ * r.Bivector.ZX,
			l.Vector.X * r.Bivector.YZ + l.Vector.Y * r.Bivector.ZX + l.Vector.Z * r.Bivector.XY + l.Trivector.XYZ * r.Scalar
		);

	public static Multivector3<TScalar> Multiply<TScalar>( in Multivector3<TScalar> l, in Multivector3<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			l.Scalar * r.Scalar + l.Vector.X * r.Vector.X + l.Vector.Y * r.Vector.Y + l.Vector.Z * r.Vector.Z - l.Bivector.XY * r.Bivector.XY - l.Bivector.YZ * r.Bivector.YZ - l.Bivector.ZX * r.Bivector.ZX - l.Trivector.XYZ * r.Trivector.XYZ,
			l.Scalar * r.Vector.X + l.Vector.Z * r.Bivector.ZX + l.Vector.X * r.Scalar + l.Bivector.XY * r.Vector.Y - l.Vector.Y * r.Bivector.XY - l.Bivector.YZ * r.Trivector.XYZ - l.Bivector.ZX * r.Vector.Z - l.Trivector.XYZ * r.Bivector.YZ,
			l.Scalar * r.Vector.Y + l.Vector.X * r.Bivector.XY + l.Vector.Y * r.Scalar + l.Bivector.YZ * r.Vector.Z - l.Vector.Z * r.Bivector.YZ - l.Bivector.ZX * r.Trivector.XYZ - l.Bivector.XY * r.Vector.X - l.Trivector.XYZ * r.Bivector.ZX,
			l.Scalar * r.Vector.Z + l.Vector.Y * r.Bivector.YZ + l.Vector.Z * r.Scalar + l.Bivector.ZX * r.Vector.X - l.Vector.X * r.Bivector.ZX - l.Bivector.XY * r.Trivector.XYZ - l.Bivector.YZ * r.Vector.Y - l.Trivector.XYZ * r.Bivector.XY,
			l.Scalar * r.Bivector.YZ + l.Vector.Y * r.Vector.Z + l.Vector.X * r.Trivector.XYZ + l.Bivector.XY * r.Bivector.ZX + l.Bivector.YZ * r.Scalar + l.Trivector.XYZ * r.Vector.X - l.Vector.Z * r.Vector.Y - l.Bivector.ZX * r.Bivector.XY,
			l.Scalar * r.Bivector.ZX + l.Vector.Z * r.Vector.X + l.Vector.Y * r.Trivector.XYZ + l.Bivector.YZ * r.Bivector.XY + l.Bivector.ZX * r.Scalar + l.Trivector.XYZ * r.Vector.Y - l.Vector.X * r.Vector.Z - l.Bivector.XY * r.Bivector.YZ,
			l.Scalar * r.Bivector.XY + l.Vector.X * r.Vector.Y + l.Vector.Z * r.Trivector.XYZ + l.Bivector.ZX * r.Bivector.YZ + l.Bivector.XY * r.Scalar + l.Trivector.XYZ * r.Vector.Z - l.Vector.Y * r.Vector.X - l.Bivector.YZ * r.Bivector.ZX,
			l.Scalar * r.Trivector.XYZ + l.Vector.X * r.Bivector.YZ + l.Vector.Y * r.Bivector.ZX + l.Vector.Z * r.Bivector.XY + l.Bivector.XY * r.Vector.Z + l.Bivector.YZ * r.Vector.X + l.Bivector.ZX * r.Vector.Y + l.Trivector.XYZ * r.Scalar
		);
}