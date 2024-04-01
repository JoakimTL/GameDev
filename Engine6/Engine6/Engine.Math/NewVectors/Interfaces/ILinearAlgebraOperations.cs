using System.Numerics;

namespace Engine.Math.NewVectors.Interfaces;

public interface ILinearAlgebraOperations<TVector, TScalar>
	where TVector :
		unmanaged, ILinearAlgebraOperations<TVector, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	static abstract TVector Negate( in TVector l );
	static abstract TVector Add( in TVector l, in TVector r );
	static abstract TVector Subtract( in TVector l, in TVector r );
	static abstract TVector ScalarMultiply( in TVector l, TScalar r );
	static abstract TVector ScalarDivide( in TVector l, TScalar r );
	static abstract TVector DivideScalar( TScalar l, in TVector r );
}
