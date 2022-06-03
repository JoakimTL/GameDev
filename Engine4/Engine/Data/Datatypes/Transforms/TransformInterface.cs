using System.Numerics;

namespace Engine.Data.Datatypes.Transforms;
public class TransformInterface<T, R, S> : IMatrixProvider
	where T : unmanaged
	where R : unmanaged
	where S : unmanaged {

	private readonly TransformBase<T, R, S> _transform;

	public TransformInterface( TransformBase<T, R, S> transform ) {
		this._transform = transform ?? throw new ArgumentNullException( nameof( transform ) );
		MatrixChanged += ParentChanged;
	}

	public TransformReadonly<T, R, S>? Parent => this._transform.Parent?.Readonly;
	public T Translation {
		get => this._transform.Translation;
		set => this._transform.Translation = value;
	}
	public T GlobalTranslation => this._transform.GlobalTranslation;
	public R Rotation {
		get => this._transform.Rotation;
		set => this._transform.Rotation = value;
	}
	public R GlobalRotation => this._transform.GlobalRotation;
	public S Scale {
		get => this._transform.Scale;
		set => this._transform.Scale = value;
	}
	public S GlobalScale => this._transform.GlobalScale;

	public Matrix4x4 Matrix => this._transform.Matrix;

	public Matrix4x4 InverseMatrix => this._transform.InverseMatrix;

	public event Action<IMatrixProvider> MatrixChanged;

	private void ParentChanged( IMatrixProvider obj ) => MatrixChanged?.Invoke( obj );
}
