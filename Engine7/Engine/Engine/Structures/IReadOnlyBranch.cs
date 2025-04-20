using System.Numerics;

namespace Engine.Structures;

public interface IReadOnlyBranch<T, TScalar>
	where T : IOcTreeLeaf<TScalar>
	where TScalar : unmanaged, INumber<TScalar> {
	AABB<Vector3<TScalar>> BranchBounds { get; }
	AABB<Vector3<TScalar>> ActualBounds { get; }
	IReadOnlyCollection<T> Contents { get; }
	uint Level { get; }
	int Count { get; }
}
