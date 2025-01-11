using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.Textures;

public class OglMultisampledTexture( string name, TextureTarget target, Vector2<int> level0, uint samples, InternalFormat format, bool fixedSampleLocations, params (TextureParameterName, int)[] parameters )
	: OglTextureBase<OglMultisampledTextureMetadata>( name, target, level0, new( samples, format, fixedSampleLocations ), parameters ) {

	protected override void AddLevels( List<Vector2<int>> levelsList ) { }
	protected override void GenerateTexture( uint textureId )
		=> Gl.TextureStorage2DMultisample( textureId, (int) Metadata.Samples, Metadata.Format, this.Level0.X, this.Level0.Y, Metadata.FixedSampleLocations );
}
