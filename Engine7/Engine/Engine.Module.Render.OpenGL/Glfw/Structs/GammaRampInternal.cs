using System.Runtime.InteropServices;

namespace Engine.Module.Render.OpenGL.Glfw.Structs;

[StructLayout( LayoutKind.Sequential )]
public readonly struct GammaRampInternal {
	public readonly nint Red;
	public readonly nint Green;
	public readonly nint Blue;
	public readonly int Size;
}
