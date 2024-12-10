using System.Numerics;

namespace Engine.Transforms;

public interface IMatrixProvider<TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	Matrix4x4<TScalar> Matrix { get; }
	Matrix4x4<TScalar> InverseMatrix { get; }
	event Action<IMatrixProvider<TScalar>>? OnMatrixChanged;
}
