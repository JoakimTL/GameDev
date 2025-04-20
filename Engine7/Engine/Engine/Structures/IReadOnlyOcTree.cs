using System.Numerics;

namespace Engine.Structures;

public interface IReadOnlyOcTree<T, TScalar>
	where T : IOcTreeLeaf<TScalar>
	where TScalar : unmanaged, INumber<TScalar> {
	IReadOnlyList<AABB<Vector3<TScalar>>> GetBoundsAtLevel( uint level = 0 );
	IReadOnlyList<IReadOnlyCollection<T>> GetContentsAtLevel( uint level = 0 );
	IReadOnlyList<T> Get( AABB<Vector3<TScalar>> bounds, bool requireLeafIntersection = true );
	int Get( List<T> outputList, AABB<Vector3<TScalar>> bounds, bool requireLeafIntersection = true );
	IReadOnlyList<IReadOnlyBranch<T, TScalar>> GetBranches( AABB<Vector3<TScalar>> bounds );
	IReadOnlyList<IReadOnlyBranch<T, TScalar>> GetBranches();
	AABB<Vector3<TScalar>> MaxDepthBounds { get; }
}
