using System.Numerics;

namespace Engine.Math.Operations.Interfaces;

public interface IAABBAreaOperations<T, TVector, TScalar> where T : unmanaged where TVector : unmanaged where TScalar : unmanaged, INumber<TScalar> {
	static abstract TScalar GetArea( in T aabb );
	static abstract IEnumerable<TVector> GetPointsInAreaExclusive( T aabb, TScalar increment );
	static abstract IEnumerable<TVector> GetPointsInAreaInclusive( T aabb, TScalar increment );
	static abstract IEnumerable<TVector> GetPointsInAreaExclusiveExcept( T aabb, T other, TScalar increment );
	static abstract IEnumerable<TVector> GetPointsInAreaIncusiveExcept( T aabb, T other, TScalar increment );
}

