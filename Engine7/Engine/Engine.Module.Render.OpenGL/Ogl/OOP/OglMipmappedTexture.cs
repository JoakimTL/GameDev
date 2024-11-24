using OpenGL;

namespace Engine.Module.Render.OpenGL.Ogl.OOP;

public class OglMipmappedTexture( string name, TextureTarget target, Vector2<int> level0, uint levels, InternalFormat format, params (TextureParameterName, int)[] parameters )
	: OglTextureBase<MipmappedTextureMetadata>( name, target, level0, new( Math.Max( levels, 1 ), format ), parameters ) {

	protected override void GenerateTexture( MipmappedTextureMetadata metadata )
		=> Gl.TextureStorage2D( TextureID, (int) metadata.Levels, metadata.Format, Level0.X, Level0.Y );

	protected override IReadOnlyList<Vector2<int>> GetLevels( Vector2<int> level0, MipmappedTextureMetadata metadata ) {
		Vector2<int>[] sizes = new Vector2<int>[ metadata.Levels ];
		sizes[ 0 ] = level0;
		for (int i = 1; i < metadata.Levels; i++)
			sizes[ i ] = new Vector2<int>( Math.Max( 1, sizes[ i - 1 ].X / 2 ), Math.Max( 1, sizes[ i - 1 ].Y / 2 ) );
		return sizes.AsReadOnly();
	}
}
