using Engine.Module.Render.Input;
using Engine.Standard;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;

namespace Sandbox.Render.Ui;
public sealed class StartMenu : UserInterfaceElementBase {

	private Button _btnNewGame = null!;
	private Button _btnExit = null!;

	protected override void Initialize() {
		_btnNewGame = new Button( this, "New Game" );
		_btnNewGame.Placement.Set( new( (.3, -.15), 0, (.25, .1) ), Alignment.Negative, Alignment.Positive );
		_btnNewGame.OnClicked += OnNewGameButtonClicked;
		_btnExit = new Button( this, "Exit" );
		_btnExit.Placement.Set( new( (.3, -.4), 0, (.25, .1) ), Alignment.Negative, Alignment.Positive );
		_btnExit.OnClicked += OnExitButtonClicked;
	}

	private void OnNewGameButtonClicked( Button btn, MouseButtonEvent @event ) {
		GameStateProvider.SetNewState( "ingame", true );
	}

	private void OnExitButtonClicked( Button btn, MouseButtonEvent @event ) {
		GameStateProvider.SetNewState( "closegame", true );
	}

	protected override bool ShouldDisplay() {
		return !GameStateProvider.Get<bool>( "ingame" );
	}

	protected override void OnUpdate( double time, double deltaTime ) {
		//_btnExit.Placement.SetTransform( new( (.3, -.4), time, (.25, .1) ) );
	}
}
