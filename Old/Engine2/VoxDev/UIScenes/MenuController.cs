using Engine.Graphics.Objects.Default.SceneObjects.UIs;
using System;

namespace VoxDev.UIScenes {
	class MenuController {

		private UIManager manager;

		private MainMenu mainMenu;
		private OptionsMenu optionsMenu;
		private PlayMenu playMenu;
		private JoinMenu joinMenu;

		public event Action<string> HostRequest;
		public event Action<string, string> ConnectionRequest;
		public event Action ExitClicked;

		public MenuController( UIManager manager ) {
			this.manager = manager;

			manager.Add( mainMenu = new MainMenu(), true );
			manager.Add( optionsMenu = new OptionsMenu(), false );
			manager.Add( playMenu = new PlayMenu(), false );
			manager.Add( joinMenu = new JoinMenu(), false );

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
