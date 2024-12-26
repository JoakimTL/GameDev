using Engine.MemLib;
using OpenGL;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual.Text {
	public class Font {

		public readonly string FilePath;
		public readonly Texture Texture;
		public readonly FontMetadata Data;
		public readonly Material Material;

		public Font( string filePath ) {
			FilePath = filePath;
			Texture = Mem.Textures.Get( "fonts\\" + filePath );
			Material = new FontMaterial( filePath, TextureUnit.Texture0, Texture );
			Data = new FontMetadata( "res\\textures\\fonts\\" + filePath + ".fnt" );
		}

		public override string ToString() {
			return $"Font[{FilePath}]";
		}
	}
}
