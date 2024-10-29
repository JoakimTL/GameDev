using System.Numerics;

namespace Math.GeometricAlgebra;
public interface ILinearAlgebraScalarOperators<TVector, TScalar>
	where TVector :
		unmanaged, ILinearAlgebraScalarOperators<TVector, TScalar>, ILinearAlgebraScalarOperations<TVector, TScalar>
	where TScalar :
		unmanaged, INumber<TScalar>
{
	static abstract TVector operator *(in TVector l, TScalar r);
	static abstract TVector operator *(TScalar l, in TVector r);
	static abstract TVector operator /(in TVector l, TScalar r);
	static abstract TVector operator /(TScalar l, in TVector r);
}
