using System.Numerics;

namespace Math.GeometricAlgebra;

public interface IVectorPartsOperations<TVector, TScalar>
	where TVector :
		unmanaged, IVectorPartsOperations<TVector, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar>
{
	TScalar SumOfParts();
	TScalar ProductOfParts();
	TScalar SumOfUnitBasisAreas();
	TScalar SumOfUnitBasisVolumes();
}
