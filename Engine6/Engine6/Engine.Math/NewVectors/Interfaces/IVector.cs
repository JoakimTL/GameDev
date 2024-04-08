using Engine.Math.NewFolder.Calculation.Interfaces;
using System.Numerics;

namespace Engine.Math.NewVectors.Interfaces;

public interface IVector<TVector, TScalar> :
		ILinearAlgebraOperations<TVector, TScalar>,
		IInnerProduct<TVector, TScalar>,
		IAdditiveIdentity<TVector, TVector>,
		IMultiplicativeIdentity<TVector, TVector>,
		IInEqualityOperators<TVector, TVector, bool>
	where TVector :
		unmanaged, IVector<TVector, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	static abstract TVector One { get; }
	static abstract TVector Zero { get; }
}