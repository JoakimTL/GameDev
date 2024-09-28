using System.Numerics;

namespace Engine;

public interface ILinearAlgebraScalarOperations<TVector, TScalar>
	where TVector :
		unmanaged, ILinearAlgebraScalarOperations<TVector, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	TVector ScalarMultiply( TScalar r );
	TVector ScalarDivide( TScalar r );
	static abstract TVector DivideScalar( TScalar l, in TVector r );
}
