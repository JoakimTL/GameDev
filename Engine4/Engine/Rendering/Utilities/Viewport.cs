using System.Runtime.CompilerServices;
using Engine.Data.Datatypes;

namespace Engine.Rendering.Utilities;
public static class Viewport {

	private static Vector2i _location, _size;

	public static Vector2i Location {
		get => _location;
		set {
			if ( value == _location )
				return;
			_location = value;
			SetInternal();
		}
	}

	public static Vector2i Size {
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
	public static void Set( Vector2i location, Vector2i size ) {
		if ( location == _location && size == _size )
			return;
		_location = location;
		_size = size;
		SetInternal();
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private static void SetInternal() => OpenGL.Gl.Viewport( _location.X, _location.Y, _size.X, _size.Y );

}
