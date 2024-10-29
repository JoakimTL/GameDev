namespace Math.GeometricAlgebra;

public interface ISquareMatrix<TMatrix> : IInvertible<TMatrix>
	where TMatrix :
		unmanaged, ISquareMatrix<TMatrix>
{
	TMatrix GetTransposed();
}
