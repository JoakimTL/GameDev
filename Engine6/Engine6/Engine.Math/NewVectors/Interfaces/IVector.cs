using System.Numerics;

namespace Engine.Math.NewVectors.Interfaces;

public interface IVector<TVector, TScalar> :
		ILinearAlgebraVectorOperations<TVector>,
		ILinearAlgebraScalarOperations<TVector, TScalar>,
		IInnerProduct<TVector, TScalar>,
		IVectorIdentities<TVector>,
		IInEqualityOperators<TVector, TVector, bool>
	where TVector :
		unmanaged, IVector<TVector, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> { }
