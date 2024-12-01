using OpenGL;
using System.Runtime.CompilerServices;

namespace Engine.Module.Render.Ogl.Services;

public sealed class ViewportStateService : Identifiable {

	private Vector2<int> _location;
	private Vector2<int> _size;

	public Vector2<int> Location {
		get => this._location;
		set {
			if (value == this._location)
				return;
			this._location = value;
			SetInternal();
		}
	}

	public Vector2<int> Size {
		get => this._size;
		set {
			if (value.IsNegativeOrZero())
				throw new ArgumentOutOfRangeException( nameof( value ), value, "Viewport size must be positive" );
			if (value == this._size)
				return;
			this._size = value;
			SetInternal();
		}
	}

	/// <summary>
	/// Sets both the location and size at the same time, reducing the number of viewport calls
	/// </summary>
	public void Set( Vector2<int> location, Vector2<int> size ) {
		if (size == 0)
			size = 1;
		if (size.IsNegativeOrZero())
			throw new ArgumentOutOfRangeException( nameof( size ), size, "Viewport size must be positive" );
		if (location == this._location && size == this._size)
			return;
		this._location = location;
		this._size = size;
		SetInternal();
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private void SetInternal() => Gl.Viewport( this._location.X, this._location.Y, this._size.X, this._size.Y );
}
