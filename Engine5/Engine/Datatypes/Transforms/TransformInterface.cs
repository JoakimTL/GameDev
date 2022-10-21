using System.Numerics;

namespace Engine.Datatypes.Transforms;
public class TransformInterface<T, R, S> : IMatrixProvider
	where T : unmanaged
	where R : unmanaged
	where S : unmanaged {

	private readonly TransformBase<T, R, S> _transform;

	public TransformInterface( TransformBase<T, R, S> transform ) {
		_transform = transform ?? throw new ArgumentNullException( nameof( transform ) );
		MatrixChanged += ParentChanged;
	}

	public TransformReadonly<T, R, S>? Parent => _transform.Parent?.Readonly;
	public T Translation {
		get => _transform.Translation;
		set => _transform.Translation = value;
	}
	public T GlobalTranslation => _transform.GlobalTranslation;
	public R Rotation {
		get => _transform.Rotation;
		set => _transform.Rotation = value;
	}
	public R GlobalRotation => _transform.GlobalRotation;
	public S Scale {
		get => _transform.Scale;
		set => _transform.Scale = value;
	}
	public S GlobalScale => _transform.GlobalScale;

	public Matrix4x4 Matrix => _transform.Matrix;

	public Matrix4x4 InverseMatrix => _transform.InverseMatrix;

	public event Action<IMatrixProvider> MatrixChanged;

	private void ParentChanged( IMatrixProvider obj ) => MatrixChanged?.Invoke( obj );
}
