using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.Textures;

public class OglMultisampledTexture( string name, TextureTarget target, Vector2<int> level0, uint samples, InternalFormat format, bool fixedSampleLocations, params (TextureParameterName, int)[] parameters )
	: OglTextureBase<OglMultisampledTextureMetadata>( name, target, level0, new( samples, format, fixedSampleLocations ), parameters ) {

	protected override void GenerateTexture( OglMultisampledTextureMetadata metadata )
		=> Gl.TextureStorage2DMultisample( this.TextureID, (int) metadata.Samples, metadata.Format, this.Level0.X, this.Level0.Y, metadata.FixedSampleLocations );

	protected override IReadOnlyList<Vector2<int>> GetLevels( Vector2<int> level0, OglMultisampledTextureMetadata metadata )
		=> [ level0 ];
}
