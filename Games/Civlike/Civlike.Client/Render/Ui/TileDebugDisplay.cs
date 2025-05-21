using Civlike.World;
using Engine;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;
using System.Text;

namespace Civlike.Client.Render.Ui;

public sealed class TileDebugDisplay : UserInterfaceElementBase {

	private Label _infoLabel = null!;

	protected override void Initialize() {
		_infoLabel = new Label( this ) {
			Text = "noupdate",
			FontName = "COURBD",
			TextScale = 0.08f,
			Color = (1, 1, 1, 1),
			HorizontalAlignment = Alignment.Center,
			VerticalAlignment = Alignment.Positive
		};
		_infoLabel.Placement.Set( new( (0, -.2), 0, (.8, .2) ), Alignment.Center, Alignment.Positive );

	}

	protected override void OnUpdate( double time, double deltaTime ) {
		DisplayTileInformation();
	}

	private void DisplayTileInformation() {
		Face? selectedTile = GameStateProvider.Get<Face>( "selectedTile" );
		if (selectedTile is null) {
			_infoLabel.Text = "No tile selected";
			return;
		}

		StringBuilder sb = new StringBuilder();
		sb.AppendLine( $"Tile: {selectedTile.Id}" );
		sb.AppendLine( $"Terrain: {selectedTile.State.TerrainType.Name}" );
		sb.AppendLine( $"Seismic activity: {selectedTile.State.SeismicActivity}" );
		sb.AppendLine( $"Height: {selectedTile.State.Height}" );
		sb.AppendLine( $"Temperature: {selectedTile.State.Temperature}" );
		sb.AppendLine( $"Pressure: {selectedTile.State.AtmosphericPressure}" );
		sb.AppendLine( $"Moisture capacity mm: {selectedTile.State.GetMoistureCapacityMm()}" );
		sb.AppendLine( $"Evaporation mm: {selectedTile.State.EvaporationMm}" );
		sb.AppendLine( $"Moisture mm: {selectedTile.State.AbsoluteHumidityMm}" );
		sb.AppendLine( $"Precipitation mm: {selectedTile.State.PrecipitationMm}" );
		sb.AppendLine( $"Local relief: {selectedTile.State.LocalRelief}m" );
		sb.AppendLine( $"Ruggedness: {selectedTile.State.LocalRelief * selectedTile.State.Ruggedness}m ({selectedTile.State.Ruggedness}%)" );

		_infoLabel.Text = sb.ToString();
	}

	private bool InternalShouldDisplay() {
		Face? selectedTile = GameStateProvider.Get<Face>( "selectedTile" );
		if (selectedTile is null)
			return false;
		return true;
	}

	protected override bool ShouldDisplay() {
		return InternalShouldDisplay();
	}
}
