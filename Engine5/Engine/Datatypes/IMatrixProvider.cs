using System.Numerics;

namespace Engine.Datatypes;
public interface IMatrixProvider {
	Matrix4x4 Matrix { get; }
	Matrix4x4 InverseMatrix { get; }
	event Action<IMatrixProvider> MatrixChanged;
}
