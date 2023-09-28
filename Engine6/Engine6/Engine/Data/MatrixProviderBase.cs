using System.Numerics;

namespace Engine.Data;

public abstract class MatrixProviderBase : Identifiable, IMatrixProvider {
	private Matrix4x4 _matrix;
	private Matrix4x4 _inverseMatrix;
	private bool _changed;
	public event Action<IMatrixProvider>? MatrixChanged;

	protected MatrixProviderBase() {
		this._matrix = Matrix4x4.Identity;
		this._inverseMatrix = Matrix4x4.Identity;
	}

	protected void SetChanged() {
		this._changed = true;
		MatrixChanged?.Invoke( this );
	}

	protected abstract void MatrixAccessed();

	public Matrix4x4 Matrix {
		get {
			if ( this._changed ) {
				MatrixAccessed();
				this._changed = false;
			}
			return this._matrix;
		}
		protected set {
			if ( value == this._matrix )
				return;
			this._matrix = value;
			if ( !Matrix4x4.Invert( this._matrix, out this._inverseMatrix ) )
				this._inverseMatrix = Matrix4x4.Identity;
		}
	}

	public Matrix4x4 InverseMatrix {
		get {
			if ( this._changed ) {
				MatrixAccessed();
				this._changed = false;
			}
			return this._inverseMatrix;
		}
	}
}
