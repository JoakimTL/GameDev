namespace Engine;

public interface IMatrixEigenvector<TMatrix, TVector>
    where TMatrix :
        unmanaged, IMatrixEigenvector<TMatrix, TVector>
    where TVector :
        unmanaged
{
    /// <summary>
    /// Returns the eigenvectors of the matrix. It is recommended to use <see cref="GetEigenVectors(Span{TVector})"/> instead if you know how many eigenvectors the matrix can produce.
    /// </summary>
    TVector[] GetEigenvectors();
    /// <summary>
    /// Fills the span with the eigenvectors of the matrix. If there are more eigenvectors than the span can hold, the rest are ignored.
    /// </summary>
    /// <param name="result"></param>
    /// <returns>The number of eigenvectors found.</returns>
    uint GetEigenVectors(Span<TVector> result);
}