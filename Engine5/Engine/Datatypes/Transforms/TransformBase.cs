using System.Numerics;

namespace Engine.Datatypes.Transforms;
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

	protected override string UniqueNameTag => $"{GlobalTranslation:N2},{GlobalRotation:N2},{GlobalScale:N2}";

	public TransformBase() {
		_parent = null;
		_translation = default;
		_rotation = default;
		_scale = default;
		_gTranslation = _translation;
		_gRotation = _rotation;
		_gScale = _scale;
		_readonly = new( this );
		_interface = new( this );
		SetChanged();
	}

	public TransformBase<T, R, S>? Parent {
		get => _parent;
		set {
			if ( ReferenceEquals( _parent, value ) )
				return;
			if ( _parent is not null )
				_parent.MatrixChanged -= ParentMatrixChanged;
			_parent = value;
			if ( _parent is not null )
				_parent.MatrixChanged += ParentMatrixChanged;
			SetChanged();
		}
	}

	public void SetParent( TransformBase<T, R, S>? parent, bool adjustForFrameOfReference ) {
		if ( ReferenceEquals( _parent, parent ) && adjustForFrameOfReference == _adjustedForFrameOfReference )
			return;
		if ( _parent is not null )
			_parent.MatrixChanged -= ParentMatrixChanged;
		if (_adjustedForFrameOfReference )
			RevertAdjustment( _parent );
		_parent = parent;
		if ( adjustForFrameOfReference )
			Adjust( _parent );
		_adjustedForFrameOfReference = adjustForFrameOfReference;
		if ( _parent is not null )
			_parent.MatrixChanged += ParentMatrixChanged;
		SetChanged();
	}

	public TransformReadonly<T, R, S> Readonly => _readonly;
	public TransformInterface<T, R, S> Interface => _interface;

	private void ParentMatrixChanged( IMatrixProvider obj ) => SetChanged();

	public T Translation {
		get => _translation;
		set {
			if ( _translation.Equals( value ) )
				return;
			_translation = value;
			SetChanged();
		}
	}

	public T GlobalTranslation {
		get {
			MatrixAccessed();
			return _gTranslation;
		}
	}

	public R Rotation {
		get => _rotation;
		set {
			if ( _rotation.Equals( value ) )
				return;
			_rotation = value;
			SetChanged();
		}
	}

	public R GlobalRotation {
		get {
			MatrixAccessed();
			return _gRotation;
		}
	}

	public S Scale {
		get => _scale;
		set {
			if ( _scale.Equals( value ) )
				return;
			_scale = value;
			SetChanged();
		}
	}

	public S GlobalScale {
		get {
			MatrixAccessed();
			return _gScale;
		}
	}

	public TransformData<T, R, S> Data => new( Translation, Rotation, Scale );

	public void SetData( TransformData<T, R, S> data ) {
		Translation = data.Translation;
		Rotation = data.Rotation;
		Scale = data.Scale;
	}

	protected override void MatrixAccessed() {
		Matrix = Parent is not null ? GetLocalMatrix() * Parent.Matrix : GetLocalMatrix();
		_gTranslation = GetGlobalTranslation();
		_gRotation = GetGlobalRotation();
		_gScale = GetGlobalScale();
	}

	protected abstract void Adjust( TransformBase<T, R, S>? parent );
	protected abstract void RevertAdjustment( TransformBase<T, R, S>? parent );
	protected abstract TransformData<T, R, S> GetInterpolated( TransformBase<T, R, S> other, float interpolation );
	protected abstract Matrix4x4 GetLocalMatrix();
	protected abstract T GetGlobalTranslation();
	protected abstract R GetGlobalRotation();
	protected abstract S GetGlobalScale();

}
