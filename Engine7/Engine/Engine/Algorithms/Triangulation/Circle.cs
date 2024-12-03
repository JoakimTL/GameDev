using System.Numerics;

namespace Engine.Algorithms.Triangulation;

public readonly struct Circle<TScalar>( Vector2<TScalar> center, TScalar radius )
	where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {
	public readonly Vector2<TScalar> Center = center;
	public readonly TScalar Radius = radius;
}