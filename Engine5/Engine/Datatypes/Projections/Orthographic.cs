using System.Numerics;
using Engine.Rendering;

namespace Engine.Datatypes.Projections;
public class Orthographic : MatrixProviderBase
{
	private Vector2 _size;
	private float _zNear;
	private float _zFar;

	public Orthographic(Vector2 size, float zNear, float zFar)
	{
		if (size.X <= 0 || size.Y <= 0)
			throw new ArgumentOutOfRangeException(nameof(size));
		if (zFar <= zNear)
			throw new ArgumentException($"{nameof(zNear)} cannot be greater than or equal to {nameof(zFar)}!");
		_size = size;
		_zNear = zNear;
		_zFar = zFar;
		SetChanged();
	}

	protected override void MatrixAccessed() => Matrix = Matrix4x4.CreateOrthographic(_size.X, _size.Y, _zNear, _zFar);

	public Vector2 Size
	{
		get => _size;
		set
		{
			if (_size == value)
				return;
			if (value.X <= 0 || value.Y <= 0)
				return;
			_size = value;
			SetChanged();
		}
	}

	public float ZNear
	{
		get => _zNear;
		set
		{
			if (_zNear == value)
				return;
			if (_zFar <= value)
				return;
			_zNear = value;
			SetChanged();
		}
	}

	public float ZFar
	{
		get => _zFar;
		set
		{
			if (_zFar == value)
				return;
			if (value <= _zNear)
				return;
			_zFar = value;
			SetChanged();
		}
	}

	public class Dynamic : Orthographic, IDisposable
	{

		private readonly Window _window;
		private Vector2 _scale;

		public Dynamic(Window window, Vector2 scale, float zNear, float zFar) : base(window.AspectRatioVector * scale, zNear, zFar)
		{
			_window = window;
			_scale = scale;
			_window.Resized += WindowResized;
		}

		private void WindowResized(Window window) => Size = window.AspectRatioVector * _scale;

        public void Dispose()
        {
            _window.Resized -= WindowResized;
			GC.SuppressFinalize(this);
        }

        public Vector2 Scale
		{
			get => _scale;
			set
			{
				if (_scale == value)
					return;
				if (value.X <= 0 || value.Y <= 0)
					return;
				_scale = value;
				Size = _window.AspectRatioVector * _scale;
			}
		}
	}
}
