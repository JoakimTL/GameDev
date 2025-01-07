using System.Numerics;

namespace Engine.Transforms;

public abstract class MatrixProviderBase<TScalar> : Identifiable, IMatrixProvider<TScalar>
	where TScalar :
		unmanaged, INumber<TScalar> {
	private Matrix4x4<TScalar> _matrix;
	private Matrix4x4<TScalar> _inverseMatrix;
	private bool _changed;
	public event Action<IMatrixProvider<TScalar>>? OnMatrixChanged;

	protected MatrixProviderBase() {
		this._matrix = Matrix4x4<TScalar>.MultiplicativeIdentity;
		this._inverseMatrix = Matrix4x4<TScalar>.MultiplicativeIdentity;
	}

	protected void SetChanged() {
		this._changed = true;
		OnMatrixChanged?.Invoke( this );
	}

	private void Update() {
		if (!this._changed)
			return;
		MatrixAccessed();
		this._changed = false;
	}

	protected abstract void MatrixAccessed();

	public Matrix4x4<TScalar> Matrix {
		get {
			Update();
			return this._matrix;
		}
		protected set {
			if (value == this._matrix)
				return;
			this._matrix = value;
			if (!this._matrix.TryGetInverse( out this._inverseMatrix ))
				this._inverseMatrix = Matrix4x4<TScalar>.MultiplicativeIdentity;
		}
	}

	public Matrix4x4<TScalar> InverseMatrix {
		get {
			Update();
			return this._inverseMatrix;
		}
	}
}
