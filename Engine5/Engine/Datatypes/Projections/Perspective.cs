﻿using System.Numerics;
using Engine.Rendering;

namespace Engine.Datatypes.Projections;
public class Perspective : MatrixProviderBase
{

	public const float DEFAULT_NEAR = 0.0078125f;   //2^-7
	public const float DEFAULT_FAR = 2048;          //2^11

	private float _fov;
	private float _aspectRatio;
	private float _zNear;
	private float _zFar;

	public Perspective(float fov, float aspectRatio, float zNear = DEFAULT_NEAR, float zFar = DEFAULT_FAR)
	{
		if (zFar <= zNear)
			throw new ArgumentOutOfRangeException( $"{nameof(zNear)} cannot be greater than or equal to {nameof(zFar)}!");
		if (aspectRatio <= 0)
			throw new ArgumentOutOfRangeException(nameof(aspectRatio));
		if (fov <= 0 || fov >= 180)
			throw new ArgumentOutOfRangeException(nameof(fov));
		_fov = fov;
		_aspectRatio = aspectRatio;
		_zNear = zNear;
		_zFar = zFar;
		SetChanged();
	}

	protected override void MatrixAccessed() => Matrix = Matrix4x4.CreatePerspectiveFieldOfView(_fov / 180 * MathF.PI, _aspectRatio, _zNear, _zFar);

	public float FOV
	{
		get => _fov;
		set
		{
			if (_fov == value)
				return;
			if ( value > 180 ) {
				this.LogWarning( $"FOV cannot be greater than 180 degrees! (Value: {value})" );
				return;
			}
			if ( value <= 0 ) {
				this.LogWarning( $"FOV cannot be less than or equal to 0 degrees! (Value: {value})" );
				return;
			}
			_fov = value;
			SetChanged();
		}
	}

	public float AspectRatio
	{
		get => _aspectRatio;
		set
		{
			if (_aspectRatio == value)
				return;
			if ( value <= 0 ) {
				this.LogWarning( $"Aspect ratio cannot be less than or equal to 0! (Value: {value})" );
				return;
			}
			_aspectRatio = value;
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
			if ( _zFar <= value ) {
				this.LogWarning( $"ZNear cannot be greater than or equal to ZFar! (ZNear: {value}, ZFar: {_zFar})" );
				return;
			}
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
			if ( value <= _zNear ) {
				this.LogWarning( $"ZFar cannot be less than or equal to ZNear! (ZNear: {_zNear}, ZFar: {value})" );
				return;
			}
			_zFar = value;
			SetChanged();
		}
	}

	public class Dynamic : Perspective, IDisposable
	{
		private readonly Window _window;

		public Dynamic(Window window, float fov, float zNear = DEFAULT_NEAR, float zFar = DEFAULT_FAR) : base(fov, window.AspectRatio, zNear, zFar)
		{
			_window = window;
			_window.Resized += WindowResized;
		}

        public void Dispose()
        {
			_window.Resized -= WindowResized;
			GC.SuppressFinalize(this);
        }

        private void WindowResized(Window window) => AspectRatio = window.AspectRatio;
    }
}