using Civlike.World.State;
using Civlike.World.TectonicGeneration.Landscape.States;
using Civlike.World.TectonicGeneration.NoiseProviders;
using Engine;
using Engine.Logging;
using System.Collections.Concurrent;

namespace Civlike.World.TectonicGeneration.Landscape;

[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<NodePlateAssigningStep>]
public sealed class PlateSeparationStep( TectonicGenerationParameters parameters ) : TectonicGlobeGenerationProcessingStepBase( parameters ) {
	public override void Process( Globe globe ) {
		List<SphericalVoronoiRegion> regions = globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions;

		ConcurrentQueue<List<int>> nodesToSeparate = new();

		ParallelProcessing.Range( regions.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				SphericalVoronoiRegion currentSphericalRegion = regions[ i ];
				SphericalVoronoiRegionTectonicPlateState tectonicState = currentSphericalRegion.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();

				HashSet<int> visitedNodes = [];
				Queue<int> queuedNodes = [];
				List<List<int>> localRegionNodeIds = [];
				List<int> currentRegionNodeIds = [];
				localRegionNodeIds.Add( currentRegionNodeIds );

				foreach (Node node in tectonicState.Nodes) {
					int id = node.Vertex.Id;
					if (visitedNodes.Contains( id ))
						continue;

					visitedNodes.Add( id );
					queuedNodes.Enqueue( id );

					while (queuedNodes.TryDequeue( out int curId )) {
						Node n = globe.Nodes[ curId ];
						currentRegionNodeIds.Add( curId );
						foreach (Node nn in n.NeighbouringNodes) {
							int nnId = nn.Vertex.Id;
							if (visitedNodes.Contains( nnId ))
								continue;
							NodeTectonicLandscapeState landscapeState = nn.GetStateOrThrow<NodeTectonicLandscapeState>();
							if (landscapeState.Region != currentSphericalRegion)
								continue;
							currentRegionNodeIds.Add( nnId );
							visitedNodes.Add( nnId );
							queuedNodes.Enqueue( nnId );
						}
					}

					currentRegionNodeIds = [];
					localRegionNodeIds.Add( currentRegionNodeIds );
				}

				localRegionNodeIds.RemoveAll( p => p.Count == 0 );
				List<int> biggest = localRegionNodeIds.OrderByDescending( p => p.Count ).First();
				localRegionNodeIds.Remove( biggest );

				foreach (List<int> r in localRegionNodeIds)
					nodesToSeparate.Enqueue( r );
			}
		} );

		while (nodesToSeparate.TryDequeue( out List<int>? newRegionNodes )) {
			Vector3<float> center = Vector3<float>.Zero;
			foreach (int n in newRegionNodes) {
				center += globe.Nodes[ n ].Vertex.Vector;
			}
			center /= newRegionNodes.Count;
			center = center.Normalize<Vector3<float>, float>();
			SphericalVoronoiRegion newRegion = new( center );
			SphericalVoronoiRegionTectonicPlateState state = new();
			newRegion.AddState( state );
			regions.Add( newRegion );
			foreach (int n in newRegionNodes) {
				Node nodeInNewRegion = globe.Nodes[ n ];
				nodeInNewRegion.GetStateOrThrow<NodeTectonicLandscapeState>().Region = newRegion;
				state.Nodes.Add( nodeInNewRegion );
			}
		}
	}
}
