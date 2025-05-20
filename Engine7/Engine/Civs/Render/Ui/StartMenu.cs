using Civs.Messages;
using Engine;
using Engine.Modularity;
using Engine.Module.Render.Input;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;

namespace Civs.Render.Ui;
public sealed class StartMenu() : UserInterfaceElementWithMessageNodeBase( "ui_startmenu" ) {

	private InteractableButton _btnNewGame = null!;
	private InteractableButton _btnExit = null!;

	protected override void Initialize() {
		_btnNewGame = new InteractableButton( this, "A%" );
		_btnNewGame.Placement.Set( new( (.3, -.15), 0, (.25, .1) ), Alignment.Negative, Alignment.Positive );
		_btnNewGame.OnClicked += OnNewGameButtonClicked;
		_btnExit = new InteractableButton( this, "%");
		_btnExit.Placement.Set( new( (.3, -.4), 0, (.25, .1) ), Alignment.Negative, Alignment.Positive );
		_btnExit.OnClicked += OnExitButtonClicked;
	}

	private void OnNewGameButtonClicked( InteractableButton btn, MouseButtonEvent @event ) => Publish( new NewGameMessage(), "gamelogic", true );
	private void OnExitButtonClicked( InteractableButton btn, MouseButtonEvent @event ) => Publish( new ExitGameMessage(), null, true );

	protected override bool ShouldDisplay() {
		return GameStateProvider.Get<bool>( UiElementConstants.ShowStartMenu ); //TODO: Create a more complex state machine for ui?
	}

	protected override void OnMessageReceived( Message message ) {
		if (message.Content is NewGameMessageResponse newGameMessageResponse) {
			GameStateProvider.SetNewState( UiElementConstants.ShowStartMenu, false );
			GameStateProvider.SetNewState( UiElementConstants.ShowNewGameMenu, true );
		}
	}
}
