namespace Engine.Math.NewVectors.Interfaces;

public interface IEntrywiseMinMaxOperations<TVector>
	where TVector :
		unmanaged, IEntrywiseMinMaxOperations<TVector> {
	static abstract TVector Min( in TVector l, in TVector r );
	static abstract TVector Max( in TVector l, in TVector r );
}
