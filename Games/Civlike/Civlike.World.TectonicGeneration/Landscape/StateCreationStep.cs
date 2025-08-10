using Civlike.World.State;
using Civlike.World.State.States;
using Civlike.World.TectonicGeneration.Landscape.States;

namespace Civlike.World.TectonicGeneration.Landscape;

public sealed class StateCreationStep( TectonicGenerationParameters parameters ) : TectonicGlobeGenerationProcessingStepBase( parameters ) {
	public override void Process( Globe globe ) {
		globe.AddState( new GlobeTectonicPlateState() );
		foreach (Node node in globe.Nodes) {
			node.AddState( new NodeLandmassState() );
			node.AddState( new NodeTectonicLandscapeState() );
		}
		foreach (Tile tile in globe.Tiles)
			tile.AddState( new TileColorState() );
	}
}
