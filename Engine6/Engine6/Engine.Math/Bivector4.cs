using System.Numerics;

namespace Engine.Math;

public readonly struct Bivector4<T>( T yz, T zx, T xy, T yw, T zw, T xw ) :
		IAdditiveIdentity<Bivector4<T>, Bivector4<T>>,
		IMultiplicativeIdentity<Bivector4<T>, Bivector4<T>>
	where T : 
		unmanaged, INumberBase<T> {

	public readonly T YZ = yz;
	public readonly T ZX = zx;
	public readonly T XY = xy;
	public readonly T YW = yw;
	public readonly T ZW = zw;
	public readonly T XW = xw;

	public static Bivector4<T> AdditiveIdentity => Zero;
	public static Bivector4<T> MultiplicativeIdentity => One;

	public static readonly Bivector4<T> Zero = new( T.Zero, T.Zero, T.Zero, T.Zero, T.Zero, T.Zero );
	public static readonly Bivector4<T> One = new( T.One, T.One, T.One, T.One, T.One, T.One );

}
