using System.Numerics;

namespace Engine.Math.NewVectors.Interfaces;

public interface IEntrywiseOperations<TVector, TScalar>
	where TVector :
		unmanaged, IEntrywiseOperations<TVector, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	//TODO: Might be a slow operation, use sparingly
	TVector EntrywiseOperation( Func<TScalar, TScalar> operation );
}