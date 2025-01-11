using Engine.Standard;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;
using Sandbox.Logic.World.Tiles;

namespace Sandbox.Render.Ui;

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