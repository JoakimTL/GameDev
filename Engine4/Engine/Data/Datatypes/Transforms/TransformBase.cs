using System.Numerics;
using System.Runtime.InteropServices;

namespace Engine.Data.Datatypes.Transforms;
public abstract class TransformBase<T, R, S> : MatrixProviderBase
	where T : unmanaged
	where R : unmanaged
	where S : unmanaged {

	private T _translation;
	private R _rotation;
	private S _scale;
	private T _gTranslation;
	private R _gRotation;
	private S _gScale;
	private TransformBase<T, R, S>? _parent;
	private readonly TransformReadonly<T, R, S> _readonly;
	private readonly TransformInterface<T, R, S> _interface;

	public TransformBase() {
		this._parent = null;
		this._translation = default;
		this._rotation = default;
		this._scale = default;
		this._gTranslation = this._translation;
		this._gRotation = this._rotation;
		this._gScale = this._scale;
		this._readonly = new( this );
		this._interface = new( this );
		SetChanged();
	}

	public TransformBase<T, R, S>? Parent {
		get => this._parent;
		set {
			if ( ReferenceEquals( this._parent, value ) )
				return;
			if ( this._parent is not null )
				this._parent.MatrixChanged -= ParentMatrixChanged;
			this._parent = value;
			if ( this._parent is not null )
				this._parent.MatrixChanged += ParentMatrixChanged;
			SetChanged();
		}
	}

	public TransformReadonly<T, R, S> Readonly => this._readonly;
	public TransformInterface<T, R, S> Interface => this._interface;

	private void ParentMatrixChanged( IMatrixProvider obj ) => SetChanged();

	public T Translation {
		get => this._translation;
		set {
			if ( this._translation.Equals( value ) )
				return;
			this._translation = value;
			SetChanged();
		}
	}

	public T GlobalTranslation {
		get {
			MatrixAccessed();
			return this._gTranslation;
		}
	}

	public R Rotation {
		get => this._rotation;
		set {
			if ( this._rotation.Equals( value ) )
				return;
			this._rotation = value;
			SetChanged();
		}
	}

	public R GlobalRotation {
		get {
			MatrixAccessed();
			return this._gRotation;
		}
	}

	public S Scale {
		get => this._scale;
		set {
			if ( this._scale.Equals( value ) )
				return;
			this._scale = value;
			SetChanged();
		}
	}

	public S GlobalScale {
		get {
			MatrixAccessed();
			return this._gScale;
		}
	}

	public TransformData<T, R, S> Data => new( this.Translation, this.Rotation, this.Scale );

	public void SetData( TransformData<T, R, S> data ) {
		this.Translation = data.Translation;
		this.Rotation = data.Rotation;
		this.Scale = data.Scale;
	}

	protected override void MatrixAccessed() {
		this.Matrix = this.Parent is not null ? GetLocalMatrix() * this.Parent.Matrix : GetLocalMatrix();
		this._gTranslation = GetGlobalTranslation();
		this._gRotation = GetGlobalRotation();
		this._gScale = GetGlobalScale();
	}

	protected abstract TransformData<T, R, S> GetInterpolated( TransformBase<T, R, S> other, float interpolation );
	protected abstract Matrix4x4 GetLocalMatrix();
	protected abstract T GetGlobalTranslation();
	protected abstract R GetGlobalRotation();
	protected abstract S GetGlobalScale();

}

[StructLayout(LayoutKind.Sequential)]
public readonly struct TransformData<T, R, S>
	where T : unmanaged
	where R : unmanaged
	where S : unmanaged {
	public readonly T Translation;
	public readonly R Rotation;
	public readonly S Scale;
	public TransformData(T translation, R rotation, S scale) {
		this.Translation = translation;
		this.Rotation = rotation;
		this.Scale = scale;
	}
}