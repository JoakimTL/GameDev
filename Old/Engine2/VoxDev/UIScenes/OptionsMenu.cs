using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.SceneObjects.UIs;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Tactile;
using Engine.MemLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoxDev.UIScenes {
	public class OptionsMenu : UIElement {

		private MenuButton playButton;
		private MenuButton optionsButton;
		private MenuButton backButton;

		public event ButtonST.MouseEventHandler PlayClicked;
		public event ButtonST.MouseEventHandler OptionsClicked;
		public event Action BackClicked;

		public OptionsMenu() : base() {
			Mesh = Mem.Mesh2.Square;
			Material = new Material( "optionsMenu" ).AddTexture( OpenGL.TextureUnit.Texture0, Mem.Textures.BlankWhite );
			Constraints.Set(
				new ConstraintBundle(
					new ModTranslationSetAligmentHorizontal( HorizontalAlignment.LEFT, true ),
					new ModTranslationAdd( (1, 0) ),
					new ModScalingSet( (1, 1) ),
					new ModColorSet( (1, 0, 0, 0.1f) )
				)
			);

			backButton = new MenuButton( Mem.Textures.BlankWhite, (-.6f, -0.6f), (0.3f, 0.1f) );
			backButton.SetParent( this );
			backButton.SetText( "Back", (0, 0, 0, 1) );
			backButton.Click += OnBackClicked;
		}

		private void OnBackClicked( MouseInputEventData data ) {
			BackClicked?.Invoke();
		}
	}
}
