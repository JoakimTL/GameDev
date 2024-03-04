using OpenGL;

namespace Engine.Modules.Rendering.Ogl.OOP;

public readonly struct MultisampledTextureMetadata {
	public readonly uint Samples;
	public readonly InternalFormat Format;
	public readonly bool FixedSampleLocations;

	public MultisampledTextureMetadata( uint samples, InternalFormat format, bool fixedSampleLocations ) {
		if (samples == 0)
			throw new OpenGlArgumentException( "Texture cannot have zero samples", nameof( samples ) );
		Samples = samples;
		Format = format;
		FixedSampleLocations = fixedSampleLocations;
	}
}
