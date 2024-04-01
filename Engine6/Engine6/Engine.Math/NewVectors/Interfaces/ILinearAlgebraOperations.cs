using System.Numerics;

namespace Engine.Math.NewVectors.Interfaces;

public interface ILinearAlgebraOperations<TVector, TScalar>
	where TVector :
		unmanaged, ILinearAlgebraOperations<TVector, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	TVector Negate();
	TVector Add(in TVector r );
	TVector Subtract( in TVector r );
	TVector ScalarMultiply( TScalar r );
	TVector ScalarDivide( TScalar r );
	static abstract TVector DivideScalar( TScalar l, in TVector r );
}
