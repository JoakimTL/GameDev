using System.Numerics;

namespace Engine.Data;
public interface IMatrixProvider<TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	Matrix4x4<TScalar> Matrix { get; }
	Matrix4x4<TScalar> InverseMatrix { get; }
	event Action<IMatrixProvider<TScalar>> MatrixChanged;
}
