using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Math.GeometricAlgebra;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public readonly struct Vector4<TScalar>(TScalar x, TScalar y, TScalar z, TScalar w) :
		IVector<Vector4<TScalar>, TScalar>,
		IPartOfMultivector<Multivector4<TScalar>, Vector4<TScalar>>,
		IEntrywiseProductOperations<Vector4<TScalar>>,
		IEntrywiseOperations<Vector4<TScalar>, TScalar>,
		ILinearAlgebraVectorOperators<Vector4<TScalar>>,
		ILinearAlgebraScalarOperators<Vector4<TScalar>, TScalar>,
		IEntrywiseMinMaxOperations<Vector4<TScalar>>,
		IVectorPartsOperations<Vector4<TScalar>, TScalar>,
		IProduct<Vector4<TScalar>, Matrix4x4<TScalar>, Vector4<TScalar>>,
		IEntrywiseComparisonOperations<Vector4<TScalar>>,
		IEntrywiseComparisonOperators<Vector4<TScalar>>
	where TScalar :
		unmanaged, INumber<TScalar>
{
	public readonly TScalar X = x;
	public readonly TScalar Y = y;
	public readonly TScalar Z = z;
	public readonly TScalar W = w;

	public static Vector4<TScalar> AdditiveIdentity => Zero;
	public static Vector4<TScalar> MultiplicativeIdentity => One;
	public static Vector4<TScalar> Zero { get; } = new(TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.Zero);
	public static Vector4<TScalar> One { get; } = new(TScalar.One, TScalar.One, TScalar.One, TScalar.One);
	public static Vector4<TScalar> Two { get; } = One + One;

	public static Vector4<TScalar> UnitX { get; } = new(TScalar.One, TScalar.Zero, TScalar.Zero, TScalar.Zero);
	public static Vector4<TScalar> UnitY { get; } = new(TScalar.Zero, TScalar.One, TScalar.Zero, TScalar.Zero);
	public static Vector4<TScalar> UnitZ { get; } = new(TScalar.Zero, TScalar.Zero, TScalar.One, TScalar.Zero);
	public static Vector4<TScalar> UnitW { get; } = new(TScalar.Zero, TScalar.Zero, TScalar.Zero, TScalar.One);

	public Multivector4<TScalar> GetMultivector() => new(TScalar.Zero, this, Bivector4<TScalar>.Zero, Trivector4<TScalar>.Zero, Quadvector4<TScalar>.Zero);

	public Vector4<TScalar> Negate() => new(-X, -Y, -Z, -W);
	public Vector4<TScalar> Add(in Vector4<TScalar> r) => new(X + r.X, Y + r.Y, Z + r.Z, W + r.W);
	public Vector4<TScalar> Subtract(in Vector4<TScalar> r) => new(X - r.X, Y - r.Y, Z - r.Z, W - r.W);
	public Vector4<TScalar> ScalarMultiply(TScalar r) => new(X * r, Y * r, Z * r, W * r);
	public Vector4<TScalar> ScalarDivide(TScalar r) => new(X / r, Y / r, Z / r, W / r);
	public static Vector4<TScalar> DivideScalar(TScalar l, in Vector4<TScalar> r) => new(l / r.X, l / r.Y, l / r.Z, l / r.W);
	public Vector4<TScalar> MultiplyEntrywise(in Vector4<TScalar> r) => new(X * r.X, Y * r.Y, Z * r.Z, W * r.W);
	public Vector4<TScalar> DivideEntrywise(in Vector4<TScalar> r) => new(X / r.X, Y / r.Y, Z / r.Z, W / r.W);
	public Vector4<TScalar> EntrywiseOperation(Func<TScalar, TScalar> operation) => new(operation(X), operation(Y), operation(Z), operation(W));
	public TScalar Dot(in Vector4<TScalar> r) => X * r.X + Y * r.Y + Z * r.Z + W * r.W;
	public TScalar MagnitudeSquared() => Dot(this);
	public Vector4<TScalar> Min(in Vector4<TScalar> r) => new(TScalar.Min(X, r.X), TScalar.Min(Y, r.Y), TScalar.Min(Z, r.Z), TScalar.Min(W, r.W));
	public Vector4<TScalar> Max(in Vector4<TScalar> r) => new(TScalar.Max(X, r.X), TScalar.Max(Y, r.Y), TScalar.Max(Z, r.Z), TScalar.Max(W, r.W));
	public TScalar SumOfParts() => X + Y + Z + W;
	public TScalar ProductOfParts() => X * Y * Z * W;
	public TScalar SumOfUnitBasisAreas() => X * Y + Y * Z + Z * X + W * X + W * Y + W * Z;
	public TScalar SumOfUnitBasisVolumes() => X * Y * Z + Y * Z * W + Z * W * X + W * X * Y;

	public Vector4<TScalar> Multiply(in Matrix4x4<TScalar> m) => new(Dot(m.Col0), Dot(m.Col1), Dot(m.Col2), Dot(m.Col3));
	public static Vector4<TScalar> operator *(in Vector4<TScalar> l, in Matrix4x4<TScalar> r) => l.Multiply(r);

	public static Vector4<TScalar> operator -(in Vector4<TScalar> l) => l.Negate();
	public static Vector4<TScalar> operator +(in Vector4<TScalar> l, in Vector4<TScalar> r) => l.Add(r);
	public static Vector4<TScalar> operator -(in Vector4<TScalar> l, in Vector4<TScalar> r) => l.Subtract(r);
	public static Vector4<TScalar> operator *(in Vector4<TScalar> l, TScalar r) => l.ScalarMultiply(r);
	public static Vector4<TScalar> operator *(TScalar l, in Vector4<TScalar> r) => r.ScalarMultiply(l);
	public static Vector4<TScalar> operator /(in Vector4<TScalar> l, TScalar r) => l.ScalarDivide(r);
	public static Vector4<TScalar> operator /(TScalar l, in Vector4<TScalar> r) => DivideScalar(l, r);

	public bool GreaterThanEntrywise(in Vector4<TScalar> other) => X > other.X && Y > other.Y && Z > other.Z && W > other.W;
	public bool GreaterThanOrEqualEntrywise(in Vector4<TScalar> other) => X >= other.X && Y >= other.Y && Z >= other.Z && W >= other.W;
	public static bool operator <(in Vector4<TScalar> left, in Vector4<TScalar> right) => right.GreaterThanEntrywise(left);
	public static bool operator >(in Vector4<TScalar> left, in Vector4<TScalar> right) => left.GreaterThanEntrywise(right);
	public static bool operator <=(in Vector4<TScalar> left, in Vector4<TScalar> right) => right.GreaterThanOrEqualEntrywise(left);
	public static bool operator >=(in Vector4<TScalar> left, in Vector4<TScalar> right) => left.GreaterThanOrEqualEntrywise(right);

	public static bool operator ==(in Vector4<TScalar> l, in Vector4<TScalar> r) => l.X == r.X && l.Y == r.Y && l.Z == r.Z && l.W == r.W;
	public static bool operator !=(in Vector4<TScalar> l, in Vector4<TScalar> r) => !(l == r);
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is Vector4<TScalar> v && this == v;
	public override int GetHashCode() => HashCode.Combine(X, Y);
	public override string ToString()
		=> $"[{X.ToFormattedString(true)}X {Y.ToFormattedString()}Y {Z.ToFormattedString()}Z {W.ToFormattedString()}W]";

	public static implicit operator Vector4<TScalar>(TScalar s) => new(s, s, s, s);
	public static implicit operator Vector4<TScalar>((TScalar x, TScalar y, TScalar z, TScalar w) tuple) => new(tuple.x, tuple.y, tuple.z, tuple.w);
}