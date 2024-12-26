using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.SceneObjects.UIs;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints;
using Engine.MemLib;

namespace TicTacToe {
	internal class ClaimDisplay : UIElement {

		public ClaimDisplay( Mesh mesh ) {
			Mesh = mesh;
			Material = new Material( "claim" ).AddTexture( OpenGL.TextureUnit.Texture0, Mem.Textures.BlankWhite );
			ShaderBundle = Mem.ShaderBundles.UI;

			Constraints.Set( new ConstraintBundle( new ModScalingSet( 0.9f ) ) );


		}

	}
}