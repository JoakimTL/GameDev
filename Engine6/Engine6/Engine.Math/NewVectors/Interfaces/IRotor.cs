using System.Numerics;

namespace Engine.Math.NewVectors.Interfaces;

public interface IRotor<TRotor, TVector, TScalar> :
		IVector<TRotor, TScalar>
	where TRotor :
		unmanaged, IRotor<TRotor, TVector, TScalar>
	where TVector :
		unmanaged, IVector<TVector, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	TRotor Conjugate();
	TVector Rotate( in TVector v );
}