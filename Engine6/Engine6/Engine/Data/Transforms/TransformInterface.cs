using System.Numerics;

namespace Engine.Data.Transforms;
public class TransformInterface<TScalar, TTranslation, TRotation, TScale> : IMatrixProvider<TScalar>
	where TScalar : unmanaged, INumber<TScalar>
	where TTranslation : unmanaged
	where TRotation : unmanaged
	where TScale : unmanaged {

	private readonly TransformBase<TScalar, TTranslation, TRotation, TScale> _transform;

	public TransformInterface( TransformBase<TScalar, TTranslation, TRotation, TScale> transform ) {
		this._transform = transform ?? throw new ArgumentNullException( nameof( transform ) );
		MatrixChanged += ParentChanged;
	}

	public TransformReadonly<TScalar, TTranslation, TRotation, TScale>? Parent => this._transform.Parent?.Readonly;
	public TTranslation Translation {
		get => this._transform.Translation;
		set => this._transform.Translation = value;
	}
	public TTranslation GlobalTranslation => this._transform.GlobalTranslation;
	public TRotation Rotation {
		get => this._transform.Rotation;
		set => this._transform.Rotation = value;
	}
	public TRotation GlobalRotation => this._transform.GlobalRotation;
	public TScale Scale {
		get => this._transform.Scale;
		set => this._transform.Scale = value;
	}
	public TScale GlobalScale => this._transform.GlobalScale;

	public Matrix4x4<TScalar> Matrix => this._transform.Matrix;

	public Matrix4x4<TScalar> InverseMatrix => this._transform.InverseMatrix;

	public event Action<IMatrixProvider<TScalar>> MatrixChanged;

	private void ParentChanged( IMatrixProvider<TScalar> obj ) => MatrixChanged?.Invoke( obj );
}
