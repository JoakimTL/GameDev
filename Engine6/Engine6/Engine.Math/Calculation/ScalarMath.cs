using System.Numerics;
using System.Runtime.CompilerServices;
using Engine.Math.Calculation.Interfaces;

namespace Engine.Math.Calculation;

/// <summary>
/// All methods that return <see cref="{T}"/> are implemented here."/>
/// </summary>
public sealed class ScalarMath<T> :
		IInnerProduct<Vector2<T>, T>,
		IInnerProduct<Bivector2<T>, T>,
		IInnerProduct<Rotor2<T>, T>,
		IInnerProduct<Multivector2<T>, T>,
		IInnerProduct<Vector3<T>, T>,
		IInnerProduct<Bivector3<T>, T>,
		IInnerProduct<Trivector3<T>, T>,
		IInnerProduct<Rotor3<T>, T>,
		IInnerProduct<Multivector3<T>, T>,
		IInnerProduct<Vector4<T>, T>,
		IInnerProduct<Bivector4<T>, T>,
		IGeometricProduct<Bivector2<T>, Bivector2<T>, T>,
		IGeometricProduct<Trivector3<T>, Trivector3<T>, T>,
		IGeometricProduct<Quadvector4<T>, Quadvector4<T>, T>
	where T :
		unmanaged, INumber<T> {

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Dot( in Vector2<T> l, in Vector2<T> r )
		=> l.X * r.X + l.Y * r.Y;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Dot( in Bivector2<T> l, in Bivector2<T> r )
		=> Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Dot( in Rotor2<T> l, in Rotor2<T> r )
		=> l.Scalar * r.Scalar + Dot( l.Bivector, r.Bivector );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Dot( in Multivector2<T> l, in Multivector2<T> r )
		=> l.Scalar * r.Scalar + Dot( l.Vector, r.Vector ) + Dot( l.Bivector, r.Bivector );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Dot( in Vector3<T> l, in Vector3<T> r )
		=> l.X * r.X + l.Y * r.Y + l.Z * r.Z;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Dot( in Bivector3<T> l, in Bivector3<T> r )
		=> -(l.XY * r.XY) - l.YZ * r.YZ - l.ZX * r.ZX;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Dot( in Trivector3<T> l, in Trivector3<T> r )
		=> Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Dot( in Rotor3<T> l, in Rotor3<T> r )
		=> l.Scalar * r.Scalar + Dot( l.Bivector, r.Bivector );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Dot( in Multivector3<T> l, in Multivector3<T> r )
		=> l.Scalar * r.Scalar + Dot( l.Vector, r.Vector ) + Dot( l.Bivector, r.Bivector ) + Dot( l.Trivector, r.Trivector );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Dot( in Vector4<T> l, in Vector4<T> r )
		=> l.X * r.X + l.Y * r.Y + l.Z * r.Z + l.W * r.W;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Dot( in Bivector4<T> l, in Bivector4<T> r )
		=> -(l.XY * r.XY) - l.YZ * r.YZ - l.ZX * r.ZX - l.YW * r.YW - l.ZW * r.ZW - l.XW * r.XW;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Dot( in Trivector4<T> l, in Trivector4<T> r )
		=> -(l.XYZ * r.XYZ) - l.XZW * r.XZW - l.XYW * r.XYW - l.YZW * r.YZW;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Dot( in Quadvector4<T> l, in Quadvector4<T> r )
		=> Multiply( l, r );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Multiply( in Bivector2<T> l, in Bivector2<T> r )
		=> -(l.XY * r.XY);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Multiply( in Trivector3<T> l, in Trivector3<T> r )
		=> -(l.XYZ * r.XYZ);

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T Multiply( in Quadvector4<T> l, in Quadvector4<T> r )
		=> l.XYZW * r.XYZW;

}
