using System.Numerics;

namespace Engine.Math.NewVectors.Interfaces;

public interface IVectorPartsOperations<TVector, TScalar>
	where TVector :
		unmanaged, IVectorPartsOperations<TVector, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	static abstract TScalar SumOfParts( in TVector l );
	static abstract TScalar ProductOfParts( in TVector l );
}
