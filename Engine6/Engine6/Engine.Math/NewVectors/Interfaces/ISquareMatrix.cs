namespace Engine.Math.NewVectors.Interfaces;

public interface ISquareMatrix<TMatrix>
	where TMatrix :
		unmanaged, ISquareMatrix<TMatrix> {
	TMatrix GetTransposed();
	/// <summary>
	/// Does not work that well for integers compared to floating point numbers
	/// </summary>
	/// <returns>False if the matrix can't be inverted</returns>
	bool TryGetInverse( out TMatrix matrix );
}
