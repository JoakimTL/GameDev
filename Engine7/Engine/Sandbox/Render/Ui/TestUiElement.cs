using Engine.Standard;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;

namespace Sandbox.Render.Ui;
public sealed class StartMenu : UserInterfaceElementBase {

	private Button _btnNewGame = null!;
	private Button _btnExit = null!;

	protected override void Initialize() {
		AddComponent( _btnNewGame = new Button( this, "New Game", "calibrib", (1, 1, 1, 1), (0.77, .77, .77, 1), (.5, .5, .5, 1) ) );
		_btnNewGame.Placement.Set( new( (.3, -.15), 0, (.25, .1) ), Alignment.Negative, Alignment.Positive );
		_btnNewGame.ButtonClicked += OnNewGameButtonClicked;
		AddComponent( _btnExit = new Button( this, "Exit", "calibrib", (1, 1, 1, 1), (0.77, .77, .77, 1), (.5, .5, .5, 1) ) );
		_btnExit.Placement.Set( new( (.3, -.4), 0, (.25, .1) ), Alignment.Negative, Alignment.Positive );
		_btnExit.ButtonClicked += OnExitButtonClicked;
	}

	private void OnNewGameButtonClicked() {
		GameStateProvider.Set( "ingame", true );
	}

	private void OnExitButtonClicked() {
		GameStateProvider.Set( "closegame", true );
	}

	protected override bool ShouldDisplay() {
		return !GameStateProvider.Get<bool>( "ingame" );
	}

	protected override void OnUpdate( double time, double deltaTime ) {
	}
}
