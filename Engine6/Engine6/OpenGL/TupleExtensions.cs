global using Vector2i = (int X, int Y);
global using Vector2ui = (uint X, uint Y);

namespace OpenGL;

public static class TupleExtensions {
	public static Vector2ui ToUnsigned( this Vector2i vector ) => ((uint) vector.X, (uint) vector.Y);
	public static Vector2i ToSigned( this Vector2ui vector ) => ((int) vector.X, (int) vector.Y);
}
