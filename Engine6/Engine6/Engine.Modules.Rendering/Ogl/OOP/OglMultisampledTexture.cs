using Engine.Data;
using OpenGL;

namespace Engine.Modules.Rendering.Ogl.OOP;

public class OglMultisampledTexture( string name, TextureTarget target, Vector2i level0, uint samples, InternalFormat format, bool fixedSampleLocations, params (TextureParameterName, int)[] parameters )
	: OglTextureBase<MultisampledTextureMetadata>( name, target, level0, new( samples, format, fixedSampleLocations ), parameters ) {

	protected override void GenerateTexture( MultisampledTextureMetadata metadata )
		=> Gl.TextureStorage2DMultisample( TextureID, (int) metadata.Samples, metadata.Format, Level0.X, Level0.Y, metadata.FixedSampleLocations );

	protected override IReadOnlyList<Vector2i> GetLevels( Vector2i level0, MultisampledTextureMetadata metadata )
		=> [ level0 ];
}
