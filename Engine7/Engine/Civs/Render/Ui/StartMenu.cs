using Civs.Messages;
using Engine;
using Engine.Modularity;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;

namespace Civs.Render.Ui;
public sealed class StartMenu() : UserInterfaceElementWithMessageNodeBase( "ui_startmenu" ) {

	private Button _btnNewGame = null!;
	private Button _btnExit = null!;

	protected override void Initialize() {
		AddComponent( _btnNewGame = new Button( this, "New Game", "calibrib",
			( btn ) => btn.Background.Color = 1,
			( btn ) => btn.Background.Color = (.9, .9, .9, 1),
			( btn ) => btn.Background.Color = (.75, .75, .75, 1) ) );
		_btnNewGame.Placement.Set( new( (.3, -.15), 0, (.25, .1) ), Alignment.Negative, Alignment.Positive );
		_btnNewGame.ButtonClicked += OnNewGameButtonClicked;
		AddComponent( _btnExit = new Button( this, "Exit", "calibrib",
			( btn ) => btn.Background.Color = 1,
			( btn ) => btn.Background.Color = (.9, .9, .9, 1),
			( btn ) => btn.Background.Color = (.75, .75, .75, 1) ) );
		_btnExit.Placement.Set( new( (.3, -.4), 0, (.25, .1) ), Alignment.Negative, Alignment.Positive );
		_btnExit.ButtonClicked += OnExitButtonClicked;
	}

	private void OnNewGameButtonClicked() => Publish( new NewGameMessage(), "gamelogic" );
	private void OnExitButtonClicked() => Publish( new ExitGameMessage(), null );

	protected override bool ShouldDisplay() {
		return GameStateProvider.Get<bool>( "showStartMenu" ); //TODO: Create a more complex state machine for ui?
	}

	protected override void OnMessageReceived( Message message ) {
		if (message.Content is NewGameMessageResponse newGameMessageResponse) {
			GameStateProvider.Set( "showStartMenu", false );
			GameStateProvider.Set( "showNewGameMenu", true );
		}
	}
}
