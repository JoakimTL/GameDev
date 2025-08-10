using Civlike.World.State;
using Civlike.World.TectonicGeneration.Landscape.States;
using Civlike.World.TectonicGeneration.NoiseProviders;
using Engine;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.ObjectModel;
using System.Net;
using System.Numerics;
using System.Threading;

namespace Civlike.World.TectonicGeneration.Landscape;

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

						nodeState.IsEdgeNode = true;
						if (!state.Edges.TryGetValue( neighbouringRegion, out PlateEdge? edge ))
							state.Edges.Add( neighbouringRegion, edge = new( state, neighbouringRegion.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>() ) );
						edge.Nodes.Add( n );
					}
				}
			}
		} );
	}
}
[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<PlateEdgeDetectionStep>]
public sealed class PlateEdgeInitializationStep( TectonicGenerationParameters parameters ) : TectonicGlobeGenerationProcessingStepBase( parameters ) {

	private readonly struct Acc {

		public readonly float Sum;
		public readonly int Count;

		public Acc( float sum, int count ) : this() {
			this.Sum = sum;
			this.Count = count;
		}

		public Acc Add( float v ) {
			return new( Sum + v, Count + 1 );
		}

		public static Acc Merge( Acc a, Acc b ) {
			return new( a.Sum + b.Sum, a.Count + b.Count );
		}
	}

	public override void Process( Globe globe ) {
		List<SphericalVoronoiRegion> regions = globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions;
		ConcurrentBag<Dictionary<Node, Acc>> partials = [];
		FrozenDictionary<SphericalVoronoiRegion, int> regionIds = regions
			.Select( ( p, i ) => (p, i) )
			.ToDictionary( p => p.p, p => p.i )
			.ToFrozenDictionary();

		float radiusKm = (float) globe.Radius / 1000f;

		ParallelProcessing.Range( regions.Count, ( start, end, taskId ) => {
			Dictionary<Node, Acc> local = new( capacity: 4096 );
			HashSet<ulong> visited = []; // de-dup (minId<<32 | maxId)
			for (int i = start; i < end; i++) {
				SphericalVoronoiRegion region = regions[ i ];
				SphericalVoronoiRegionTectonicPlateState state = region.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();
				int regionId = regionIds[ region ];
				Vector3<float> omegaA = state.AngularVelocity; // Vector3 (rad/Ma)

				foreach (PlateEdge e in state.Edges.Values) {
					if (regionIds[ e.Neighbour.StateContainer ] <= regionId)
						continue;

					Vector3<float> omegaB = e.Neighbour.AngularVelocity;  // Vector3 (rad/Ma)
					foreach (Node n in e.Nodes) {
						foreach (Node neighbouringNode in n.NeighbouringNodes) {
							NodeTectonicLandscapeState neighbouringNodeState = neighbouringNode.GetStateOrThrow<NodeTectonicLandscapeState>();

							if (neighbouringNodeState.Region != e.Neighbour.StateContainer)
								continue;

							uint aId = (uint) n.Vertex.Id, bId = (uint) neighbouringNode.Vertex.Id;
							ulong key = aId < bId ? (((ulong) aId) << 32) | bId : (((ulong) bId) << 32) | aId;
							if (!visited.Add( key ))
								continue;

							// --- per-pair midpoint & frame ---
							Vector3<float> a = n.Vertex.Vector;    // unit vector r̂_a
							Vector3<float> b = neighbouringNode.Vertex.Vector;   // unit vector r̂_b
							Vector3<float> rMid = (a + b).Normalize<Vector3<float>, float>();                 // r̂
							Vector3<float> chord = b - a;
							Vector3<float> acrossHat = (chord - chord.Dot( rMid ) * rMid).Normalize<Vector3<float>, float>(); // across-edge
							Vector3<float> alongHat = rMid.Cross( acrossHat ).Normalize<Vector3<float>, float>(); // along-edge

							Vector3<float> vRel = radiusKm * (omegaB - omegaA).Cross( rMid ); // km/Ma
							float vN = vRel.Dot( acrossHat ); // normal (signed)
							float vT = vRel.Dot( alongHat ); // along-edge

							float full = float.Abs( vN );
							float tol = float.Max( 0.5f, 0.05f * vRel.Magnitude<Vector3<float>, float>() ); // example: 0.5 km/Ma or 5% of |v_rel|
							BoundaryType type = (full <= tol) ? BoundaryType.Transform : (vN > 0 ? BoundaryType.Divergent : BoundaryType.Convergent);

							//TODO store more values from type https://chatgpt.com/g/g-p-684d6a55d0e08191a2b6b1b20addfb2c-4x-rt-gs-game/c/6897d49b-51f4-832c-9aa8-915550018c47
							if (type != BoundaryType.Divergent)
								continue;
							float half = 0.5f * full;

							if (!local.TryGetValue( n, out Acc accN ))
								accN = default;
							local[ n ] = accN.Add( half );

							if (!local.TryGetValue( neighbouringNode, out Acc accNN ))
								accNN = default;
							local[ neighbouringNode ] = accNN.Add( half );

						}
					}
				}
			}

			partials.Add( local );
		} );

		// --- single-threaded merge ---
		Dictionary<Node, Acc> merged = new( globe.Nodes.Count );
		foreach (Dictionary<Node, Acc> dict in partials) {
			foreach (KeyValuePair<Node, Acc> kv in dict) {
				if (merged.TryGetValue( kv.Key, out Acc acc )) {
					merged[ kv.Key ] = Acc.Merge( acc, kv.Value );
				} else
					merged[ kv.Key ] = kv.Value;
			}
		}

		// --- write once to node state ---
		foreach ((Node node, Acc acc) in merged)
			node.GetStateOrThrow<NodeTectonicLandscapeState>().RidgeHalfRateKmPerMa = acc.Count > 0 ? acc.Sum / acc.Count : 0;
	}
}

public enum BoundaryType {
	Convergent,
	Divergent,
	Transform
}