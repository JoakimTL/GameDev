namespace Engine.Math.NewVectors.Interfaces;

public interface IEntrywiseMinMaxOperations<TVector>
	where TVector :
		unmanaged, IEntrywiseMinMaxOperations<TVector> {
	TVector Min( in TVector r );
	TVector Max( in TVector r );
}
