namespace Engine;

public interface IMatrixTriangulation<TMatrix>
	where TMatrix :
		unmanaged, IMatrixTriangulation<TMatrix> {
	/// <returns>False if triangulation failed</returns>
	bool TryGetUpperTriangular( out TMatrix upperTriangular );
	/// <summary>
	/// Returns the decomposition of the matrix into a lower and upper triangular matrix.
	/// </summary>
	/// <param name="upperDecomposition">The upper triangular matrix, same as <see cref="TryGetUpperTriangular(out TMatrix)"/></param>
	/// <returns>False if triangulation failed</returns>
	bool TryGetUpperTriangularDecomposition( out TMatrix lowerDecomposition, out TMatrix upperDecomposition );
	/// <returns>False if triangulation failed</returns>
	bool TryGetLowerTriangular( out TMatrix lowerTriangular );
	/// <summary>
	/// Returns the decomposition of the matrix into a lower and upper triangular matrix.
	/// </summary>
	/// <param name="lowerDecomposition">The upper triangular matrix, same as <see cref="TryGetLowerTriangular(out TMatrix)"/></param>
	/// <returns>False if triangulation failed</returns>
	bool TryGetLowerTriangularDecomposition( out TMatrix lowerDecomposition, out TMatrix upperDecomposition );
}

public interface IMatrixVectorTransformation<TMatrix, TVector>
	where TMatrix :
		unmanaged, IMatrixVectorTransformation<TMatrix, TVector>
	where TVector :
		unmanaged {
	/// <returns><c>null</c> if the transform can't be resolved.</returns>
	TVector? TransformWorld( in TVector l );
	/// <returns><c>null</c> if the transform can't be resolved.</returns>
	TVector TransformNormal( in TVector l );
}