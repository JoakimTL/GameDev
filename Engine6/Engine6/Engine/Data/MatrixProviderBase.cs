using System.Numerics;

namespace Engine.Data;

public abstract class MatrixProviderBase<TScalar> : Identifiable, IMatrixProvider <TScalar>
	where TScalar :
		unmanaged, INumber<TScalar>{
	private Matrix4x4<TScalar> _matrix;
	private Matrix4x4<TScalar> _inverseMatrix;
	private bool _changed;
	public event Action<IMatrixProvider<TScalar>>? MatrixChanged;

	protected MatrixProviderBase() {
		this._matrix = Matrix4x4<TScalar>.MultiplicativeIdentity;
		this._inverseMatrix = Matrix4x4<TScalar>.MultiplicativeIdentity;
	}

	protected void SetChanged() {
		this._changed = true;
		MatrixChanged?.Invoke( this );
	}

	protected abstract void MatrixAccessed();

	public Matrix4x4<TScalar> Matrix {
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
			if ( !this._matrix.TryGetInverse( out this._inverseMatrix ) )
				this._inverseMatrix = Matrix4x4<TScalar>.MultiplicativeIdentity;
		}
	}

	public Matrix4x4<TScalar> InverseMatrix {
		get {
			if ( this._changed ) {
				MatrixAccessed();
				this._changed = false;
			}
			return this._inverseMatrix;
		}
	}
}
