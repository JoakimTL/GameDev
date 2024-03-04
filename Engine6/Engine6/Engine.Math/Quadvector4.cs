using System.Numerics;

namespace Engine.Math;

public readonly struct Quadvector4<T>( T xyzw ) :
		IAdditiveIdentity<Quadvector4<T>, Quadvector4<T>>,
		IMultiplicativeIdentity<Quadvector4<T>, Quadvector4<T>>
	where T :
		unmanaged, INumberBase<T> {
	public readonly T XYZW = xyzw;

	public static Quadvector4<T> AdditiveIdentity => Zero;
	public static Quadvector4<T> MultiplicativeIdentity => One;

	public static readonly Quadvector4<T> Zero = new( T.Zero );
	public static readonly Quadvector4<T> One = new( T.One );

	public static implicit operator Quadvector4<T>( T xyzw ) => new( xyzw );
}
