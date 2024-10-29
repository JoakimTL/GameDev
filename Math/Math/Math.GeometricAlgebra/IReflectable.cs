using System.Numerics;

namespace Math.GeometricAlgebra;

public interface IReflectable<TVector, TScalar> :
		IVector<TVector, TScalar>
	where TVector :
		unmanaged, IVector<TVector, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar>
{
	TVector ReflectNormal(in TVector normal);
}
