namespace Engine.Module.Render.Ogl.OOP.Textures;

public sealed class TextureReference( OglTextureBase texture ) {
	private readonly OglTextureBase _texture = texture;

	internal event Action? OnDestruction;

	~TextureReference() {
		OnDestruction?.Invoke();
	}

	public ulong GetHandle() => this._texture.Handle;
}