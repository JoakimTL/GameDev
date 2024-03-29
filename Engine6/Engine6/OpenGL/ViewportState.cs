﻿using System.Runtime.CompilerServices;

namespace OpenGL;

public sealed class ViewportState {

	private Vector2i _location;
	private Vector2ui _size;

	public Vector2i Location {
		get => _location;
		set {
			if (value == _location)
				return;
			_location = value;
			SetInternal();
		}
	}

	public Vector2ui Size {
		get => _size;
		set {
			if (value == _size)
				return;
			_size = value;
			SetInternal();
		}
	}

	/// <summary>
	/// Sets both the location and size at the same time, reducing the number of viewport calls
	/// </summary>
	public void Set( Vector2i location, Vector2ui size ) {
		if (location == _location && size == _size)
			return;
		_location = location;
		_size = size;
		SetInternal();
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private void SetInternal() => OpenGL.Gl.Viewport( _location.X, _location.Y, (int) _size.X, (int) _size.Y );
}
