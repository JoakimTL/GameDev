using System.Numerics;

namespace Engine.Data.Transforms;
public abstract class TransformBase<T, R, S> : MatrixProviderBase
	where T : unmanaged
	where R : unmanaged
	where S : unmanaged {

	private bool _adjustedForFrameOfReference;
	private T _translation;
	private R _rotation;
	private S _scale;
	private T _gTranslation;
	private R _gRotation;
	private S _gScale;
	private TransformBase<T, R, S>? _parent;
	private readonly TransformReadonly<T, R, S> _readonly;
	private readonly TransformInterface<T, R, S> _interface;

	//protected override string UniqueNameTag => $"{GlobalTranslation:N2},{GlobalRotation:N2},{GlobalScale:N2}";

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

	public TransformBase<T, R, S>? Parent
		=> this._parent;

	/// <param name="adjustForFrameOfReference">If true the transform will retian it's current world space location while still being a child of the new parent.</param>
	public void SetParent( TransformBase<T, R, S>? parent, bool adjustForFrameOfReference ) {
		if ( ReferenceEquals( this._parent, parent ) && adjustForFrameOfReference == this._adjustedForFrameOfReference )
			return;
		if ( this._parent is not null )
			this._parent.MatrixChanged -= ParentMatrixChanged;
		if ( this._adjustedForFrameOfReference )
			RevertAdjustment( this._parent );
		this._parent = parent;
		if ( adjustForFrameOfReference )
			Adjust( this._parent );
		this._adjustedForFrameOfReference = adjustForFrameOfReference && this._parent is not null;
		if ( this._parent is not null )
			this._parent.MatrixChanged += ParentMatrixChanged;
		SetChanged();
	}

	public TransformReadonly<T, R, S> Readonly => this._readonly;
	public TransformInterface<T, R, S> Interface => this._interface;

	private void ParentMatrixChanged( IMatrixProvider obj ) => SetChanged();

	public bool Adjusted => this._adjustedForFrameOfReference;

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
	public TransformData<T, R, S> GlobalData => new( this.GlobalTranslation, this.GlobalRotation, this.GlobalScale );

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

	protected abstract void Adjust( TransformBase<T, R, S>? parent );
	protected abstract void RevertAdjustment( TransformBase<T, R, S>? parent );
	protected abstract TransformData<T, R, S> GetInterpolated( TransformBase<T, R, S> other, float interpolation );
	protected abstract Matrix4x4 GetLocalMatrix();
	protected abstract T GetGlobalTranslation();
	protected abstract R GetGlobalRotation();
	protected abstract S GetGlobalScale();

}
