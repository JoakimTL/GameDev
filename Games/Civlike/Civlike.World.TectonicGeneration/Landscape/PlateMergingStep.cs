using Civlike.World.State;
using Civlike.World.TectonicGeneration.Landscape.States;
using Civlike.World.TectonicGeneration.NoiseProviders;
using Engine.Logging;
using System.Collections.Concurrent;
using System.Drawing;
using System.Numerics;
using System.Runtime.Intrinsics;

namespace Civlike.World.TectonicGeneration.Landscape;

[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<PlateSeparationStep>]
public sealed class PlateMergingStep( TectonicGenerationParameters parameters ) : TectonicGlobeGenerationProcessingStepBase( parameters ) {
	public override void Process( Globe globe ) {
		List<SphericalVoronoiRegion> regions = globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions;

		ConcurrentDictionary<SphericalVoronoiRegion, SphericalVoronoiRegion> mergeOperations = new();
		ParallelProcessing.Range( regions.Count, ( start, end, taskId ) => {
			HashSet<SphericalVoronoiRegion> neighbouringPlates = [];
			for (int i = start; i < end; i++) {
				SphericalVoronoiRegion region = regions[ i ];
				SphericalVoronoiRegionTectonicPlateState state = region.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();
				//Here we check how many neighbouring regions this region has. If it has 1 we merge it into that neighbour, if it has 2 or more we leave it be.

				neighbouringPlates.Clear();
				foreach (Node n in state.Nodes)
					foreach (Node nn in n.NeighbouringNodes) {
						NodeTectonicLandscapeState nnState = nn.GetStateOrThrow<NodeTectonicLandscapeState>();
						if (nnState.Region == region)
							continue;
						neighbouringPlates.Add( nnState.Region );
					}

				if (neighbouringPlates.Count == 1) {
					mergeOperations[ region ] = neighbouringPlates.First();
				}
			}
		} );

		foreach (KeyValuePair<SphericalVoronoiRegion, SphericalVoronoiRegion> pair in mergeOperations) {
			this.LogLine( $"Merging {pair.Key} into {pair.Value}" );
			SphericalVoronoiRegion mergeFrom = pair.Key;
			SphericalVoronoiRegion mergeTo = pair.Value;
			while (mergeOperations.TryGetValue( mergeTo, out SphericalVoronoiRegion? finalMergeTo ))
				mergeTo = finalMergeTo;

			SphericalVoronoiRegionTectonicPlateState? mergeFromState = mergeFrom.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();
			SphericalVoronoiRegionTectonicPlateState? mergeToState = mergeTo.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();

			mergeToState.Nodes.AddRange( mergeFromState.Nodes );
			foreach (Node node in mergeFromState.Nodes)
				node.GetStateOrThrow<NodeTectonicLandscapeState>().Region = mergeTo;
			mergeFromState.Nodes.Clear();
			regions.Remove( mergeFrom );

		}
	}
}
