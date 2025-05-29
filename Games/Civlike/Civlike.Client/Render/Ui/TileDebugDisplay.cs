using Civlike.World.GameplayState;
using Civlike.World.TectonicGeneration;
using Engine;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;
using System.Runtime.Intrinsics.Arm;
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
		Vector3<float> center = selectedTile.Blueprint.GetCenter();
		sb.AppendLine( $"Tile: {selectedTile.Id}" );
		sb.AppendLine( $"Terrain: {selectedTile.State.TerrainType.Name}" );
		if (selectedTile.GenerationFace is not null && selectedTile.GenerationFace is Civlike.World.GenerationState.Face<TectonicFaceState> tectonicFace) {
			TectonicFaceState tectonicState = tectonicFace.State;
			sb.AppendLine( $"Seismic activity: {tectonicState.BaselineValues.SeismicActivity:N4}" );
			sb.AppendLine( $"Height: {tectonicState.BaselineValues.ElevationMean:N2}m" );
			sb.AppendLine( $"Vertex Elevations: {selectedTile.Blueprint.Vertices[ 0 ].Height:N2}m {selectedTile.Blueprint.Vertices[ 1 ].Height:N2}m {selectedTile.Blueprint.Vertices[ 2 ].Height:N2}m" );
			sb.AppendLine( $"Gradient: {(tectonicState.BaselineValues.Gradient.Length() * 100):N4}% {tectonicState.BaselineValues.Gradient}" );
			sb.AppendLine( $"Elevation std. dev.: {tectonicState.BaselineValues.ElevationStandardDeviation:N4}m" );
			//sb.AppendLine( $"Length: {selectedTile.GenerationFace.G.ApproximateTileLength:N2}m" );
			sb.AppendLine( $"Average Temperature: {tectonicState.AverageAirTemperature}" );
			sb.AppendLine( $"Average surface temperature: {tectonicState.AverageSurfaceTemperature}" );
			sb.AppendLine( $"Pressure: {tectonicState.Pressure.Pascal:N2}Pa" );
			sb.AppendLine( $"Sea pressure: {(tectonicState.Pressure.Pascal + tectonicFace.State.SeaPressure.Pascal):N2}Pa" );
			sb.AppendLine( $"Distance from land {tectonicState.DistanceFromLand:N2}m" );
			sb.AppendLine( $"Specific Humidity: {tectonicState.SpecificHumidity:N4} kg/kg" );
			sb.AppendLine( $"Wind: {tectonicState.Wind} {tectonicState.Wind.Length():N3}m/s" );
			sb.AppendLine( $"Tangential wind: {tectonicState.TangentialWind} {tectonicState.TangentialWind.Length():N3}m/s" );
			sb.AppendLine( $"Soil moisture: {tectonicState.SoilMoisture:N2}mm" );
			sb.AppendLine( $"Runoff accumulation: {tectonicState.RunoffAccumulation:N2}m^3/s" );
			//sb.AppendLine( $"Moisture capacity mm: {tectonicState.GetMoistureCapacityMm():N2}" );
			//sb.AppendLine( $"Evaporation mm: {selectedTile.State.EvaporationMm:N2}" );
			//sb.AppendLine( $"Moisture mm: {selectedTile.State.AbsoluteHumidityMm:N2}" );
			//sb.AppendLine( $"Precipitation mm: {selectedTile.State.PrecipitationMm:N2}" );
			//sb.AppendLine( $"Local relief: {selectedTile.State.LocalRelief:N2}m" );
			//sb.AppendLine( $"Ruggedness: {selectedTile.State.LocalRelief * selectedTile.State.Ruggedness:N2}m ({selectedTile.State.Ruggedness:N4}%)" );
		}

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
