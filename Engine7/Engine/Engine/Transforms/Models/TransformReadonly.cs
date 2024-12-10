using System.Numerics;

namespace Engine.Transforms.Models;

public class TransformReadonly<TScalar, TTranslation, TRotation, TScale> : IMatrixProvider<TScalar>
	where TScalar : unmanaged, INumber<TScalar>
	where TTranslation : unmanaged
	where TRotation : unmanaged
	where TScale : unmanaged {

	private readonly TransformBase<TScalar, TTranslation, TRotation, TScale> _transform;

	public TransformReadonly( TransformBase<TScalar, TTranslation, TRotation, TScale> transform ) {
		this._transform = transform ?? throw new ArgumentNullException( nameof( transform ) );
		_transform.OnMatrixChanged += TransformChangedPropagation;
	}

	private void TransformChangedPropagation( IMatrixProvider<TScalar> provider ) => OnMatrixChanged?.Invoke( provider );

	public TransformReadonly<TScalar, TTranslation, TRotation, TScale>? Parent => this._transform.Parent?.Readonly;
	public TTranslation Translation => this._transform.Translation;
	public TTranslation GlobalTranslation => this._transform.GlobalTranslation;
	public TRotation Rotation => this._transform.Rotation;
	public TRotation GlobalRotation => this._transform.GlobalRotation;
	public TScale Scale => this._transform.Scale;
	public TScale GlobalScale => this._transform.GlobalScale;

	public Matrix4x4<TScalar> Matrix => this._transform.Matrix;

	public Matrix4x4<TScalar> InverseMatrix => this._transform.InverseMatrix;

	public event Action<IMatrixProvider<TScalar>>? OnMatrixChanged;
}
