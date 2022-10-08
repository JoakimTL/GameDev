using System.Numerics;
using Engine.Rendering;

namespace Engine.Data.Datatypes.Projections;
public class Orthographic : MatrixProviderBase {
	private Vector2 _size;
	private float _zNear;
	private float _zFar;

	public Orthographic( Vector2 size, float zNear, float zFar ) {
		if ( size.X <= 0 || size.Y <= 0 )
			throw new ArgumentOutOfRangeException( nameof( size ) );
		if ( zFar <= zNear )
			throw new ArgumentException( $"{nameof( zNear )} cannot be greater than or equal to {nameof( zFar )}!" );
		this._size = size;
		this._zNear = zNear;
		this._zFar = zFar;
		SetChanged();
	}

	protected override void MatrixAccessed() => this.Matrix = Matrix4x4.CreateOrthographic( this._size.X, this._size.Y, this._zNear, this._zFar );

	public Vector2 Size {
		get => this._size;
		set {
			if ( this._size == value )
				return;
			if ( value.X <= 0 || value.Y <= 0 )
				return;
			this._size = value;
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

	public class Dynamic : Orthographic {

		private readonly Window _window;
		private Vector2 _scale;

		public Dynamic( Window window, Vector2 scale, float zNear, float zFar ) : base( window.AspectRatioVector * scale, zNear, zFar ) {
			this._window = window;
			this._scale = scale;
			window.WindowEvents.Resized += WindowResized;
		}

		private void WindowResized( int width, int height ) => this.Size = _window.AspectRatioVector * this._scale;

		public Vector2 Scale {
			get => this._scale;
			set {
				if ( this._scale == value )
					return;
				if ( value.X <= 0 || value.Y <= 0 )
					return;
				this._scale = value;
				this.Size = _window.AspectRatioVector * this._scale;
			}
		}
	}
}
