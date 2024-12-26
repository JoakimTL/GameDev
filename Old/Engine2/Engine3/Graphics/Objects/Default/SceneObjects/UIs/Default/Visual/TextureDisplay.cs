using Engine.MemLib;
using Engine.Graphics.Objects.Default.Shaders;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Graphics.Objects.Default.SceneObjects.UIs;
using Engine.Graphics.Objects.Default.Materials;
using Engine.LinearAlgebra;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual {
	public class TextureDisplay : UIElement {

		public TextureDisplay( Texture t ) {
			Mesh = Mem.Mesh2.Square;
			Material = new SingleTextureMaterial( "texdisp", TextureUnit.Texture0, t );
			Mem.CollisionMolds.SquareUniform.MoldNew( Data.CollisionModel );
		}
	}
}
