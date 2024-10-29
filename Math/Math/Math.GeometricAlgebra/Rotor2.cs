using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Math.GeometricAlgebra;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public readonly struct Rotor2<TScalar>(TScalar scalar, Bivector2<TScalar> bivector) :
		IVector<Rotor2<TScalar>, TScalar>,
		IRotor<Rotor2<TScalar>, Vector2<TScalar>, TScalar>,
		IPartOfMultivector<Multivector2<TScalar>, Rotor2<TScalar>>,
		ILinearAlgebraVectorOperators<Rotor2<TScalar>>,
		ILinearAlgebraScalarOperators<Rotor2<TScalar>, TScalar>,
		IProduct<Rotor2<TScalar>, Vector2<TScalar>, Vector2<TScalar>>,
		IProduct<Rotor2<TScalar>, Bivector2<TScalar>, Rotor2<TScalar>>,
		IProduct<Rotor2<TScalar>, Rotor2<TScalar>, Rotor2<TScalar>>,
		IProduct<Rotor2<TScalar>, Multivector2<TScalar>, Multivector2<TScalar>>,
		IExplicitCast<Rotor2<TScalar>, TScalar>,
		IExplicitCast<Rotor2<TScalar>, Bivector2<TScalar>>
	where TScalar :
		unmanaged, INumber<TScalar>
{
	public readonly TScalar Scalar = scalar;
	public readonly Bivector2<TScalar> Bivector = bivector;

	public static Rotor2<TScalar> AdditiveIdentity => Zero;
	public static Rotor2<TScalar> MultiplicativeIdentity => new(TScalar.One, Bivector2<TScalar>.Zero);
	public static Rotor2<TScalar> Zero { get; } = new(TScalar.Zero, Bivector2<TScalar>.Zero);
	public static Rotor2<TScalar> One { get; } = new(TScalar.One, Bivector2<TScalar>.One);
	public static Rotor2<TScalar> Two { get; } = One + One;

	public Multivector2<TScalar> GetMultivector() => new(Scalar, Vector2<TScalar>.Zero, Bivector);

	public Rotor2<TScalar> Negate() => new(-Scalar, -Bivector);
	public Rotor2<TScalar> Add(in Rotor2<TScalar> r) => new(Scalar + r.Scalar, Bivector + r.Bivector);
	public Rotor2<TScalar> Subtract(in Rotor2<TScalar> r) => new(Scalar - r.Scalar, Bivector - r.Bivector);
	public Rotor2<TScalar> ScalarMultiply(TScalar r) => new(Scalar * r, Bivector * r);
	public Rotor2<TScalar> ScalarDivide(TScalar r) => new(Scalar / r, Bivector / r);
	public static Rotor2<TScalar> DivideScalar(TScalar l, in Rotor2<TScalar> r) => new(l / r.Scalar, l / r.Bivector);
	public TScalar Dot(in Rotor2<TScalar> r) => Scalar * r.Scalar + Bivector.Dot(r.Bivector);
	public TScalar MagnitudeSquared() => Scalar * Scalar + Bivector.MagnitudeSquared();

	public Rotor2<TScalar> Conjugate() => new(Scalar, -Bivector);
	public Vector2<TScalar> Rotate(in Vector2<TScalar> v) => Multiply(v).Multiply(Conjugate());
	public bool TryGetInverse(out Rotor2<TScalar> rotor)
	{
		Rotor2<TScalar> conjugate = Conjugate();
		TScalar magnitudeSquared = conjugate.MagnitudeSquared();
		if (TScalar.IsZero(magnitudeSquared))
		{
			rotor = default;
			return false;
		}
		rotor = conjugate / magnitudeSquared;
		return true;
	}

	public Vector2<TScalar> Multiply(in Vector2<TScalar> r) => GeometricAlgebraMath2.Multiply(this, r);
	public Rotor2<TScalar> Multiply(in Bivector2<TScalar> r) => GeometricAlgebraMath2.Multiply(this, r);
	public Rotor2<TScalar> Multiply(in Rotor2<TScalar> r) => GeometricAlgebraMath2.Multiply(this, r);
	public Multivector2<TScalar> Multiply(in Multivector2<TScalar> r) => GeometricAlgebraMath2.Multiply(this, r);
	public static Vector2<TScalar> operator *(in Rotor2<TScalar> l, in Vector2<TScalar> r) => l.Multiply(r);
	public static Rotor2<TScalar> operator *(in Rotor2<TScalar> l, in Bivector2<TScalar> r) => l.Multiply(r);
	public static Rotor2<TScalar> operator *(in Rotor2<TScalar> l, in Rotor2<TScalar> r) => l.Multiply(r);
	public static Multivector2<TScalar> operator *(in Rotor2<TScalar> l, in Multivector2<TScalar> r) => l.Multiply(r);

	public static Rotor2<TScalar> operator -(in Rotor2<TScalar> l) => l.Negate();
	public static Rotor2<TScalar> operator +(in Rotor2<TScalar> l, in Rotor2<TScalar> r) => l.Add(r);
	public static Rotor2<TScalar> operator -(in Rotor2<TScalar> l, in Rotor2<TScalar> r) => l.Subtract(r);
	public static Rotor2<TScalar> operator *(in Rotor2<TScalar> l, TScalar r) => l.ScalarMultiply(r);
	public static Rotor2<TScalar> operator *(TScalar l, in Rotor2<TScalar> r) => r.ScalarMultiply(l);
	public static Rotor2<TScalar> operator /(in Rotor2<TScalar> l, TScalar r) => l.ScalarDivide(r);
	public static Rotor2<TScalar> operator /(TScalar l, in Rotor2<TScalar> r) => DivideScalar(l, r);

	public static bool operator ==(in Rotor2<TScalar> l, in Rotor2<TScalar> r) => l.Scalar == r.Scalar && l.Bivector == r.Bivector;
	public static bool operator !=(in Rotor2<TScalar> l, in Rotor2<TScalar> r) => !(l == r);
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is Rotor2<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine(Scalar, Bivector);
	public override string ToString()
		=> $"<{Scalar.ToFormattedString(true)} + {Bivector}>";

	public static explicit operator TScalar(in Rotor2<TScalar> part) => part.Scalar;
	public static explicit operator Bivector2<TScalar>(in Rotor2<TScalar> part) => part.Bivector;
}
