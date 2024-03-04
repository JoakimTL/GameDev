namespace OpenGL.OOP;

public class OglMipmappedTexture( ContextWarningLog warningLog, TextureTarget target, Vector2ui level0, uint levels, InternalFormat format, params (TextureParameterName, int)[] parameters ) 
	: OglTextureBase<MipmappedTextureMetadata>( warningLog, target, level0, new( Math.Max( levels, 1 ), format ), parameters ) {

	protected override void GenerateTexture( MipmappedTextureMetadata metadata ) 
		=> Gl.TextureStorage2D( TextureID, (int) metadata.Levels, metadata.Format, (int) Level0.X, (int) Level0.Y );

	protected override IReadOnlyList<Vector2ui> GetLevels( Vector2ui level0, MipmappedTextureMetadata metadata ) {
		Vector2ui[] sizes = new Vector2ui[ metadata.Levels ];
		sizes[ 0 ] = level0;
		for (int i = 1; i < metadata.Levels; i++)
			sizes[ i ] = new Vector2ui( Math.Max( 1, sizes[ i - 1 ].X / 2 ), Math.Max( 1, sizes[ i - 1 ].Y / 2 ) );
		return sizes.AsReadOnly();
	}
}
