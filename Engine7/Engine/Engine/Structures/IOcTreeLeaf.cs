using System.Numerics;

namespace Engine.Structures;

public interface IOcTreeLeaf<TScalar>
	where TScalar : unmanaged, INumber<TScalar> {
	AABB<Vector3<TScalar>> Bounds { get; }
}
