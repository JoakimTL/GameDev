namespace Engine.Transforms.Camera;

public class Orthographic : MatrixProviderBase<float> {
	private Vector2<float> _size;
	private float _zNear;
	private float _zFar;

	public Orthographic( Vector2<float> size, float zNear, float zFar ) {
		if (size.X <= 0 || size.Y <= 0)
			throw new ArgumentOutOfRangeException( nameof( size ) );
		if (zFar <= zNear)
			throw new ArgumentOutOfRangeException( $"{nameof( zNear )} cannot be greater than or equal to {nameof( zFar )}!" );
		_size = size;
		_zNear = zNear;
		_zFar = zFar;
		SetChanged();
	}

	protected override void MatrixAccessed() => Matrix = Engine.Matrix.Create4x4.Orthographic( -_size.X, -_size.Y, _size.X, _size.Y, _zNear, _zFar );

	public Vector2<float> Size {
		get => _size;
		set {
			if (_size == value)
				return;
			if (value.X <= 0 || value.Y <= 0)
				return;
			_size = value;
			SetChanged();
		}
	}

	public float ZNear {
		get => _zNear;
		set {
			if (_zNear == value)
				return;
			if (_zFar <= value)
				return;
			_zNear = value;
			SetChanged();
		}
	}

	public float ZFar {
		get => _zFar;
		set {
			if (_zFar == value)
				return;
			if (value <= _zNear)
				return;
			_zFar = value;
			SetChanged();
		}
	}

	public class Dynamic : Orthographic, IDisposable {

		private readonly IResizableSurface<int, float> _surface;
		private Vector2<float> _scale;

		public Dynamic( IResizableSurface<int, float> surface, Vector2<float> scale, float zNear, float zFar ) : base( surface.AspectRatioVector.MultiplyEntrywise( scale ), zNear, zFar ) {
			_surface = surface;
			_scale = scale;
			_surface.OnResized += WindowResized;
		}

		private void WindowResized( IResizableSurface<int, float> surface ) => Size = _surface.AspectRatioVector.MultiplyEntrywise( _scale );

		public void Dispose() {
			_surface.OnResized -= WindowResized;
			GC.SuppressFinalize( this );
		}

		public Vector2<float> Scale {
			get => _scale;
			set {
				if (_scale == value)
					return;
				if (value.X <= 0 || value.Y <= 0)
					return;
				_scale = value;
				Size = _surface.AspectRatioVector.MultiplyEntrywise( _scale );
			}
		}
	}
}
