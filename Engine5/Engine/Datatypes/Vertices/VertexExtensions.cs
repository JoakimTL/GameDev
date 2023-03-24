using Engine.Datatypes.Colors;
using System.Numerics;

namespace Engine.Datatypes.Vertices;

public static class VertexExtensions
{
	public static Vertex2 SetTranslation(this Vertex2 v, Vector2 translation) => new(translation, v.UV, v.Color);
	public static Vertex2 SetUV(this Vertex2 v, Vector2 uv) => new(v.Translation, uv, v.Color);
	public static Vertex2 SetColor(this Vertex2 v, Vector4 color) => new(v.Translation, v.UV, color);
	public static Vertex2 SetColor(this Vertex2 v, Color16x4 color) => new(v.Translation, v.UV, color);
	public static Vertex3 SetTranslation(this Vertex3 v, Vector3 translation) => new(translation, v.UV, v.Normal, v.Color);
	public static Vertex3 SetUV(this Vertex3 v, Vector2 uv) => new(v.Translation, uv, v.Normal, v.Color);
	public static Vertex3 SetNormal(this Vertex3 v, Vector3 normal) => new(v.Translation, v.UV, normal, v.Color);
	public static Vertex3 SetColor(this Vertex3 v, Vector4 color) => new(v.Translation, v.UV, v.Normal, color);
	public static Vertex3 SetColor(this Vertex3 v, Color16x4 color) => new(v.Translation, v.UV, v.Normal, color);
}