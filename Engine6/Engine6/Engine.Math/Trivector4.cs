using System.Numerics;

namespace Engine.Math;

public readonly struct Trivector4<T>( T xyz, T xyw, T xzw, T yzw ) :
		IAdditiveIdentity<Trivector4<T>, Trivector4<T>>,
		IMultiplicativeIdentity<Trivector4<T>, Trivector4<T>>
	where T :
		unmanaged, INumberBase<T> {
	public readonly T XYZ = xyz;
	public readonly T XYW = xyw;
	public readonly T XZW = xzw;
	public readonly T YZW = yzw;

	public static Trivector4<T> AdditiveIdentity => Zero;
	public static Trivector4<T> MultiplicativeIdentity => One;

	public static readonly Trivector4<T> Zero = new( T.Zero, T.Zero, T.Zero, T.Zero );
	public static readonly Trivector4<T> One = new( T.One, T.One, T.One, T.One );

}
