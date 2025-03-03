﻿namespace Engine.Transforms.Camera;

public class Orthographic : MatrixProviderBase<float> {
	private Vector2<float> _size;
	private float _zNear;
	private float _zFar;

	public Orthographic( Vector2<float> size, float zNear, float zFar ) {
		if (size.X <= 0 || size.Y <= 0)
			throw new ArgumentOutOfRangeException( nameof( size ) );
		if (zFar <= zNear)
			throw new ArgumentOutOfRangeException( $"{nameof( zNear )} cannot be greater than or equal to {nameof( zFar )}!" );
		this._size = size;
		this._zNear = zNear;
		this._zFar = zFar;
		SetChanged();
	}

	protected override void MatrixAccessed() => this.Matrix = Engine.Matrix.Create4x4.Orthographic( -this._size.X, this._size.Y, this._size.X, -this._size.Y, this._zNear, this._zFar );

	public Vector2<float> Size {
		get => this._size;
		set {
			if (this._size == value)
				return;
			if (value.X <= 0 || value.Y <= 0)
				return;
			this._size = value;
			SetChanged();
		}
	}

	public float ZNear {
		get => this._zNear;
		set {
			if (this._zNear == value)
				return;
			if (this._zFar <= value)
				return;
			this._zNear = value;
			SetChanged();
		}
	}

	public float ZFar {
		get => this._zFar;
		set {
			if (this._zFar == value)
				return;
			if (value <= this._zNear)
				return;
			this._zFar = value;
			SetChanged();
		}
	}

	public class Dynamic : Orthographic, IDisposable {

		private readonly IResizableAspectRatioSurface<int, float> _surface;
		private Vector2<float> _scale;

		public Dynamic( IResizableAspectRatioSurface<int, float> surface, Vector2<float> scale, float zNear, float zFar ) : base( surface.AspectRatioVector.MultiplyEntrywise( scale ), zNear, zFar ) {
			this._surface = surface;
			this._scale = scale;
			this._surface.OnResized += WindowResized;
		}

		private void WindowResized( IResizableSurface<int> surface ) => this.Size = this._surface.AspectRatioVector.MultiplyEntrywise( this._scale );

		public void Dispose() {
			this._surface.OnResized -= WindowResized;
			GC.SuppressFinalize( this );
		}

		public Vector2<float> Scale {
			get => this._scale;
			set {
				if (this._scale == value)
					return;
				if (value.X <= 0 || value.Y <= 0)
					return;
				this._scale = value;
				this.Size = this._surface.AspectRatioVector.MultiplyEntrywise( this._scale );
			}
		}
	}
}
