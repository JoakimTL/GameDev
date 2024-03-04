using Engine.Data;
using Engine.OpenGL;
using OpenGL;

namespace Engine.OpenGL.OOP;

public class OglMipmappedTexture( ContextWarningLog warningLog, TextureTarget target, Vector2i level0, uint levels, InternalFormat format, params (TextureParameterName, int)[] parameters )
	: OglTextureBase<MipmappedTextureMetadata>( warningLog, target, level0, new( Math.Max( levels, 1 ), format ), parameters ) {

	protected override void GenerateTexture( MipmappedTextureMetadata metadata )
		=> Gl.TextureStorage2D( TextureID, (int) metadata.Levels, metadata.Format, Level0.X, Level0.Y );

	protected override IReadOnlyList<Vector2i> GetLevels( Vector2i level0, MipmappedTextureMetadata metadata ) {
		Vector2i[] sizes = new Vector2i[ metadata.Levels ];
		sizes[ 0 ] = level0;
		for (int i = 1; i < metadata.Levels; i++)
			sizes[ i ] = new Vector2i( Math.Max( 1, sizes[ i - 1 ].X / 2 ), Math.Max( 1, sizes[ i - 1 ].Y / 2 ) );
		return sizes.AsReadOnly();
	}
}
