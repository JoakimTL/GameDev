using Engine.Module.Render.Input;
using Engine.Standard;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;

namespace Sandbox.Render.Ui;
public sealed class StartMenu : UserInterfaceElementBase {

	private InteractableButton _btnNewGame = null!;
	private InteractableButton _btnExit = null!;

	protected override void Initialize() {
		_btnNewGame = new InteractableButton( this, "New Game" );
		_btnNewGame.Placement.Set( new( (.3, -.15), 0, (.25, .1) ), Alignment.Negative, Alignment.Positive );
		_btnNewGame.OnClicked += OnNewGameButtonClicked;
		_btnExit = new InteractableButton( this, "Exit" );
		_btnExit.Placement.Set( new( (.3, -.4), 0, (.25, .1) ), Alignment.Negative, Alignment.Positive );
		_btnExit.OnClicked += OnExitButtonClicked;
	}

	private void OnNewGameButtonClicked( InteractableButton btn, MouseButtonEvent @event ) {
		GameStateProvider.SetNewState( "ingame", true );
	}

	private void OnExitButtonClicked( InteractableButton btn, MouseButtonEvent @event ) {
		GameStateProvider.SetNewState( "closegame", true );
	}

	protected override bool ShouldDisplay() {
		return !GameStateProvider.Get<bool>( "ingame" );
	}

	protected override void OnUpdate( double time, double deltaTime ) {
		//_btnExit.Placement.SetTransform( new( (.3, -.4), time, (.25, .1) ) );
	}
}
