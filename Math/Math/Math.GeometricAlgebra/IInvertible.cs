namespace Math.GeometricAlgebra;

public interface IInvertible<TVector>
	where TVector :
		unmanaged, IInvertible<TVector>
{
	/// <summary>
	/// Does not work that well for integers compared to floating point numbers
	/// </summary>
	/// <returns>False if the inverse can't be obtained</returns>
	bool TryGetInverse(out TVector v);
}