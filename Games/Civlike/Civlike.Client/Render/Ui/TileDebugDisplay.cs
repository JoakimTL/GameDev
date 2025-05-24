using Civlike.WorldOld;
using Engine;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;
using System.Text;

namespace Civlike.Client.Render.Ui;

public sealed class TileDebugDisplay : UserInterfaceElementBase {

	private Label _infoLabel = null!;

	protected override void Initialize() {
		this._infoLabel = new Label( this ) {
			Text = "noupdate",
			FontName = "COURBD",
			TextScale = 0.08f,
			Color = (1, 1, 1, 1),
			HorizontalAlignment = Alignment.Center,
			VerticalAlignment = Alignment.Positive
		};
		this._infoLabel.Placement.Set( new( (0, -.2), 0, (.8, .2) ), Alignment.Center, Alignment.Positive );

	}

	protected override void OnUpdate( double time, double deltaTime ) {
		DisplayTileInformation();
	}

	private void DisplayTileInformation() {
		Face? selectedTile = this.GameStateProvider.Get<Face>( "selectedTile" );
		if (selectedTile is null) {
			this._infoLabel.Text = "No tile selected";
			return;
		}

		StringBuilder sb = new StringBuilder();
		sb.AppendLine( $"Tile: {selectedTile.Id}" );
		sb.AppendLine( $"Terrain: {selectedTile.State.TerrainType.Name}" );
		sb.AppendLine( $"Seismic activity: {selectedTile.State.SeismicActivity:N4}" );
		sb.AppendLine( $"Height: {selectedTile.State.Height:N2}m" );
		sb.AppendLine( $"Length: {selectedTile.Globe.ApproximateTileLength:N2}m" );
		sb.AppendLine( $"Temperature: {selectedTile.State.Temperature}" );
		sb.AppendLine( $"Pressure: {selectedTile.State.StaticPressure}" );
		sb.AppendLine( $"Moisture capacity mm: {selectedTile.State.GetMoistureCapacityMm():N2}" );
		sb.AppendLine( $"Evaporation mm: {selectedTile.State.EvaporationMm:N2}" );
		sb.AppendLine( $"Moisture mm: {selectedTile.State.AbsoluteHumidityMm:N2}" );
		sb.AppendLine( $"Precipitation mm: {selectedTile.State.PrecipitationMm:N2}" );
		sb.AppendLine( $"Local relief: {selectedTile.State.LocalRelief:N2}m" );
		sb.AppendLine( $"Ruggedness: {selectedTile.State.LocalRelief * selectedTile.State.Ruggedness:N2}m ({selectedTile.State.Ruggedness:N4}%)" );
		sb.AppendLine( $"Gradient: {(selectedTile.State.Gradient.Magnitude<Vector3<float>, float>() * 100):N4}% {selectedTile.State.Gradient}" );

		this._infoLabel.Text = sb.ToString();
	}

	private bool InternalShouldDisplay() {
		Face? selectedTile = this.GameStateProvider.Get<Face>( "selectedTile" );
		if (selectedTile is null)
			return false;
		return true;
	}

	protected override bool ShouldDisplay() {
		return InternalShouldDisplay();
	}
}
