namespace Math.GeometricAlgebra;

public interface IEntrywiseProductOperations<TVector>
	where TVector :
		unmanaged, IEntrywiseProductOperations<TVector>
{
	TVector MultiplyEntrywise(in TVector r);
	TVector DivideEntrywise(in TVector r);
}
