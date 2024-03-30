using Engine.Math.Calculation;
using Engine.Math.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Engine.Math;

[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public readonly struct Matrix2x2<T>( T m00, T m01, T m10, T m11 ) :
		ILinearOperators<Matrix2x2<T>, T>,
		IProductOperator<Matrix2x2<T>, Matrix2x2<T>, Matrix2x2<T>>
	where T :
		unmanaged, INumber<T> {

	public readonly T M00 = m00;
	public readonly T M01 = m01;
	public readonly T M10 = m10;
	public readonly T M11 = m11;

	public Vector2<T> Row0 => new( M00, M01 );
	public Vector2<T> Row1 => new( M10, M11 );
	public Vector2<T> Col0 => new( M00, M10 );
	public Vector2<T> Col1 => new( M01, M11 );

	public static Matrix2x2<T> Identity => new(
		T.One, T.Zero,
		T.Zero, T.One
	);

	public static Matrix2x2<T> operator -( in Matrix2x2<T> l ) => l.Negate();
	public static Matrix2x2<T> operator -( in Matrix2x2<T> l, in Matrix2x2<T> r ) => l.Subtract( r );
	public static Matrix2x2<T> operator +( in Matrix2x2<T> l, in Matrix2x2<T> r ) => l.Add( r );
	public static Matrix2x2<T> operator *( in Matrix2x2<T> l, T r ) => l.ScalarMultiply( r );
	public static Matrix2x2<T> operator *( T l, in Matrix2x2<T> r ) => r.ScalarMultiply( l );
	public static Matrix2x2<T> operator /( in Matrix2x2<T> l, T r ) => l.ScalarDivide( r );
	public static Matrix2x2<T> operator *( in Matrix2x2<T> l, in Matrix2x2<T> r ) => l.Multiply( r );

	public static bool operator ==( in Matrix2x2<T> l, in Matrix2x2<T> r ) => l.Row0 == r.Row0 && l.Row1 == r.Row1;
	public static bool operator !=( in Matrix2x2<T> l, in Matrix2x2<T> r ) => !(l == r);
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Matrix2x2<T> v && this == v;
	public override int GetHashCode() => HashCode.Combine( Row0, Row1 );
	public override string ToString() => $"[{Row0}]/[{Row1}]";
}
