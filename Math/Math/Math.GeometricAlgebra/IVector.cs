using System.Numerics;

namespace Math.GeometricAlgebra;

public interface IVector<TVector, TScalar> :
		ILinearAlgebraVectorOperations<TVector>,
		ILinearAlgebraScalarOperations<TVector, TScalar>,
		IInnerProduct<TVector, TScalar>,
		IMagnitudeSquared<TScalar>,
		IVectorIdentities<TVector>,
		IInEqualityOperators<TVector, TVector, bool>
	where TVector :
		unmanaged, IVector<TVector, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar>
{ }
