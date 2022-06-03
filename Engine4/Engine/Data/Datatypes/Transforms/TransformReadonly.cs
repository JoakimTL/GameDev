using System.Numerics;

namespace Engine.Data.Datatypes.Transforms;
public class TransformReadonly<T, R, S> : IMatrixProvider
	where T : unmanaged
	where R : unmanaged
	where S : unmanaged {

	private readonly TransformBase<T, R, S> _transform;

	public TransformReadonly( TransformBase<T, R, S> transform ) {
		this._transform = transform ?? throw new ArgumentNullException( nameof( transform ) );
		MatrixChanged += ParentChanged;
	}

	public TransformReadonly<T, R, S>? Parent => this._transform.Parent?.Readonly;
	public T Translation => this._transform.Translation;
	public T GlobalTranslation => this._transform.GlobalTranslation;
	public R Rotation => this._transform.Rotation;
	public R GlobalRotation => this._transform.GlobalRotation;
	public S Scale => this._transform.Scale;
	public S GlobalScale => this._transform.GlobalScale;

	public Matrix4x4 Matrix => this._transform.Matrix;

	public Matrix4x4 InverseMatrix => this._transform.InverseMatrix;

	public event Action<IMatrixProvider> MatrixChanged;

	private void ParentChanged( IMatrixProvider obj ) => MatrixChanged?.Invoke( obj );
}
