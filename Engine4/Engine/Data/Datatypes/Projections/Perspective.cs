using System.Numerics;
using Engine.Rendering;

namespace Engine.Data.Datatypes.Projections;
public class Perspective : MatrixProviderBase {

	public const float DEFAULT_NEAR = 0.0078125f;   //2^-7
	public const float DEFAULT_FAR = 2048;          //2^11

	private float _fov;
	private float _aspectRatio;
	private float _zNear;
	private float _zFar;

	public Perspective( float fov, float aspectRatio, float zNear = DEFAULT_NEAR, float zFar = DEFAULT_FAR ) {
		if ( zFar <= zNear )
			throw new ArgumentException( $"{nameof( zNear )} cannot be greater than or equal to {nameof( zFar )}!" );
		if ( aspectRatio <= 0 )
			throw new ArgumentOutOfRangeException( nameof( aspectRatio ) );
		if ( fov <= 0 || fov > 180 )
			throw new ArgumentOutOfRangeException( nameof( fov ) );
		this._fov = fov;
		this._aspectRatio = aspectRatio;
		this._zNear = zNear;
		this._zFar = zFar;
		SetChanged();
	}

	protected override void MatrixAccessed() => this.Matrix = Matrix4x4.CreatePerspectiveFieldOfView( this._fov / 180 * MathF.PI, this._aspectRatio, this._zNear, this._zFar );

	public float FOV {
		get => this._fov;
		set {
			if ( this._fov == value )
				return;
			if ( value > 180 )
				return;
			if ( value <= 0 )
				return;
			this._fov = value;
			SetChanged();
		}
	}

	public float AspectRatio {
		get => this._aspectRatio;
		set {
			if ( this._aspectRatio == value )
				return;
			if ( value <= 0 )
				return;
			this._aspectRatio = value;
			SetChanged();
		}
	}

	public float ZNear {
		get => this._zNear;
		set {
			if ( this._zNear == value )
				return;
			if ( this._zFar <= value )
				return;
			this._zNear = value;
			SetChanged();
		}
	}

	public float ZFar {
		get => this._zFar;
		set {
			if ( this._zFar == value )
				return;
			if ( value <= this._zNear )
				return;
			this._zFar = value;
			SetChanged();
		}
	}

	public class Dynamic : Perspective {
		private readonly Window _window;

		public Dynamic( Window window, float fov, float zNear = DEFAULT_NEAR, float zFar = DEFAULT_FAR ) : base(fov, window.AspectRatio, zNear, zFar) {
			this._window = window;
			window.WindowEvents.Resized += WindowResized;
		}

		private void WindowResized( int width, int height ) => this.AspectRatio = _window.AspectRatio;
	}
}
