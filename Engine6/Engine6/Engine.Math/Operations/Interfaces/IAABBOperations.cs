namespace Engine.Math.Operations.Interfaces;

public interface IAABBOperations<T, TVector> where T : unmanaged where TVector : unmanaged {
	static abstract T Extend( in T aabb, in TVector v );
	static abstract bool Intersects( in T r, in T l );
	static abstract T GetLargestBounds( in T r, in T l );
	static abstract T GetSmallestBounds( in T r, in T l );
}

