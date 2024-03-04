using Engine.Data;
using OpenGL;
using System.Runtime.CompilerServices;

namespace Engine.OpenGL;

public sealed class ViewportState( ContextWarningLog warningLog ) {

	private ContextWarningLog _warningLog = warningLog;
	private Vector2i _location;
	private Vector2i _size;

	public Vector2i Location {
		get => _location;
		set {
			if (value == _location) {
				_warningLog.LogWarning( "Viewport location is already set to the given value" );
				return;
			}
			_location = value;
			SetInternal();
		}
	}

	public Vector2i Size {
		get => _size;
		set {
			if (Vector2i.NegativeOrZero(value))
				throw new ArgumentOutOfRangeException( nameof( value ), value, "Viewport size must be positive" );
			if (value == _size) {
				_warningLog.LogWarning( "Viewport size is already set to the given value" );
				return;
			}
			_size = value;
			SetInternal();
		}
	}

	/// <summary>
	/// Sets both the location and size at the same time, reducing the number of viewport calls
	/// </summary>
	public void Set( Vector2i location, Vector2i size ) {
		if (Vector2i.NegativeOrZero(size))
			throw new ArgumentOutOfRangeException( nameof( size ), size, "Viewport size must be positive" );
		if (location == _location && size == _size) {
			_warningLog.LogWarning( "Viewport location and size are already set to the given values" );
			return;
		}
		_location = location;
		_size = size;
		SetInternal();
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private void SetInternal() => Gl.Viewport( _location.X, _location.Y, _size.X, _size.Y );
}
