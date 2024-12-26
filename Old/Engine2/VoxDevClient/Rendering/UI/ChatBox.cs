using Engine.Graphics.Objects.Default.SceneObjects.UIs;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Tactile;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Visual;
using Engine.MemLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoxDevClient.Rendering.UI {
	class ChatBox : UIElement {

		private ScrollableTextbox textbox;
		private TextField textfield;

		public ChatBox() {
			Mesh = Mem.Mesh2.Square;
			ShaderBundle = Mem.ShaderBundles.UI;
			Material = new Engine.Graphics.Objects.Material( "lol" ).AddTexture( OpenGL.TextureUnit.Texture0, Mem.Textures.BlankWhite );
			textbox = new ScrollableTextbox();
			textfield = new TextField( "Chat..." );

			textbox.Constraints.Set(
				new ConstraintBundle(
					new ModTranslationSet( (0, 1 - .85f) ),
					new ModScalingSet( (1, 1 - .15f) ),
					new ModColorSet( (1, 1, 1, 0.05f) )
				)
			);

			textfield.Constraints.Set(
				new ConstraintBundle(
					new ModTranslationSet( (0, -0.85f) ),
					new ModScalingSet( (1, 0.15f) ),
					new ModColorSet( (1, 1, 1, 0.05f) )
				)
			);

			textbox.SetParent( this );
			textfield.SetParent( this );
		}


	}
}
