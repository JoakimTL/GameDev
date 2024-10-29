using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Math.GeometricAlgebra;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public readonly struct Trivector3<TScalar>(TScalar xyz) :
		IVector<Trivector3<TScalar>, TScalar>,
		IPartOfMultivector<Multivector3<TScalar>, Trivector3<TScalar>>,
		ILinearAlgebraVectorOperators<Trivector3<TScalar>>,
		ILinearAlgebraScalarOperators<Trivector3<TScalar>, TScalar>,
		IProduct<Trivector3<TScalar>, Vector3<TScalar>, Bivector3<TScalar>>,
		IProduct<Trivector3<TScalar>, Bivector3<TScalar>, Vector3<TScalar>>,
		IProduct<Trivector3<TScalar>, Trivector3<TScalar>, TScalar>,
		IProduct<Trivector3<TScalar>, Rotor3<TScalar>, Multivector3<TScalar>>,
		IProduct<Trivector3<TScalar>, Multivector3<TScalar>, Multivector3<TScalar>>
	where TScalar :
		unmanaged, INumber<TScalar>
{
	public readonly TScalar XYZ = xyz;

	public static Trivector3<TScalar> AdditiveIdentity => Zero;
	public static Trivector3<TScalar> MultiplicativeIdentity => One;
	public static Trivector3<TScalar> Zero { get; } = new(TScalar.Zero);
	public static Trivector3<TScalar> One { get; } = new(TScalar.One);
	public static Trivector3<TScalar> Two { get; } = One + One;

	public Multivector3<TScalar> GetMultivector() => new(TScalar.Zero, Vector3<TScalar>.Zero, Bivector3<TScalar>.Zero, this);

	public Trivector3<TScalar> Negate() => new(-XYZ);
	public Trivector3<TScalar> Add(in Trivector3<TScalar> r) => new(XYZ + r.XYZ);
	public Trivector3<TScalar> Subtract(in Trivector3<TScalar> r) => new(XYZ - r.XYZ);
	public Trivector3<TScalar> ScalarMultiply(TScalar r) => new(XYZ * r);
	public Trivector3<TScalar> ScalarDivide(TScalar r) => new(XYZ / r);
	public static Trivector3<TScalar> DivideScalar(TScalar l, in Trivector3<TScalar> r) => new(l / r.XYZ);
	public TScalar Dot(in Trivector3<TScalar> r) => -XYZ * r.XYZ;
	public TScalar MagnitudeSquared() => XYZ * XYZ;

	public Bivector3<TScalar> Multiply(in Vector3<TScalar> r) => GeometricAlgebraMath3.Multiply(this, r);
	public Vector3<TScalar> Multiply(in Bivector3<TScalar> r) => GeometricAlgebraMath3.Multiply(this, r);
	public TScalar Multiply(in Trivector3<TScalar> r) => GeometricAlgebraMath3.Multiply(this, r);
	public Multivector3<TScalar> Multiply(in Rotor3<TScalar> r) => GeometricAlgebraMath3.Multiply(this, r);
	public Multivector3<TScalar> Multiply(in Multivector3<TScalar> r) => GeometricAlgebraMath3.Multiply(this, r);
	public static Bivector3<TScalar> operator *(in Trivector3<TScalar> l, in Vector3<TScalar> r) => l.Multiply(r);
	public static Vector3<TScalar> operator *(in Trivector3<TScalar> l, in Bivector3<TScalar> r) => l.Multiply(r);
	public static TScalar operator *(in Trivector3<TScalar> l, in Trivector3<TScalar> r) => l.Multiply(r);
	public static Multivector3<TScalar> operator *(in Trivector3<TScalar> l, in Rotor3<TScalar> r) => l.Multiply(r);
	public static Multivector3<TScalar> operator *(in Trivector3<TScalar> l, in Multivector3<TScalar> r) => l.Multiply(r);

	public static Trivector3<TScalar> operator -(in Trivector3<TScalar> l) => l.Negate();
	public static Trivector3<TScalar> operator +(in Trivector3<TScalar> l, in Trivector3<TScalar> r) => l.Add(r);
	public static Trivector3<TScalar> operator -(in Trivector3<TScalar> l, in Trivector3<TScalar> r) => l.Subtract(r);
	public static Trivector3<TScalar> operator *(in Trivector3<TScalar> l, TScalar r) => l.ScalarMultiply(r);
	public static Trivector3<TScalar> operator *(TScalar l, in Trivector3<TScalar> r) => r.ScalarMultiply(l);
	public static Trivector3<TScalar> operator /(in Trivector3<TScalar> l, TScalar r) => l.ScalarDivide(r);
	public static Trivector3<TScalar> operator /(TScalar l, in Trivector3<TScalar> r) => DivideScalar(l, r);

	public static bool operator ==(in Trivector3<TScalar> l, in Trivector3<TScalar> r) => l.XYZ == r.XYZ;
	public static bool operator !=(in Trivector3<TScalar> l, in Trivector3<TScalar> r) => !(l == r);
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is Trivector3<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine(XYZ);
	public override string ToString()
		=> $"[{XYZ.ToFormattedString(true)}XYZ]";
}
