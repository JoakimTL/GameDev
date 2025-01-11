using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.Textures;

public interface IOglTexture : IDisposable {
	uint TextureID { get; }
	TextureTarget Target { get; }
	bool Resident { get; }
	Vector2<int> Level0 { get; }
}
