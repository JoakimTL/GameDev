using Civlike.World.State;
using Civlike.World.TectonicGeneration.Landscape.Plates;
using Civlike.World.TectonicGeneration.Landscape.States;
using Civlike.World.TectonicGeneration.NoiseProviders;
using Engine;

namespace Civlike.World.TectonicGeneration.Landscape.Ages;

internal static class TectonicAgeUtils {
	public const float RateEpsKmPerMa = 1e-3f;

	// ------------- Geometry & indexing -------------

	public static Dictionary<Node, int> BuildIndex( IReadOnlyList<Node> nodes ) {
		var index = new Dictionary<Node, int>( nodes.Count );
		for (int i = 0; i < nodes.Count; i++)
			index[ nodes[ i ] ] = i;
		return index;
	}

	public static Vector3<float>[] PlateFlowVectors( Vector3<float> omegaA, IReadOnlyList<Node> nodes, float radiusKm ) {
		int n = nodes.Count;
		var uA = new Vector3<float>[ n ];
		for (int i = 0; i < n; i++) {
			var rHat = nodes[ i ].Vertex.Vector;
			uA[ i ] = radiusKm * omegaA.Cross( rHat ); // km/Ma
		}
		return uA;
	}

	public static float ArcRadians( in Vector3<float> a, in Vector3<float> b ) {
		float d = a.Dot( b );
		d = float.Clamp( d, -1f, 1f );
		return float.Acos( d );
	}

	public static Vector3<float> EdgeTangent( in Vector3<float> uVec, in Vector3<float> vVec ) {
		var rMid = (uVec + vVec).Normalize<Vector3<float>, float>();
		var chord = vVec - uVec;
		return (chord - chord.Dot( rMid ) * rMid).Normalize<Vector3<float>, float>();
	}

	// ------------- Dijkstra variants -------------

	/// Generic “stay-inside-plate” multi-source Dijkstra; distances in radians.
	public static Dictionary<Node, float> MultiSourceDijkstraWithinPlate(
		IReadOnlyList<Node> plateNodes,
		SphericalVoronoiRegion plateRegion,
		System.Func<Node, bool> seedPredicate ) {
		var dist = new Dictionary<Node, float>( plateNodes.Count );
		var pq = new PriorityQueue<Node, float>();

		foreach (var n in plateNodes)
			if (seedPredicate( n )) { dist[ n ] = 0f; pq.Enqueue( n, 0f ); }
		if (pq.Count == 0)
			return dist;

		while (pq.TryDequeue( out var u, out var du )) {
			if (!dist.TryGetValue( u, out float cur ) || du > cur)
				continue;
			var uVec = u.Vertex.Vector;

			foreach (var v in u.NeighbouringNodes) {
				var vs = v.GetStateOrThrow<NodeTectonicLandscapeState>();
				if (vs.Region != plateRegion)
					continue;

				float w = ArcRadians( uVec, v.Vertex.Vector );
				float alt = du + w;
				if (!dist.TryGetValue( v, out float dv ) || alt < dv) { dist[ v ] = alt; pq.Enqueue( v, alt ); }
			}
		}
		return dist;
	}

	/// Ridge variant: also forwards each seed’s half-rate payload along its shortest paths.
	public static void MultiSourceDijkstraRidgeWithPayload(
		IReadOnlyList<Node> plateNodes,
		SphericalVoronoiRegion plateRegion,
		out Dictionary<Node, float> distOutRadians,
		out Dictionary<Node, float> nearestHalfRateOutKmPerMa ) {
		var dist = new Dictionary<Node, float>( plateNodes.Count );
		var near = new Dictionary<Node, float>( plateNodes.Count );
		var pq = new PriorityQueue<Node, float>();

		foreach (var n in plateNodes) {
			var s = n.GetStateOrThrow<NodeTectonicLandscapeState>();
			if (!s.IsRidgeSeed)
				continue;

			dist[ n ] = 0f;
			pq.Enqueue( n, 0f );
			float hr = (float) s.RidgeHalfRateKmPerMa;
			if (hr > 0f)
				near[ n ] = hr;
		}
		if (pq.Count == 0) { distOutRadians = dist; nearestHalfRateOutKmPerMa = near; return; }

		while (pq.TryDequeue( out var u, out float du )) {
			if (!dist.TryGetValue( u, out float cur ) || du > cur)
				continue;
			var uVec = u.Vertex.Vector;

			foreach (var v in u.NeighbouringNodes) {
				var vs = v.GetStateOrThrow<NodeTectonicLandscapeState>();
				if (vs.Region != plateRegion)
					continue;

				float w = ArcRadians( uVec, v.Vertex.Vector );
				float alt = du + w;

				if (!dist.TryGetValue( v, out float dv ) || alt < dv) {
					dist[ v ] = alt;
					if (near.TryGetValue( u, out float hr ))
						near[ v ] = hr; // forward seed payload
					pq.Enqueue( v, alt );
				}
			}
		}

		distOutRadians = dist;
		nearestHalfRateOutKmPerMa = near;
	}

	/// Travel-time Dijkstra with custom speed/filters. Returns Ma per node.
	public static Dictionary<Node, float> TravelTimeFromSeeds(
		IReadOnlyList<Node> nodes,
		Dictionary<Node, int> indexOf,
		Vector3<float>[] uA,
		float radiusKm,
		System.Func<Node, bool> seedPredicate,
		System.Func<Node, Node, int, int, float, Vector3<float>, float> speedAlongEdgeKmPerMa, // (u,v,iu,iv,arc,tHat) -> speed
		System.Func<Node, Node, bool>? edgeFilter = null,        // return false to skip edge
		System.Func<Node, Node, bool>? backtrackGuard = null,    // return true to skip edge
		float rateEps = RateEpsKmPerMa ) {
		var time = new Dictionary<Node, float>( nodes.Count );
		var pq = new PriorityQueue<Node, float>();

		foreach (var n in nodes)
			if (seedPredicate( n )) { time[ n ] = 0f; pq.Enqueue( n, 0f ); }
		if (pq.Count == 0)
			return time;

		while (pq.TryDequeue( out var u, out float tu )) {
			if (!time.TryGetValue( u, out float cur ) || tu > cur)
				continue;

			int iu = indexOf[ u ];
			var uVec = u.Vertex.Vector;

			foreach (var v in u.NeighbouringNodes) {
				if (!indexOf.TryGetValue( v, out int iv ))
					continue;     // stay within plate
				if (edgeFilter != null && !edgeFilter( u, v ))
					continue;
				if (backtrackGuard != null && backtrackGuard( u, v ))
					continue;

				var vVec = v.Vertex.Vector;
				float arc = ArcRadians( uVec, vVec );
				var tHat = EdgeTangent( uVec, vVec );

				float speed = speedAlongEdgeKmPerMa( u, v, iu, iv, arc, tHat );
				if (speed < rateEps)
					speed = rateEps;

				float dt = (arc * radiusKm) / speed; // Ma
				float alt = tu + dt;
				if (!time.TryGetValue( v, out float tv ) || alt < tv) { time[ v ] = alt; pq.Enqueue( v, alt ); }
			}
		}
		return time;
	}

	// ------------- Smoothing -------------

	/// Jacobi smoothing with fixed seeds; returns the smoothed array (length = nodes.Count).
	public static float[] JacobiSmoothFixedSeeds(
		IReadOnlyList<Node> nodes,
		Dictionary<Node, int> indexOf,
		System.Func<Node, (bool isFixed, float fixedValue)> seedSelector,
		int iterations,
		float blendSelf,
		float defaultGuess ) {
		int N = nodes.Count;
		var x = new float[ N ];
		var y = new float[ N ];
		var fixedMask = new bool[ N ];

		// init
		double sum = 0;
		int cnt = 0;
		for (int i = 0; i < N; i++) {
			var (isFixed, value) = seedSelector( nodes[ i ] );
			fixedMask[ i ] = isFixed;
			if (isFixed) { x[ i ] = value; sum += value; cnt++; }
		}
		float guess = cnt > 0 ? (float) (sum / cnt) : defaultGuess;
		for (int i = 0; i < N; i++)
			if (!fixedMask[ i ])
				x[ i ] = guess;

		// Jacobi
		for (int it = 0; it < iterations; it++) {
			for (int i = 0; i < N; i++) {
				if (fixedMask[ i ]) { y[ i ] = x[ i ]; continue; }
				float s = 0;
				int c = 0;
				var node = nodes[ i ];
				foreach (var nb in node.NeighbouringNodes) {
					if (!indexOf.TryGetValue( nb, out int j ))
						continue;
					s += x[ j ];
					c++;
				}
				if (c == 0) { y[ i ] = x[ i ]; continue; }
				float avg = s / c;
				y[ i ] = blendSelf * x[ i ] + (1f - blendSelf) * avg;
			}
			// swap
			var tmp = x;
			x = y;
			y = tmp;
		}
		return x;
	}
}


[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<PlateEdgeInitializationStep>]
public sealed class PlateBoundaryDistanceAndAgeStep( TectonicGenerationParameters p ) : TectonicGlobeGenerationProcessingStepBase( p ) {
	public override void Process( Globe globe ) {
		var regions = globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions;
		float radiusKm = (float) globe.RadiusKm;

		ParallelProcessing.Range( regions.Count, ( start, end, _ ) => {
			for (int i = start; i < end; i++) {
				var region = regions[ i ];
				var plate = region.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();
				var nodes = plate.Nodes;

				// 1) Ridge distances + nearest half-rate payload
				TectonicAgeUtils.MultiSourceDijkstraRidgeWithPayload(
					nodes, region,
					out var distRidge,
					out var nearestHalfRate );

				// 2) Convergent distances
				var distConv = TectonicAgeUtils.MultiSourceDijkstraWithinPlate(
					nodes, region,
					seedPredicate: n => n.GetStateOrThrow<NodeTectonicLandscapeState>().IsConvergentSeed );

				// 3) Write back
				foreach (var n in nodes) {
					var s = n.GetStateOrThrow<NodeTectonicLandscapeState>();
					s.DistanceToDivergentRadians = distRidge.TryGetValue( n, out float dR ) ? dR : float.PositiveInfinity;
					s.DistanceToConvergentRadians = distConv.TryGetValue( n, out float dC ) ? dC : float.PositiveInfinity;

					bool hasDR = distRidge.TryGetValue( n, out float dR2 );
					bool hasHR = nearestHalfRate.TryGetValue( n, out float hr );
					if (hasDR && hasHR && hr > 0f && float.IsFinite( dR2 )) {
						float distKm = dR2 * radiusKm;
						s.AgeMa = distKm / hr;       // km / (km/Ma) = Ma
					} else {
						s.AgeMa = 0f;                // “unknown”
					}
				}
			}
		} );
	}
}

[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<PlateBoundaryDistanceAndAgeStep>]
public sealed class PlateAgeTravelTimeStep( TectonicGenerationParameters p ) : TectonicGlobeGenerationProcessingStepBase( p ) {
	// Tunables (same defaults you used)
	const int SmoothIterations = 64;
	const float BlendSelf = 0.0f; // 0..0.3
	const float Alpha = 1.0f; // 1=flow only; 0=rate only

	public override void Process( Globe globe ) {
		var regions = globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions;
		float radius = (float) globe.RadiusKm;

		ParallelProcessing.Range( regions.Count, ( start, end, _ ) => {
			for (int i = start; i < end; i++) {
				var region = regions[ i ];
				var plate = region.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();
				var nodes = plate.Nodes;
				if (nodes.Count == 0)
					continue;

				var indexOf = TectonicAgeUtils.BuildIndex( nodes );

				// ---- 1) Jacobi smoothing of ridge half-rates (Dirichlet at seeds)
				float[] smoothedHalfRate = TectonicAgeUtils.JacobiSmoothFixedSeeds(
					nodes,
					indexOf,
					seedSelector: n => {
						var s = n.GetStateOrThrow<NodeTectonicLandscapeState>();
						return (s.IsRidgeSeed && s.RidgeHalfRateKmPerMa > 0)
							? (true, (float) s.RidgeHalfRateKmPerMa)
							: (false, 0f);
					},
					iterations: SmoothIterations,
					blendSelf: BlendSelf,
					defaultGuess: 0f
				);

				// No seeds ⇒ nothing to do (keep Age=0)
				bool hasSeed = false;
				foreach (var n in nodes)
					if (n.GetStateOrThrow<NodeTectonicLandscapeState>().IsRidgeSeed) { hasSeed = true; break; }
				if (!hasSeed)
					continue;

				// ---- 2) Travel-time Dijkstra, speed = Alpha·|flow⋅tHat| + (1-Alpha)·(smoothed rate)
				var uA = TectonicAgeUtils.PlateFlowVectors( plate.AngularVelocity, nodes, radius );

				var time = TectonicAgeUtils.TravelTimeFromSeeds(
					nodes,
					indexOf,
					uA,
					radius,
					seedPredicate: n => n.GetStateOrThrow<NodeTectonicLandscapeState>().IsRidgeSeed,
					speedAlongEdgeKmPerMa: ( u, v, iu, iv, arc, tHat ) => {
						var flowEdge = 0.5f * (uA[ iu ] + uA[ iv ]);
						float vFlow = MathF.Abs( flowEdge.Dot( tHat ) );
						float vRate = 0.5f * (smoothedHalfRate[ iu ] + smoothedHalfRate[ iv ]);
						return Alpha * vFlow + (1f - Alpha) * vRate;
					},
					// backtrack guard: don't step “inward” toward the ridge
					backtrackGuard: ( u, v ) => {
						float dPrev = u.GetStateOrThrow<NodeTectonicLandscapeState>().DistanceToDivergentRadians;
						float dNext = v.GetStateOrThrow<NodeTectonicLandscapeState>().DistanceToDivergentRadians;
						const float inwardEps = 1e-2f;
						return float.IsFinite( dPrev ) && float.IsFinite( dNext ) && (dNext + inwardEps < dPrev);
					}
				);

				// ---- 3) Write AgeMa
				foreach (var n in nodes) {
					var s = n.GetStateOrThrow<NodeTectonicLandscapeState>();
					s.AgeMa = time.TryGetValue( n, out float t ) ? t : 0f;
				}
			}
		} );
	}
}


[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<PlateEdgeInitializationStep>]
public sealed class ContinentalPostRiftAgeStep( TectonicGenerationParameters p ) : TectonicGlobeGenerationProcessingStepBase( p ) {
	const float OceanicThicknessKm = 12f;

	public override void Process( Globe globe ) {
		var regions = globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions;
		float radius = (float) globe.RadiusKm;

		ParallelProcessing.Range( regions.Count, ( start, end, _ ) => {
			for (int i = start; i < end; i++) {
				var region = regions[ i ];
				var plate = region.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();
				var nodes = plate.Nodes;
				if (nodes.Count == 0)
					continue;

				// continental mask
				bool IsContinental( Node n ) => n.GetStateOrThrow<NodeTectonicLandscapeState>().CrustThicknessKm > OceanicThicknessKm;

				var indexOf = TectonicAgeUtils.BuildIndex( nodes );
				var uA = TectonicAgeUtils.PlateFlowVectors( plate.AngularVelocity, nodes, radius );

				// Seeds: continental ridge nodes; edges: continental→continental only; speed: |flow⋅tHat|
				var time = TectonicAgeUtils.TravelTimeFromSeeds(
					nodes,
					indexOf,
					uA,
					radius,
					seedPredicate: n => {
						var s = n.GetStateOrThrow<NodeTectonicLandscapeState>();
						return IsContinental( n ) && s.IsRidgeSeed;
					},
					speedAlongEdgeKmPerMa: ( u, v, iu, iv, arc, tHat ) => {
						var flowEdge = 0.5f * (uA[ iu ] + uA[ iv ]);
						return MathF.Abs( flowEdge.Dot( tHat ) );
					},
					edgeFilter: ( u, v ) => IsContinental( u ) && IsContinental( v )
				);

				foreach (var n in nodes) {
					var s = n.GetStateOrThrow<NodeTectonicLandscapeState>();
					s.PostRiftAgeMa = time.TryGetValue( n, out float t ) ? t : 0f;
				}
			}
		} );
	}
}