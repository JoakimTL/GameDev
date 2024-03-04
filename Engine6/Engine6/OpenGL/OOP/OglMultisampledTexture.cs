namespace OpenGL.OOP;

public class OglMultisampledTexture( ContextWarningLog warningLog, TextureTarget target, Vector2ui level0, uint samples, InternalFormat format, bool fixedSampleLocations, params (TextureParameterName, int)[] parameters ) 
	: OglTextureBase<MultisampledTextureMetadata>( warningLog, target, level0, new( samples, format, fixedSampleLocations ), parameters ) {

	protected override void GenerateTexture( MultisampledTextureMetadata metadata )
		=> Gl.TextureStorage2DMultisample( TextureID, (int) metadata.Samples, metadata.Format, (int) Level0.X, (int) Level0.Y, metadata.FixedSampleLocations );

	protected override IReadOnlyList<Vector2ui> GetLevels( Vector2ui level0, MultisampledTextureMetadata metadata )
		=> [ level0 ];
}
