using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.Textures;

public readonly struct OglMipmappedTextureMetadata {
	public readonly uint Levels;
	public readonly InternalFormat Format;

	public OglMipmappedTextureMetadata( uint levels, InternalFormat format ) {
		if (levels == 0)
			throw new OpenGlArgumentException( "Texture cannot have zero levels", nameof( levels ) );
		this.Levels = levels;
		this.Format = format;
	}
}
