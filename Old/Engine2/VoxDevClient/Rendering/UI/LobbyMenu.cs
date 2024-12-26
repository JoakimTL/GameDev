using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.SceneObjects.UIs;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Default.Tactile;
using Engine.MemLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoxDevClient.Rendering.UI {
	public class LobbyMenu : UIElement {

		//have chat be it's own element that is shared between the lobby menu and game ui
		private TextField chat;
		private MenuButton joinButton;
		private MenuButton leaveButton;
		//Name is selected here
		//When pressing join a new menu appears where the user can enter an IP address

		public event Action JoinClicked;
		public event Action LeaveClicked;

		public LobbyMenu() : base() {
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

			chat = new TextField( "Lobby Address(IP:Port)", (.6f, .5f), (0.4f, 0.1f) );
			chat.SetParent( this );
			chat.SetText( "[::1]:12345" );

			leaveButton = new MenuButton( Mem.Textures.BlankWhite, (-.6f, -0.6f), (0.3f, 0.1f) );
			leaveButton.SetParent( this );
			leaveButton.SetText( "Leave Lobby", (0, 0, 0, 1) );
			leaveButton.Click += OnLeaveClicked;
		}

		private void OnJoinClicked( MouseInputEventData data ) {
			JoinClicked?.Invoke();
		}

		private void OnLeaveClicked( MouseInputEventData data ) {
			LeaveClicked?.Invoke();
		}
	}
}
