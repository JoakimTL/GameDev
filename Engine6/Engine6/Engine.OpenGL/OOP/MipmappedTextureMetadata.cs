using OpenGL;

namespace Engine.OpenGL.OOP;

public readonly struct MipmappedTextureMetadata {
	public readonly uint Levels;
	public readonly InternalFormat Format;

	public MipmappedTextureMetadata( uint levels, InternalFormat format ) {
		if (levels == 0)
			throw new OpenGlArgumentException( "Texture cannot have zero levels", nameof( levels ) );
		Levels = levels;
		Format = format;
	}
}
