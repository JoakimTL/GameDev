using System.Numerics;

namespace Math.GeometricAlgebra;

public interface IRotor<TRotor, TVector, TScalar> :
		IVector<TRotor, TScalar>, IInvertible<TRotor>
	where TRotor :
		unmanaged, IRotor<TRotor, TVector, TScalar>
	where TVector :
		unmanaged, IVector<TVector, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar>
{
	TRotor Conjugate();
	TVector Rotate(in TVector v);
}