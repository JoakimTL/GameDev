using Engine.Standard;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;
using Sandbox.Logic.World.Tiles;

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

public sealed class TileDataDisplay : UserInterfaceElementBase {

	private Label _lblTileName = null!;

	protected override void Initialize() {
		AddComponent( _lblTileName = new Label( this ) {
			Text = "Tile Name",
			FontName = "calibrib",
			TextScale = 0.5f,
			Color = (1, 1, 1, 1),
			HorizontalAlignment = Alignment.Center,
			VerticalAlignment = Alignment.Center
		} );
		_lblTileName.Placement.Set( new( (0, -.1), 0, (.3, .1) ), Alignment.Center, Alignment.Positive );
	}

	protected override void OnUpdate( double time, double deltaTime ) {
		_lblTileName.Text = GameStateProvider.Get<Tile>( "selectedTile" )?.Height.ToString() ?? "Unknown";
	}

	protected override bool ShouldDisplay() {
		return GameStateProvider.Get<bool>( "ingame" ) && GameStateProvider.Get<Tile>( "selectedTile" ) is not null;
	}
}