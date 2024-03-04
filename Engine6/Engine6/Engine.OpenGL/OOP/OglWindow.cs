using Engine.Data;
using System.Numerics;

namespace Engine.OpenGL.OOP;

public sealed class OglWindow {
	private readonly nint _windowHandle;

	internal OglWindow( nint handle ) {
		this._windowHandle = handle;
	}

	public Vector2i Size { get; private set; }



}