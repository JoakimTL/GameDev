using Engine.Datatypes.Vectors;
using System.Runtime.CompilerServices;

namespace Engine.Rendering.OGL;
public sealed class Viewport {

	private Vector2i _location, _size;

	public Vector2i Location {
		get => _location;
		set {
			if ( value == _location )
				return;
			_location = value;
			SetInternal();
		}
	}

	public Vector2i Size {
		get => _size;
		set {
			if ( value == _size )
				return;
			_size = value;
			SetInternal();
		}
	}

	/// <summary>
	/// Sets both the location and size at the same time, reducing the number of viewport calls
	/// </summary>
	public void Set( Vector2i location, Vector2i size ) {
		if ( location == _location && size == _size )
			return;
		_location = location;
		_size = size;
		SetInternal();
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private void SetInternal() => OpenGL.Gl.Viewport( _location.X, _location.Y, _size.X, _size.Y );
}
