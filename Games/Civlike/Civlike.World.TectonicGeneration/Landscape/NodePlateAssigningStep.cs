using Civlike.World.State;
using Civlike.World.TectonicGeneration.Landscape.States;
using Civlike.World.TectonicGeneration.NoiseProviders;
using Engine;

namespace Civlike.World.TectonicGeneration.Landscape;

[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<StateCreationStep>]
public sealed class NodePlateAssigningStep( TectonicGenerationParameters parameters ) : TectonicGlobeGenerationProcessingStepBase( parameters ) {
	public override void Process( Globe globe ) {
		SphericalVoronoiGenerator sphericalVoronoiGenerator = new( new( Parameters.SeedProvider.Next() ), Parameters.LandscapeParameters.PlateCount, Parameters.LandscapeParameters.MinimumPlateMidpointDistance );

		Noise3 xShiftNoise = new( Parameters.SeedProvider.Next(), 8 );
		Noise3 yShiftNoise = new( Parameters.SeedProvider.Next(), 7 );
		Noise3 zShiftNoise = new( Parameters.SeedProvider.Next(), 8 );

		ParallelProcessing.Range( globe.Nodes.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Node n = globe.Nodes[ i ];

				Vector3<float> nodeVector = n.Vertex.Vector;
				Vector3<float> shift = new Vector3<float>( xShiftNoise.Noise( nodeVector ), yShiftNoise.Noise( nodeVector ), zShiftNoise.Noise( nodeVector ) ) * 2 - 1;
				Vector3<float> regionPosition = nodeVector + shift * 0.16f;
				regionPosition = regionPosition.Normalize<Vector3<float>, float>();

				n.GetStateOrThrow<NodeTectonicLandscapeState>().Region = sphericalVoronoiGenerator.Get( regionPosition );
			}
		} );

		foreach (SphericalVoronoiRegion region in sphericalVoronoiGenerator.Regions)
			region.AddState( new SphericalVoronoiRegionTectonicPlateState() );

		foreach (Node node in globe.Nodes) {
			node.GetStateOrThrow<NodeTectonicLandscapeState>()
				.Region.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>()
				.Nodes.Add( node );
		}

		globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions.AddRange( sphericalVoronoiGenerator.Regions );
	}
}
