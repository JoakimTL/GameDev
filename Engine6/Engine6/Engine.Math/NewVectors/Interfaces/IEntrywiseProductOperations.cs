namespace Engine.Math.NewVectors.Interfaces;

public interface IEntrywiseProductOperations<TVector>
	where TVector :
		unmanaged, IEntrywiseProductOperations<TVector> {
	static abstract TVector MultiplyEntrywise( in TVector l, in TVector r );
	static abstract TVector DivideEntrywise( in TVector l, in TVector r );
}
