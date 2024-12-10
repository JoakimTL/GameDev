using System.Numerics;

namespace Engine.Transforms.Models;

public abstract class TransformBase<TScalar, TTranslation, TRotation, TScale> : MatrixProviderBase<TScalar>
	where TScalar : unmanaged, INumber<TScalar>
	where TTranslation : unmanaged
	where TRotation : unmanaged
	where TScale : unmanaged {

	private bool _adjustedForFrameOfReference;
	private TTranslation _translation;
	private TRotation _rotation;
	private TScale _scale;
	private TTranslation _gTranslation;
	private TRotation _gRotation;
	private TScale _gScale;
	private TransformBase<TScalar, TTranslation, TRotation, TScale>? _parent;
	private readonly TransformReadonly<TScalar, TTranslation, TRotation, TScale> _readonly;
	private readonly TransformInterface<TScalar, TTranslation, TRotation, TScale> _interface;

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

	public TransformBase<TScalar, TTranslation, TRotation, TScale>? Parent
		=> this._parent;

	/// <param name="adjustForFrameOfReference">If true the transform will retian it's current world space location while still being a child of the new parent.</param>
	public void SetParent( TransformBase<TScalar, TTranslation, TRotation, TScale>? parent, bool adjustForFrameOfReference ) {
		if (ReferenceEquals( this._parent, parent ) && adjustForFrameOfReference == this._adjustedForFrameOfReference)
			return;
		if (this._parent is not null)
			this._parent.OnMatrixChanged -= ParentMatrixChanged;
		if (this._adjustedForFrameOfReference)
			RevertAdjustment( this._parent );
		this._parent = parent;
		if (adjustForFrameOfReference)
			Adjust( this._parent );
		this._adjustedForFrameOfReference = adjustForFrameOfReference && this._parent is not null;
		if (this._parent is not null)
			this._parent.OnMatrixChanged += ParentMatrixChanged;
		SetChanged();
	}

	public TransformReadonly<TScalar, TTranslation, TRotation, TScale> Readonly => this._readonly;
	public TransformInterface<TScalar, TTranslation, TRotation, TScale> Interface => this._interface;

	private void ParentMatrixChanged( IMatrixProvider<TScalar> obj ) => SetChanged();

	public bool Adjusted => this._adjustedForFrameOfReference;

	public TTranslation Translation {
		get => this._translation;
		set {
			if (this._translation.Equals( value ))
				return;
			this._translation = value;
			SetChanged();
		}
	}

	public TTranslation GlobalTranslation {
		get {
			MatrixAccessed();
			return this._gTranslation;
		}
	}

	public TRotation Rotation {
		get => this._rotation;
		set {
			if (this._rotation.Equals( value ))
				return;
			this._rotation = value;
			SetChanged();
		}
	}

	public TRotation GlobalRotation {
		get {
			MatrixAccessed();
			return this._gRotation;
		}
	}

	public TScale Scale {
		get => this._scale;
		set {
			if (this._scale.Equals( value ))
				return;
			this._scale = value;
			SetChanged();
		}
	}

	public TScale GlobalScale {
		get {
			MatrixAccessed();
			return this._gScale;
		}
	}

	public TransformData<TTranslation, TRotation, TScale> Data => new( this.Translation, this.Rotation, this.Scale );
	public TransformData<TTranslation, TRotation, TScale> GlobalData => new( this.GlobalTranslation, this.GlobalRotation, this.GlobalScale );

	public void SetData( TransformData<TTranslation, TRotation, TScale> data ) {
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

	protected abstract void Adjust( TransformBase<TScalar, TTranslation, TRotation, TScale>? parent );
	protected abstract void RevertAdjustment( TransformBase<TScalar, TTranslation, TRotation, TScale>? parent );
	protected abstract TransformData<TTranslation, TRotation, TScale> GetInterpolated( TransformBase<TScalar, TTranslation, TRotation, TScale> other, TScalar interpolation );
	protected abstract Matrix4x4<TScalar> GetLocalMatrix();
	protected abstract TTranslation GetGlobalTranslation();
	protected abstract TRotation GetGlobalRotation();
	protected abstract TScale GetGlobalScale();

}
