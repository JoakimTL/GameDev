using System.Numerics;

namespace Engine.Transforms.Models;

public abstract class TransformAccessBase<TScalar, TTranslation, TRotation, TScale> : IMatrixProvider<TScalar>
	where TScalar : unmanaged, INumber<TScalar>
	where TTranslation : unmanaged
	where TRotation : unmanaged
	where TScale : unmanaged {
	protected readonly TransformBase<TScalar, TTranslation, TRotation, TScale> _transform;

	public TransformAccessBase( TransformBase<TScalar, TTranslation, TRotation, TScale> transform ) {
		this._transform = transform ?? throw new ArgumentNullException( nameof( transform ) );
		_transform.OnMatrixChanged += TransformChangedPropagation;
	}
	private void TransformChangedPropagation( IMatrixProvider<TScalar> provider ) => OnMatrixChanged?.Invoke( provider );
	public event Action<IMatrixProvider<TScalar>>? OnMatrixChanged;

	public Matrix4x4<TScalar> Matrix => this._transform.Matrix;
	public Matrix4x4<TScalar> InverseMatrix => this._transform.InverseMatrix;
}