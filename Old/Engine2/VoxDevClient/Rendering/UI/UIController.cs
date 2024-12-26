using Engine.Graphics.Objects.Default.SceneObjects.UIs;
using Engine.Graphics.Objects.Default.SceneObjects.UIs.Constraints;
using System;

namespace VoxDevClient.Rendering.UI {
	class UIController {

		private UIManager manager;

		private MainMenu mainMenu;
		private OptionsMenu optionsMenu;
		private PlayMenu playMenu;
		private JoinMenu joinMenu;

		private ChatBox chat;

		public event Action<string> HostRequest;
		public event Action<string, string> ConnectionRequest;
		public event Action ExitClicked;

		public UIController( UIManager manager ) {
			this.manager = manager;

			manager.Add( mainMenu = new MainMenu(), true );
			manager.Add( optionsMenu = new OptionsMenu(), false );
			manager.Add( playMenu = new PlayMenu(), false );
			manager.Add( joinMenu = new JoinMenu(), false );

			chat = new ChatBox {
				LayerOffset = 2
			};
			chat.Constraints.Set(
				new ConstraintBundle(
					new ModTranslationSetAligmentHorizontal( HorizontalAlignment.RIGHT, true ),
					new ModTranslationSetAligmentVertical( VerticalAlignment.BOTTOM, true ),
					new ModTranslationAdd( (-0.35f, 0.35f) ),
					new ModScalingSet( 0.33f ),
					new ModColorSet( (1, 1, 0, 0.1f) )
				)
			);
			manager.Add( chat, true );

			mainMenu.PlayClicked += ActivatePlayMenu;
			mainMenu.OptionsClicked += ActivateOptionsMenu;
			mainMenu.ExitClicked += Exit;

			optionsMenu.BackClicked += ActivateMainMenu;

			playMenu.JoinClicked += ActivateJoinMenu;
			playMenu.BackClicked += ActivateMainMenu;
			playMenu.HostClicked += OnHosting;

			joinMenu.JoinClicked += OnJoinClicked;
			joinMenu.BackClicked += ActivateMainMenu;
		}

		public void ConnectionDropped() {

		}

		private void ActivateOptionsMenu() {
			mainMenu.Deactivate();
			playMenu.Deactivate();
			joinMenu.Deactivate();
			optionsMenu.Activate();
		}

		private void ActivatePlayMenu() {
			mainMenu.Deactivate();
			optionsMenu.Deactivate();
			joinMenu.Deactivate();
			playMenu.Activate();
		}

		private void ActivateMainMenu() {
			playMenu.Deactivate();
			optionsMenu.Deactivate();
			joinMenu.Deactivate();
			mainMenu.Activate();
		}

		private void ActivateJoinMenu() {
			playMenu.Deactivate();
			optionsMenu.Deactivate();
			mainMenu.Deactivate();
			joinMenu.Activate();
		}

		private void OnJoinClicked() {
			ConnectionRequest?.Invoke( playMenu.Username, joinMenu.Address );
		}

		private void OnHosting() {
			HostRequest?.Invoke( playMenu.Username );
		}

		private void Exit() {
			ExitClicked?.Invoke();
		}
	}
}
