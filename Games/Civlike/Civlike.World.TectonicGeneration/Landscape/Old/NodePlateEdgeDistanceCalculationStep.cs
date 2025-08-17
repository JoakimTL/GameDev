using Civlike.World.State;
using Civlike.World.TectonicGeneration.Landscape.Plates;
using Civlike.World.TectonicGeneration.Landscape.States;
using Civlike.World.TectonicGeneration.NoiseProviders;
using Engine;
using System.Collections.Concurrent;
using System.Numerics;

namespace Civlike.World.TectonicGeneration.Landscape.Old;

//[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<PlatePropertySetupStep>]
//public sealed class NodePlateEdgeDistanceCalculationStep( TectonicGenerationParameters parameters ) : TectonicGlobeGenerationProcessingStepBase( parameters ) {
//	private static float GreatCircle( Vector3<float> a, Vector3<float> b ) {
//		float d = a.Dot( b );
//		if (d > 1f)
//			d = 1f;
//		else if (d < -1f)
//			d = -1f;
//		return MathF.Acos( d );
//	}

//	public override void Process( Globe globe ) {
//		List<SphericalVoronoiRegion> sphericalRegions = globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions;

//		ParallelProcessing.Range( sphericalRegions.Count, ( start, end, taskId ) => {
//			for (int i = start; i < end; i++) {
//				SphericalVoronoiRegion region = sphericalRegions[ i ];
//				SphericalVoronoiRegionTectonicPlateState? state = region.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();

//				PriorityQueue<Node, float> priorityQueue = new();

//				foreach (Node node in state.Nodes) {
//					//Nodes with neighbours in other regions should have a distance of 0, neighbours of those should have a distance equal to the great circle distance between them.
//					if (!node.NeighbouringNodes.Any( p => p.GetStateOrThrow<NodeTectonicLandscapeState>().Region != region )) {
//						node.GetStateOrThrow<NodeTectonicLandscapeState>().DistanceToPlateEdgeRadians = float.PositiveInfinity;
//						continue;
//					}

//					priorityQueue.Enqueue( node, 0 );
//					node.GetStateOrThrow<NodeTectonicLandscapeState>().DistanceToPlateEdgeRadians = 0;
//				}

//				while (priorityQueue.TryDequeue( out Node? node, out float distance )) {
//					Vector3<float> vector = node.Vertex.Vector;
//					foreach (Node nn in node.NeighbouringNodes) {
//						NodeTectonicLandscapeState? nnState = nn.GetStateOrThrow<NodeTectonicLandscapeState>();
//						if (nnState.Region != region)
//							continue;
//						float w = GreatCircle( vector, nn.Vertex.Vector );
//						distance += w;
//						if (nnState.DistanceToPlateEdgeRadians <= distance)
//							continue;
//						nnState.DistanceToPlateEdgeRadians = distance;
//						priorityQueue.Enqueue( nn, distance );
//					}
//				}
//			}
//		} );
//	}
//}