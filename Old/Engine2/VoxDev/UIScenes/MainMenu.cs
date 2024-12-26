using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.SceneObjects.UIs;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Tactile;
using Engine.MemLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoxDev.UIScenes {
	public class MainMenu : UIElement {

		private MenuButton playButton;
		private MenuButton optionsButton;
		private MenuButton exitButton;

		public event Action PlayClicked;
		public event Action OptionsClicked;
		public event Action ExitClicked;

		public MainMenu() : base() {
			Mesh = Mem.Mesh2.Square;
			Material = new Material( "mainMenu" ).AddTexture( OpenGL.TextureUnit.Texture0, Mem.Textures.BlankWhite );
			Constraints.Set(
				new ConstraintBundle(
					new ModTranslationSetAligmentHorizontal( HorizontalAlignment.LEFT, true ),
					new ModTranslationAdd( (1, 0) ),
					new ModScalingSet( (1, 1) ),
					new ModColorSet( (1, 0, 0, 0.1f) )
				)
			);

			playButton = new MenuButton( Mem.Textures.BlankWhite, (-.6f, 0.6f), (0.3f, 0.1f) );
			playButton.SetParent( this );
			playButton.SetText( "Play", (0, 0, 0, 1) );
			playButton.Click += OnPlayClicked;

			optionsButton = new MenuButton( Mem.Textures.BlankWhite, (-.6f, 0.2f), (0.3f, 0.1f) );
			optionsButton.SetParent( this );
			optionsButton.SetText( "Options", (0, 0, 0, 1) );
			optionsButton.Click += OnOptionsClicked;

			exitButton = new MenuButton( Mem.Textures.BlankWhite, (-.6f, -0.6f), (0.3f, 0.1f) );
			exitButton.SetParent( this );
			exitButton.SetText( "Exit", (0, 0, 0, 1) );
			exitButton.Click += OnExitClicked;
		}

		private void OnPlayClicked( MouseInputEventData data ) {
			PlayClicked?.Invoke();
		}

		private void OnOptionsClicked( MouseInputEventData data ) {
			OptionsClicked?.Invoke();
		}

		private void OnExitClicked( MouseInputEventData data ) {
			ExitClicked?.Invoke();
		}
	}
}
