using Civlike.World.State;
using Civlike.World.TectonicGeneration.Landscape.States;
using Civlike.World.TectonicGeneration.NoiseProviders;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Numerics;
using System.Threading;

namespace Civlike.World.TectonicGeneration.Landscape.Plates;

[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<PlateMergingStep>]
public sealed class PlateEdgeDetectionStep( TectonicGenerationParameters parameters ) : TectonicGlobeGenerationProcessingStepBase( parameters ) {
	public override void Process( Globe globe ) {
		List<SphericalVoronoiRegion> regions = globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions;

		ParallelProcessing.Range( regions.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				SphericalVoronoiRegion region = regions[ i ];
				SphericalVoronoiRegionTectonicPlateState state = region.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();

				foreach (Node n in state.Nodes) {
					NodeTectonicLandscapeState nodeState = n.GetStateOrThrow<NodeTectonicLandscapeState>();
					foreach (Node neighbouringNode in n.NeighbouringNodes) {
						NodeTectonicLandscapeState neighbouringNodeState = neighbouringNode.GetStateOrThrow<NodeTectonicLandscapeState>();
						SphericalVoronoiRegion neighbouringRegion = neighbouringNodeState.Region;
						if (neighbouringRegion == nodeState.Region)
							continue;

						if (!state.Edges.TryGetValue( neighbouringRegion, out PlateEdge? edge ))
							state.Edges.Add( neighbouringRegion, edge = new( state, neighbouringRegion.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>() ) );
						edge.Nodes.Add( n );
					}
				}
			}
		} );
	}
}
