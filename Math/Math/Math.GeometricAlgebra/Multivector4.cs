using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Math.GeometricAlgebra;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public readonly struct Multivector4<TScalar>(TScalar scalar, Vector4<TScalar> vector, Bivector4<TScalar> bivector, Trivector4<TScalar> trivector, Quadvector4<TScalar> quadvector) :
		IVector<Multivector4<TScalar>, TScalar>,
		IPartOfMultivector<Multivector4<TScalar>, Multivector4<TScalar>>,
		ILinearAlgebraVectorOperators<Multivector4<TScalar>>,
		ILinearAlgebraScalarOperators<Multivector4<TScalar>, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar>
{
	public readonly TScalar Scalar = scalar;
	public readonly Vector4<TScalar> Vector = vector;
	public readonly Bivector4<TScalar> Bivector = bivector;
	public readonly Trivector4<TScalar> Trivector = trivector;
	public readonly Quadvector4<TScalar> Quadvector = quadvector;

	public Multivector4(TScalar scalar, TScalar x, TScalar y, TScalar z, TScalar w, TScalar yz, TScalar zx, TScalar xy, TScalar yw, TScalar zw, TScalar xw, TScalar yzw, TScalar xzw, TScalar xyw, TScalar xyz, TScalar xyzw)
		: this(scalar, new(x, y, z, w), new(yz, zx, xy, yw, zw, xw), new(yzw, xzw, xyw, xyz), new(xyzw)) { }

	public static Multivector4<TScalar> AdditiveIdentity => Zero;
	public static Multivector4<TScalar> MultiplicativeIdentity => One;
	public static Multivector4<TScalar> Zero { get; } = new(TScalar.Zero, Vector4<TScalar>.Zero, Bivector4<TScalar>.Zero, Trivector4<TScalar>.Zero, Quadvector4<TScalar>.Zero);
	public static Multivector4<TScalar> One { get; } = new(TScalar.One, Vector4<TScalar>.One, Bivector4<TScalar>.One, Trivector4<TScalar>.One, Quadvector4<TScalar>.One);
	public static Multivector4<TScalar> Two { get; } = One + One;

	public Multivector4<TScalar> GetMultivector() => this;

	public Multivector4<TScalar> Negate() => new(-Scalar, -Vector, -Bivector, -Trivector, -Quadvector);
	public Multivector4<TScalar> Add(in Multivector4<TScalar> r) => new(Scalar + r.Scalar, Vector + r.Vector, Bivector + r.Bivector, Trivector + r.Trivector, Quadvector + r.Quadvector);
	public Multivector4<TScalar> Subtract(in Multivector4<TScalar> r) => new(Scalar - r.Scalar, Vector - r.Vector, Bivector - r.Bivector, Trivector - r.Trivector, Quadvector - r.Quadvector);
	public Multivector4<TScalar> ScalarMultiply(TScalar r) => new(Scalar * r, Vector * r, Bivector * r, Trivector * r, Quadvector * r);
	public Multivector4<TScalar> ScalarDivide(TScalar r) => new(Scalar / r, Vector / r, Bivector / r, Trivector / r, Quadvector / r);
	public static Multivector4<TScalar> DivideScalar(TScalar l, in Multivector4<TScalar> r) => new(l / r.Scalar, l / r.Vector, l / r.Bivector, l / r.Trivector, l / r.Quadvector);
	public TScalar Dot(in Multivector4<TScalar> r) => Scalar * r.Scalar + Vector.Dot(r.Vector) + Bivector.Dot(r.Bivector) + Trivector.Dot(r.Trivector) + Quadvector.Dot(r.Quadvector);
	public TScalar MagnitudeSquared() => Scalar * Scalar + Vector.MagnitudeSquared() + Bivector.MagnitudeSquared() + Trivector.MagnitudeSquared() + Quadvector.MagnitudeSquared();

	public static Multivector4<TScalar> operator -(in Multivector4<TScalar> l) => l.Negate();
	public static Multivector4<TScalar> operator +(in Multivector4<TScalar> l, in Multivector4<TScalar> r) => l.Add(r);
	public static Multivector4<TScalar> operator -(in Multivector4<TScalar> l, in Multivector4<TScalar> r) => l.Subtract(r);
	public static Multivector4<TScalar> operator *(in Multivector4<TScalar> l, TScalar r) => l.ScalarMultiply(r);
	public static Multivector4<TScalar> operator *(TScalar l, in Multivector4<TScalar> r) => r.ScalarMultiply(l);
	public static Multivector4<TScalar> operator /(in Multivector4<TScalar> l, TScalar r) => l.ScalarDivide(r);
	public static Multivector4<TScalar> operator /(TScalar l, in Multivector4<TScalar> r) => DivideScalar(l, r);

	public static bool operator ==(in Multivector4<TScalar> l, in Multivector4<TScalar> r) => l.Scalar == r.Scalar && l.Vector == r.Vector && l.Bivector == r.Bivector && l.Trivector == r.Trivector && l.Quadvector == r.Quadvector;
	public static bool operator !=(in Multivector4<TScalar> l, in Multivector4<TScalar> r) => !(l == r);
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is Multivector4<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine(Scalar, Vector, Bivector, Trivector, Quadvector);
	public override string ToString()
		=> $"<{Scalar.ToFormattedString(true)} + {Vector} + {Bivector} + {Trivector} + {Quadvector}>";
}
