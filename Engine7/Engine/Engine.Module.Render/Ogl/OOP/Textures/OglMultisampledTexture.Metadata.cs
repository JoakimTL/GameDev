using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.Textures;

public readonly struct OglMultisampledTextureMetadata {
	public readonly uint Samples;
	public readonly InternalFormat Format;
	public readonly bool FixedSampleLocations;

	public OglMultisampledTextureMetadata( uint samples, InternalFormat format, bool fixedSampleLocations ) {
		if (samples == 0)
			throw new OpenGlArgumentException( "Texture cannot have zero samples", nameof( samples ) );
		this.Samples = samples;
		this.Format = format;
		this.FixedSampleLocations = fixedSampleLocations;
	}
}
