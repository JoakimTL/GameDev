using Civlike.World.State;
using Civlike.World.TectonicGeneration.Landscape.States;
using Civlike.World.TectonicGeneration.NoiseProviders;
using Engine;
using Engine.Logging;
using System.Collections.Concurrent;

namespace Civlike.World.TectonicGeneration.Landscape.Plates;

[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<NodePlateAssigningStep>]
public sealed class PlateSeparationStep( TectonicGenerationParameters parameters ) : TectonicGlobeGenerationProcessingStepBase( parameters ) {
	public override void Process( Globe globe ) {
		List<SphericalVoronoiRegion> regions = globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions;

		// Keep both the component and the region it came from so we can cleanly remove nodes.
		ConcurrentQueue<(SphericalVoronoiRegion FromRegion, List<int> NodeIds)> splits = new ConcurrentQueue<(SphericalVoronoiRegion FromRegion, List<int> NodeIds)>();

		ParallelProcessing.Range( regions.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				SphericalVoronoiRegion region = regions[ i ];
				SphericalVoronoiRegionTectonicPlateState plateState = region.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();

				HashSet<int> visited = new HashSet<int>();
				List<List<int>> components = new List<List<int>>();

				// Walk only nodes that are still mapped to this region.
				foreach (Node node in plateState.Nodes) {
					int id = node.Vertex.Id;
					if (visited.Contains( id ))
						continue;

					NodeTectonicLandscapeState ls = node.GetStateOrThrow<NodeTectonicLandscapeState>();
					if (ls.Region != region)
						continue; // stale membership; we'll fix after the split

					Queue<int> q = new Queue<int>();
					List<int> comp = new List<int>();

					visited.Add( id );
					q.Enqueue( id );
					comp.Add( id ); // add once, when we mark visited

					while (q.Count > 0) {
						int curId = q.Dequeue();
						Node cur = globe.Nodes[ curId ];

						foreach (Node nn in cur.NeighbouringNodes) {
							int nnId = nn.Vertex.Id;
							if (visited.Contains( nnId ))
								continue;

							NodeTectonicLandscapeState nls = nn.GetStateOrThrow<NodeTectonicLandscapeState>();
							if (nls.Region != region)
								continue;

							visited.Add( nnId );
							q.Enqueue( nnId );
							comp.Add( nnId ); // add once at discovery time
						}
					}

					components.Add( comp );
				}

				if (components.Count <= 1)
					continue;

				// Keep the largest piece in the original region.
				List<int> largest = components.OrderByDescending( c => c.Count ).First();
				foreach (List<int> comp in components) {
					if (ReferenceEquals( comp, largest ) || comp.Count == 0)
						continue;

					// Queue this component to become a new region.
					splits.Enqueue( (region, comp) );
				}
			}
		} );

		// Apply splits serially: create new regions, move nodes, and clean old membership.
		while (splits.TryDequeue( out (SphericalVoronoiRegion FromRegion, List<int> NodeIds) split )) {
			(SphericalVoronoiRegion fromRegion, List<int> nodeIds) = split;

			// Compute a reasonable center for the new region.
			Vector3<float> center = Vector3<float>.Zero;
			foreach (int id in nodeIds)
				center += globe.Nodes[ id ].Vertex.Vector;

			center /= nodeIds.Count;
			center = center.Normalize<Vector3<float>, float>();

			SphericalVoronoiRegion newRegion = new SphericalVoronoiRegion( center );
			SphericalVoronoiRegionTectonicPlateState newState = new SphericalVoronoiRegionTectonicPlateState();
			newRegion.AddState( newState );
			regions.Add( newRegion );

			SphericalVoronoiRegionTectonicPlateState fromState = fromRegion.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();

			// Move nodes: remove from old, assign to new. Use Distinct in case a prior bug added dupes.
			foreach (int id in nodeIds.Distinct()) {
				Node node = globe.Nodes[ id ];

				// Remove from source state (prevents stale membership).
				fromState.Nodes.Remove( node );

				// Reassign landscape region and add to new state's membership.
				node.GetStateOrThrow<NodeTectonicLandscapeState>().Region = newRegion;
				newState.Nodes.Add( node );
			}
		}

#if DEBUG
		// Optional sanity checks — great for catching double-membership or stale regions.
		HashSet<Node> seen = new HashSet<Node>();
		foreach (SphericalVoronoiRegion r in regions) {
			SphericalVoronoiRegionTectonicPlateState s = r.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();
			foreach (Node n in s.Nodes) {
				System.Diagnostics.Debug.Assert( n.GetStateOrThrow<NodeTectonicLandscapeState>().Region == r,
					"Landscape region pointer out of sync with region membership." );
				if (!seen.Add( n ))
					System.Diagnostics.Debug.Fail( "Node appears in multiple regions." );
			}
		}
#endif
	}
}

//[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<NodePlateAssigningStep>]
//public sealed class PlateSeparationStep( TectonicGenerationParameters parameters ) : TectonicGlobeGenerationProcessingStepBase( parameters ) {
//	public override void Process( Globe globe ) {
//		List<SphericalVoronoiRegion> regions = globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions;

//		ConcurrentQueue<List<int>> nodesToSeparate = new();

//		ParallelProcessing.Range( regions.Count, ( start, end, taskId ) => {
//			for (int i = start; i < end; i++) {
//				SphericalVoronoiRegion currentSphericalRegion = regions[ i ];
//				SphericalVoronoiRegionTectonicPlateState tectonicState = currentSphericalRegion.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();

//				HashSet<int> visitedNodes = [];
//				Queue<int> queuedNodes = [];
//				List<List<int>> localRegionNodeIds = [];
//				List<int> currentRegionNodeIds = [];
//				localRegionNodeIds.Add( currentRegionNodeIds );

//				foreach (Node node in tectonicState.Nodes) {
//					int id = node.Vertex.Id;
//					if (visitedNodes.Contains( id ))
//						continue;

//					visitedNodes.Add( id );
//					queuedNodes.Enqueue( id );

//					while (queuedNodes.TryDequeue( out int curId )) {
//						Node n = globe.Nodes[ curId ];
//						currentRegionNodeIds.Add( curId );
//						foreach (Node nn in n.NeighbouringNodes) {
//							int nnId = nn.Vertex.Id;
//							if (visitedNodes.Contains( nnId ))
//								continue;
//							NodeTectonicLandscapeState landscapeState = nn.GetStateOrThrow<NodeTectonicLandscapeState>();
//							if (landscapeState.Region != currentSphericalRegion)
//								continue;
//							currentRegionNodeIds.Add( nnId );
//							visitedNodes.Add( nnId );
//							queuedNodes.Enqueue( nnId );
//						}
//					}

//					currentRegionNodeIds = [];
//					localRegionNodeIds.Add( currentRegionNodeIds );
//				}

//				localRegionNodeIds.RemoveAll( p => p.Count == 0 );
//				List<int> biggest = localRegionNodeIds.OrderByDescending( p => p.Count ).First();
//				localRegionNodeIds.Remove( biggest );

//				foreach (List<int> r in localRegionNodeIds)
//					nodesToSeparate.Enqueue( r );
//			}
//		} );

//		while (nodesToSeparate.TryDequeue( out List<int>? newRegionNodes )) {
//			Vector3<float> center = Vector3<float>.Zero;
//			foreach (int n in newRegionNodes) {
//				center += globe.Nodes[ n ].Vertex.Vector;
//			}
//			center /= newRegionNodes.Count;
//			center = center.Normalize<Vector3<float>, float>();
//			SphericalVoronoiRegion newRegion = new( center );
//			SphericalVoronoiRegionTectonicPlateState state = new();
//			newRegion.AddState( state );
//			regions.Add( newRegion );
//			foreach (int n in newRegionNodes) {
//				Node nodeInNewRegion = globe.Nodes[ n ];
//				nodeInNewRegion.GetStateOrThrow<NodeTectonicLandscapeState>().Region = newRegion;
//				state.Nodes.Add( nodeInNewRegion );
//			}
//		}
//	}
//}
