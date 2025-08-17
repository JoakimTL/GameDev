//using Civlike.World.State;
//using Civlike.World.TectonicGeneration.Landscape.Plates;
//using Civlike.World.TectonicGeneration.Landscape.States;
//using Civlike.World.TectonicGeneration.NoiseProviders;
//using Engine;
//using System.Numerics;

//namespace Civlike.World.TectonicGeneration.Landscape.Old;

//[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<PlateEdgeInitializationStep>]
//public sealed class PlateBoundaryDistanceAndAgeStep( TectonicGenerationParameters parameters ) : TectonicGlobeGenerationProcessingStepBase( parameters ) {
//	public override void Process( Globe globe ) {
//		List<SphericalVoronoiRegion> regions = globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions;
//		float radiusKm = (float) globe.RadiusKm;

//		int ridgeSeedCount = 0, ridgeSeedWithRate = 0;
//		double minRate = double.PositiveInfinity, maxRate = 0, sumRate = 0;

//		foreach (Node n in globe.Nodes) {
//			NodeTectonicLandscapeState s = n.GetStateOrThrow<NodeTectonicLandscapeState>();
//			if (s.IsRidgeSeed) {
//				ridgeSeedCount++;
//				if (s.RidgeHalfRateKmPerMa > 0) {
//					ridgeSeedWithRate++;
//					minRate = Math.Min( minRate, s.RidgeHalfRateKmPerMa );
//					maxRate = Math.Max( maxRate, s.RidgeHalfRateKmPerMa );
//					sumRate += s.RidgeHalfRateKmPerMa;
//				}
//			}
//		}

//		// Run per-plate in parallel; nodes belong to exactly one plate, so writes are thread-safe.
//		ParallelProcessing.Range( regions.Count, ( start, end, _ ) => {
//			for (int i = start; i < end; i++) {
//				SphericalVoronoiRegion region = regions[ i ];
//				SphericalVoronoiRegionTectonicPlateState plate = region.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();
//				List<Node> nodes = plate.Nodes;

//				// 1) RIDGE multi-source Dijkstra (also propagates nearest seed half-rate)
//				MultiSourceDijkstraRidge( nodes, region, out Dictionary<Node, float>? distRidge, out Dictionary<Node, float>? nearestHalfRate );

//				int distCount = distRidge.Count;
//				int rateCount = nearestHalfRate.Count;
//				int seedsOnPlate = nodes.Count( n => n.GetStateOrThrow<NodeTectonicLandscapeState>().IsRidgeSeed );
//				int withAge = nodes.Count( n => distRidge.ContainsKey( n ) && nearestHalfRate.ContainsKey( n ) );

//				// 2) CONVERGENT multi-source Dijkstra
//				MultiSourceDijkstra( nodes, region, seedPredicate: n => n.GetStateOrThrow<NodeTectonicLandscapeState>().IsConvergentSeed, out Dictionary<Node, float>? distConv );

//				// 3) Write back: distances (radians) and AgeMa
//				foreach (Node n in nodes) {
//					NodeTectonicLandscapeState s = n.GetStateOrThrow<NodeTectonicLandscapeState>();

//					// distances
//					s.DistanceToDivergentRadians = distRidge.TryGetValue( n, out float dRid ) ? dRid : float.PositiveInfinity;
//					s.DistanceToConvergentRadians = distConv.TryGetValue( n, out float dCon ) ? dCon : float.PositiveInfinity;

//					bool hasDR = distRidge.TryGetValue( n, out float dR );
//					bool hasHR = nearestHalfRate.TryGetValue( n, out float hr );
//					// age (only if a ridge is reachable and a valid half-rate propagated)
//					if (hasDR && hasHR && /*distRidge.TryGetValue( n, out float dR ) && nearestHalfRate.TryGetValue( n, out float hr ) &&*/ hr > 0f && float.IsFinite( dR )) {
//						float distKm = dR * radiusKm;
//						s.AgeMa = distKm / hr; // km / (km/Ma) = Ma
//					} else 						s.AgeMa = 0f; // or float.NaN if you prefer “unknown”
//				}
//			}
//		} );
//	}

//	// ---------- Dijkstra helpers ----------

//	// Ridge variant: seeds are IsRidgeSeed nodes; we also propagate the seed’s half-rate to each node.
//	private static void MultiSourceDijkstraRidge( IReadOnlyList<Node> plateNodes, SphericalVoronoiRegion plateRegion, out Dictionary<Node, float> distOutRadians, out Dictionary<Node, float> nearestHalfRateOutKmPerMa ) {
//		Dictionary<Node, float> dist = new( plateNodes.Count );
//		Dictionary<Node, float> nearRate = new( plateNodes.Count );

//		PriorityQueue<Node, float> pq = new();

//		// Init seeds
//		foreach (Node n in plateNodes) {
//			NodeTectonicLandscapeState s = n.GetStateOrThrow<NodeTectonicLandscapeState>();
//			if (!s.IsRidgeSeed)
//				continue;

//			dist[ n ] = 0f;                 // always enqueue for distance
//			pq.Enqueue( n, 0f );

//			float hr = (float) s.RidgeHalfRateKmPerMa;
//			if (hr > 0f)
//				nearRate[ n ] = hr;  // propagate only if positive
//		}

//		// No seeds? Return empty maps.
//		if (pq.Count == 0) {
//			distOutRadians = dist;
//			nearestHalfRateOutKmPerMa = nearRate;
//			return;
//		}

//		// Dijkstra over the plate’s node-subgraph (stay within this plate)
//		while (pq.TryDequeue( out Node? u, out float du )) {
//			if (!dist.TryGetValue( u, out float curD ) || du > curD)
//				continue;

//			NodeTectonicLandscapeState uState = u.GetStateOrThrow<NodeTectonicLandscapeState>();
//			foreach (Node v in u.NeighbouringNodes) {
//				NodeTectonicLandscapeState vs = v.GetStateOrThrow<NodeTectonicLandscapeState>();
//				if (vs.Region != plateRegion)
//					continue; // don't cross plate boundary

//				float w = ArcRadians( u.Vertex.Vector, v.Vertex.Vector );
//				float alt = du + w;

//				if (!dist.TryGetValue( v, out float dv ) || alt < dv) {
//					dist[ v ] = alt;
//					// propagate *the same seed's* half-rate along the shortest path
//					nearRate[ v ] = nearRate[ u ];
//					pq.Enqueue( v, alt );
//				}
//			}
//		}

//		distOutRadians = dist;
//		nearestHalfRateOutKmPerMa = nearRate;
//	}

//	// Generic multi-source Dijkstra (no payload propagation)
//	private static void MultiSourceDijkstra( IReadOnlyList<Node> plateNodes, SphericalVoronoiRegion plateRegion, Func<Node, bool> seedPredicate, out Dictionary<Node, float> distOutRadians ) {
//		Dictionary<Node, float> dist = new( plateNodes.Count );
//		PriorityQueue<Node, float> pq = new();

//		foreach (Node n in plateNodes) {
//			if (!seedPredicate( n ))
//				continue;
//			dist[ n ] = 0f;
//			pq.Enqueue( n, 0f );
//		}

//		if (pq.Count == 0) { distOutRadians = dist; return; }

//		while (pq.TryDequeue( out Node? u, out float du )) {
//			if (!dist.TryGetValue( u, out float curD ) || du > curD)
//				continue;

//			foreach (Node v in u.NeighbouringNodes) {
//				NodeTectonicLandscapeState vs = v.GetStateOrThrow<NodeTectonicLandscapeState>();
//				if (vs.Region != plateRegion)
//					continue;

//				float w = ArcRadians( u.Vertex.Vector, v.Vertex.Vector );
//				float alt = du + w;

//				if (!dist.TryGetValue( v, out float dv ) || alt < dv) {
//					dist[ v ] = alt;
//					pq.Enqueue( v, alt );
//				}
//			}
//		}

//		distOutRadians = dist;
//	}

//	private static float ArcRadians( in Vector3<float> a, in Vector3<float> b )
//		=> float.Acos( float.Clamp( a.Dot( b ), -1f, 1f ) );
//}
