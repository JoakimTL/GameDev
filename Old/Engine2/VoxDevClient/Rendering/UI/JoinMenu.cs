using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.SceneObjects.UIs;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Tactile;
using Engine.MemLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoxDevClient.Rendering.UI {
	public class JoinMenu : UIElement {

		private TextField address;
		private MenuButton joinButton;
		private MenuButton backButton;
		//Name is selected here
		//When pressing join a new menu appears where the user can enter an IP address

		public string Address { get => address.Text; }

		public event Action JoinClicked;
		public event Action BackClicked;

		public JoinMenu() : base() {
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

			address = new TextField( "Lobby Address(IP:Port)", (.6f, .5f), (0.4f, 0.1f) );
			address.SetParent( this );
			address.SetText("[::1]:12345");

			joinButton = new MenuButton( Mem.Textures.BlankWhite, (-.6f, 0.5f), (0.3f, 0.1f) );
			joinButton.SetParent( this );
			joinButton.SetText( "Join Lobby", (0, 0, 0, 1) );
			joinButton.Click += OnJoinClicked;

			backButton = new MenuButton( Mem.Textures.BlankWhite, (-.6f, -0.6f), (0.3f, 0.1f) );
			backButton.SetParent( this );
			backButton.SetText( "Back to\nMain Menu", (0, 0, 0, 1) );
			backButton.Click += OnBackClicked;
		}

		private void OnJoinClicked( MouseInputEventData data ) {
			JoinClicked?.Invoke();
		}

		private void OnBackClicked( MouseInputEventData data ) {
			BackClicked?.Invoke();
		}
	}
}
