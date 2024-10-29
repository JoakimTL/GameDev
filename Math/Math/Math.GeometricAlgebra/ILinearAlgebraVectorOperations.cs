namespace Math.GeometricAlgebra;

public interface ILinearAlgebraVectorOperations<TVector>
	where TVector :
		unmanaged, ILinearAlgebraVectorOperations<TVector>
{
	TVector Negate();
	TVector Add(in TVector r);
	TVector Subtract(in TVector r);
}
