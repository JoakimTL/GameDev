namespace Math.GeometricAlgebra;

public interface ILinearAlgebraVectorOperators<TVector>
	where TVector :
		unmanaged, ILinearAlgebraVectorOperators<TVector>, ILinearAlgebraVectorOperations<TVector>
{
	static abstract TVector operator -(in TVector l);
	static abstract TVector operator +(in TVector l, in TVector r);
	static abstract TVector operator -(in TVector l, in TVector r);
}
