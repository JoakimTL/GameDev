using Civlike.Messages;
using Engine;
using Engine.Modularity;
using Engine.Module.Render.Input;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;

namespace Civlike.Client.Render.Ui;
public sealed class StartMenu() : UserInterfaceElementWithMessageNodeBase( "ui_startmenu" ) {

	private InteractableButton _btnNewGame = null!;
	private InteractableButton _btnExit = null!;

	protected override void Initialize() {
		this._btnNewGame = new InteractableButton( this, "New Game" );
		this._btnNewGame.Placement.Set( new( (.3, -.15), 0, (.25, .1) ), Alignment.Negative, Alignment.Positive );
		this._btnNewGame.OnClicked += OnNewGameButtonClicked;
		this._btnExit = new InteractableButton( this, "Exit");
		this._btnExit.Placement.Set( new( (.3, -.4), 0, (.25, .1) ), Alignment.Negative, Alignment.Positive );
		this._btnExit.OnClicked += OnExitButtonClicked;
	}

	private void OnNewGameButtonClicked( InteractableButton btn, MouseButtonEvent @event ) => Publish( new NewGameMessage(), "gamelogic", true );
	private void OnExitButtonClicked( InteractableButton btn, MouseButtonEvent @event ) => Publish( new ExitGameMessage(), null, true );

	protected override bool ShouldDisplay() {
		return this.GameStateProvider.Get<bool>( UiElementConstants.ShowStartMenu ); //TODO: Create a more complex state machine for ui?
	}

	protected override void OnMessageReceived( Message message ) {
		if (message.Content is NewGameMessageResponse newGameMessageResponse) {
			this.GameStateProvider.SetNewState( UiElementConstants.ShowStartMenu, false );
			this.GameStateProvider.SetNewState( UiElementConstants.ShowNewGameMenu, true );
		}
	}
}
