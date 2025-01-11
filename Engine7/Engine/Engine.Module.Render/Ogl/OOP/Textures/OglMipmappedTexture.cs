using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.Textures;

public class OglMipmappedTexture( string name, TextureTarget target, Vector2<int> level0, uint levels, InternalFormat format, params (TextureParameterName, int)[] parameters )
	: OglTextureBase<OglMipmappedTextureMetadata>( name, target, level0, new( Math.Max( levels, 1 ), format ), parameters ) {

	protected override void AddLevels( List<Vector2<int>> levelsList ) {
		for (int i = 1; i < Metadata.Levels; i++)
			levelsList.Add( (levelsList[ ^1 ] / 2).Max( 1 ) );
	}
	protected override void GenerateTexture( uint textureId )
		=> Gl.TextureStorage2D( textureId, (int) Metadata.Levels, Metadata.Format, this.Level0.X, this.Level0.Y );
}
