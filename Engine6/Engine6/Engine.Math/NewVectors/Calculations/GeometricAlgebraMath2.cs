using System.Numerics;

namespace Engine.Math.NewVectors.Calculations;

public static class GeometricAlgebraMath2 {

	/*
	 * 0 - s - scalar
	 * 1 - v - vector2
	 * 2 - b - bivector2
	 * 3 - r - rotor2
	 * 4 - m - multivector2
	 * _____________________________
	 * | * |   | s | v | b | r | m |
	 * _____________________________
	 * | s |   | s | v | b | r | m |
	 * | v |   |   | r | v | v | m |
	 * | b |   |   |   | s | r | m |
	 * | r |   |   |   |   | r | m |
	 * | m |   |   |   |   |   | m |
	 * _____________________________
	 * 
	 * _____________________________
	 * | *     | 0 | 1 | 2 | 3 | 4 |
	 * |       |___________________|
	 * | 0 |   | 0 | 1 | 2 | 3 | 4 |
	 * | 1 |   | 1 | 3 | 1 | 1 | 4 |
	 * | 2 |   | 2 | 1 | 0 | 3 | 4 |
	 * | 3 |   | 3 | 1 | 3 | 3 | 4 |
	 * | 4 |   | 4 | 4 | 4 | 4 | 4 |
	 * _____________________________
	 */

	public static Rotor2<TScalar> Multiply<TScalar>( in Vector2<TScalar> l, in Vector2<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			(l.X * r.X) + (l.Y * r.Y),
			(l.X * r.Y) - (l.Y * r.X)
		);

	public static Vector2<TScalar> Multiply<TScalar>( in Vector2<TScalar> l, in Bivector2<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			-l.Y * r.XY,
			l.X * r.XY
		);

	public static Vector2<TScalar> Multiply<TScalar>( in Vector2<TScalar> l, in Rotor2<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			(l.X * r.Scalar) - (l.Y * r.Bivector.XY),
			(l.Y * r.Scalar) + (l.X * r.Bivector.XY)
		);

	public static Multivector2<TScalar> Multiply<TScalar>( in Vector2<TScalar> l, in Multivector2<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			(l.X * r.Vector.X) + (l.Y * r.Vector.Y),
			(l.X * r.Scalar) - (l.Y * r.Bivector.XY),
			(l.Y * r.Scalar) + (l.X * r.Bivector.XY),
			(l.X * r.Vector.Y) - (l.Y * r.Vector.X)
		);

	public static Vector2<TScalar> Multiply<TScalar>( in Bivector2<TScalar> l, in Vector2<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			l.XY * r.Y,
			-l.XY * r.X
		);

	public static TScalar Multiply<TScalar>( in Bivector2<TScalar> l, in Bivector2<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> -l.XY * r.XY;

	public static Rotor2<TScalar> Multiply<TScalar>( in Bivector2<TScalar> l, in Rotor2<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			-l.XY * r.Bivector.XY,
			l.XY * r.Scalar
		);

	public static Multivector2<TScalar> Multiply<TScalar>( in Bivector2<TScalar> l, in Multivector2<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			-l.XY * r.Bivector.XY,
			l.XY * r.Vector.Y,
			-l.XY * r.Vector.X,
			l.XY * r.Scalar
		);

	public static Vector2<TScalar> Multiply<TScalar>( in Rotor2<TScalar> l, in Vector2<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			(l.Scalar * r.X) + (l.Bivector.XY * r.Y),
			(l.Scalar * r.Y) - (l.Bivector.XY * r.X)
		);

	public static Rotor2<TScalar> Multiply<TScalar>( in Rotor2<TScalar> l, in Bivector2<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			-l.Bivector.XY * r.XY,
			l.Scalar * r.XY
		);

	public static Rotor2<TScalar> Multiply<TScalar>( in Rotor2<TScalar> l, in Rotor2<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			(l.Scalar * r.Scalar) - (l.Bivector.XY * r.Bivector.XY),
			(l.Scalar * r.Bivector.XY) + (l.Bivector.XY * r.Scalar)
		);

	public static Multivector2<TScalar> Multiply<TScalar>( in Rotor2<TScalar> l, in Multivector2<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			(l.Scalar * r.Scalar) - (l.Bivector.XY * r.Bivector.XY),
			(l.Scalar * r.Vector.X) + (l.Bivector.XY * r.Vector.Y),
			(l.Scalar * r.Vector.Y) - (l.Bivector.XY * r.Vector.X),
			(l.Scalar * r.Bivector.XY) + (l.Bivector.XY * r.Scalar)
		);

	public static Multivector2<TScalar> Multiply<TScalar>( in Multivector2<TScalar> l, in Vector2<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			(l.Vector.X * r.X) + (l.Vector.Y * r.Y),
			(l.Scalar * r.X) + (l.Bivector.XY * r.Y),
			(l.Scalar * r.Y) - (l.Bivector.XY * r.X),
			(l.Vector.X * r.Y) - (l.Vector.Y * r.X)
		);

	public static Multivector2<TScalar> Multiply<TScalar>( in Multivector2<TScalar> l, in Bivector2<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			-l.Bivector.XY * r.XY,
			-l.Vector.Y * r.XY,
			l.Vector.X * r.XY,
			l.Scalar * r.XY
		);

	public static Multivector2<TScalar> Multiply<TScalar>( in Multivector2<TScalar> l, in Rotor2<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			(l.Scalar * r.Scalar) - (l.Bivector.XY * r.Bivector.XY),
			(l.Vector.X * r.Scalar) - (l.Vector.Y * r.Bivector.XY),
			(l.Vector.Y * r.Scalar) + (l.Vector.X * r.Bivector.XY),
			(l.Scalar * r.Bivector.XY) + (l.Bivector.XY * r.Scalar)
		);

	public static Multivector2<TScalar> Multiply<TScalar>( in Multivector2<TScalar> l, in Multivector2<TScalar> r )
		where TScalar : unmanaged, INumber<TScalar>
		=> new(
			(l.Scalar * r.Scalar) + (l.Vector.X * r.Vector.X) + (l.Vector.Y * r.Vector.Y) - (l.Bivector.XY * r.Bivector.XY),
			(l.Scalar * r.Vector.X) + (l.Vector.X * r.Scalar) + (l.Bivector.XY * r.Vector.Y) - (l.Vector.Y * r.Bivector.XY),
			(l.Scalar * r.Vector.Y) + (l.Vector.Y * r.Scalar) + (l.Vector.X * r.Bivector.XY) - (l.Bivector.XY * r.Vector.X),
			(l.Scalar * r.Bivector.XY) + (l.Bivector.XY * r.Scalar) + (l.Vector.X * r.Vector.Y) - (l.Vector.Y * r.Vector.X)
		);
}
