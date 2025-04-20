using Engine.Standard;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;

namespace Sandbox.Render.Ui;
public sealed class StartMenu : UserInterfaceElementBase {

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
		//_btnExit.Placement.SetTransform( new( (.3, -.4), time, (.25, .1) ) );
	}
}
