using Engine.Math.NewFolder;
using Engine.Math.NewFolder.Operations;
using OpenGL;
using System.Runtime.CompilerServices;

namespace Engine.Modules.Rendering.Ogl.Services;

public sealed class ViewportStateService : Identifiable {

	private Vector2<int> _location;
	private Vector2<int> _size;

	public Vector2<int> Location {
		get => _location;
		set {
			if (value == _location) {
				this.LogWarning( "Viewport location is already set to the given value" );
				return;
			}
			_location = value;
			SetInternal();
		}
	}

	public Vector2<int> Size {
		get => _size;
		set {
			if (value.IsNegativeOrZero())
				throw new ArgumentOutOfRangeException( nameof( value ), value, "Viewport size must be positive" );
			if (value == _size) {
				this.LogWarning( "Viewport size is already set to the given value" );
				return;
			}
			_size = value;
			SetInternal();
		}
	}

	/// <summary>
	/// Sets both the location and size at the same time, reducing the number of viewport calls
	/// </summary>
	public void Set( Vector2<int> location, Vector2<int> size ) {
		if (size.IsNegativeOrZero())
			throw new ArgumentOutOfRangeException( nameof( size ), size, "Viewport size must be positive" );
		if (location == _location && size == _size) {
			this.LogWarning( "Viewport location and size are already set to the given values" );
			return;
		}
		_location = location;
		_size = size;
		SetInternal();
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private void SetInternal() => Gl.Viewport( _location.X, _location.Y, _size.X, _size.Y );
}
