using System.Numerics;

namespace Engine.Math;

public readonly struct Multivector4<T>( T scalar, in Vector4<T> vector, in Bivector4<T> bivector, in Trivector4<T> trivector, in Quadvector4<T> quadvector ) where T : unmanaged, INumberBase<T> {
	public readonly T Scalar = scalar;
	public readonly Vector4<T> Vector = vector;
	public readonly Bivector4<T> Bivector = bivector;
	public readonly Trivector4<T> Trivector = trivector;
	public readonly Quadvector4<T> Quadvector = quadvector;
}
