using Civlike.World.State;
using Civlike.World.TectonicGeneration.Landscape.States;
using Civlike.World.TectonicGeneration.NoiseProviders;
using Engine;
using System.Collections.Concurrent;
using System.Numerics;

namespace Civlike.World.TectonicGeneration.Landscape;

[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<PlatePropertySetupStep>]
public sealed class NodePlateEdgeDistanceCalculationStep( TectonicGenerationParameters parameters ) : TectonicGlobeGenerationProcessingStepBase( parameters ) {
	private static float GreatCircle( Vector3<float> a, Vector3<float> b ) {
		float d = a.Dot( b );
		if (d > 1f)
			d = 1f;
		else if (d < -1f)
			d = -1f;
		return MathF.Acos( d );
	}

	public override void Process( Globe globe ) {
		List<SphericalVoronoiRegion> sphericalRegions = globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions;

		ParallelProcessing.Range( sphericalRegions.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				SphericalVoronoiRegion region = sphericalRegions[ i ];
				SphericalVoronoiRegionTectonicPlateState? state = region.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();

				PriorityQueue<Node, float> priorityQueue = new();

				foreach (Node node in state.Nodes) {
					//Nodes with neighbours in other regions should have a distance of 0, neighbours of those should have a distance equal to the great circle distance between them.
					if (!node.NeighbouringNodes.Any( p => p.GetStateOrThrow<NodeTectonicLandscapeState>().Region != region )) {
						node.GetStateOrThrow<NodeTectonicLandscapeState>().DistanceToPlateEdgeRadians = float.PositiveInfinity;
						continue;
					}

					priorityQueue.Enqueue( node, 0 );
					node.GetStateOrThrow<NodeTectonicLandscapeState>().DistanceToPlateEdgeRadians = 0;
				}

				while (priorityQueue.TryDequeue( out Node? node, out float distance )) {
					Vector3<float> vector = node.Vertex.Vector;
					foreach (Node nn in node.NeighbouringNodes) {
						NodeTectonicLandscapeState? nnState = nn.GetStateOrThrow<NodeTectonicLandscapeState>();
						if (nnState.Region != region)
							continue;
						float w = GreatCircle( vector, nn.Vertex.Vector );
						distance += w;
						if (nnState.DistanceToPlateEdgeRadians <= distance)
							continue;
						nnState.DistanceToPlateEdgeRadians = distance;
						priorityQueue.Enqueue( nn, distance );
					}
				}
			}
		} );
	}
}

//public static List<BoundaryEdge> ClassifyBoundaryEdges(
//	IReadOnlyDictionary<int, List<INode>> edgeNodesByPlate,   // only edge nodes per plate
//	IReadOnlyDictionary<int, Vector3> omegaByPlate,           // Ω·k̂ for each plate (rad/Ma)
//	double radiusKm,
//	double transformTolFrac = 0.05,
//	double minTolKmPerMa = 0.5 ) {
//	var edges = new ConcurrentDictionary<(int, int), BoundaryEdge>();

//	// For each plate's edge node, visit only across-plate neighbours; de-dup with (min,max).
//	System.Threading.Tasks.Parallel.ForEach( edgeNodesByPlate, kv => {
//		foreach (var n in kv.Value) {
//			foreach (var nn in n.Neighbours) {
//				if (nn.PlateId == n.PlateId)
//					continue; // only cross-plate

//				var key = (Math.Min( n.Id, nn.Id ), Math.Max( n.Id, nn.Id ));
//				if (edges.ContainsKey( key ))
//					continue;   // someone else built it

//				// Geometry at edge midpoint
//				var rMid = Vector3.Normalize( n.Unit + nn.Unit );
//				var along = Vector3.Normalize( nn.Unit - n.Unit );
//				var tHat = Vector3.Normalize( along - Vector3.Dot( along, rMid ) * rMid );
//				var nHat = Vector3.Normalize( Vector3.Cross( rMid, tHat ) ); // tangent normal

//				// v_rel = R * ((Ω_B - Ω_A) × r̂_mid)
//				var wA = omegaByPlate[ n.PlateId ];
//				var wB = omegaByPlate[ nn.PlateId ];
//				var vRel = radiusKm * Vector3.Cross( wB - wA, rMid );     // km/Ma

//				double vn = Vector3.Dot( vRel, nHat );                    // normal component
//				double spd = Math.Abs( vn );
//				double tol = Math.Max( minTolKmPerMa, transformTolFrac * vRel.Length() );

//				BoundaryType type = (spd <= tol) ? BoundaryType.Transform
//								  : (vn > 0 ? BoundaryType.Divergent : BoundaryType.Convergent);

//				edges.TryAdd( key, new BoundaryEdge( n, nn, n.PlateId, nn.PlateId, type, spd ) );
//			}
//		}
//	} );

//	return edges.Values.ToList();
////}