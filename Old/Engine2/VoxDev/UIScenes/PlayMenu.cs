using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.SceneObjects.UIs;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Tactile;
using Engine.MemLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoxDev.UIScenes {
	public class PlayMenu : UIElement {

		private TextField username;
		private MenuButton hostButton;
		private MenuButton joinButton;
		private MenuButton backButton;
		//Name is selected here
		//When pressing join a new menu appears where the user can enter an IP address

		public string Username { get => username.Text; }

		public event Action HostClicked;
		public event Action JoinClicked;
		public event Action BackClicked;

		public PlayMenu() : base() {
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

			username = new TextField( "Username", (0, .9f), (0.4f, 0.1f) );
			username.SetParent( this );

			joinButton = new MenuButton( Mem.Textures.BlankWhite, (-.6f, 0.5f), (0.3f, 0.1f) );
			joinButton.SetParent( this );
			joinButton.SetText( "Join Games", (0, 0, 0, 1) );
			joinButton.Click += OnJoinClicked;

			hostButton = new MenuButton( Mem.Textures.BlankWhite, (-.6f, 0.2f), (0.3f, 0.1f) );
			hostButton.SetParent( this );
			hostButton.SetText( "Host Games", (0, 0, 0, 1) );
			hostButton.Click += OnHostClicked;

			backButton = new MenuButton( Mem.Textures.BlankWhite, (-.6f, -0.6f), (0.3f, 0.1f) );
			backButton.SetParent( this );
			backButton.SetText( "Back", (0, 0, 0, 1) );
			backButton.Click += OnBackClicked;
		}

		private void OnHostClicked( MouseInputEventData data ) {
			HostClicked?.Invoke();
		}

		private void OnJoinClicked( MouseInputEventData data ) {
			JoinClicked?.Invoke();
		}

		private void OnBackClicked( MouseInputEventData data ) {
			BackClicked?.Invoke();
		}
	}
}
