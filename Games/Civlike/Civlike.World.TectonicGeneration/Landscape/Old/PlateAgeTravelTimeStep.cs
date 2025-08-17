//using Civlike.World.State;
//using Civlike.World.TectonicGeneration.Landscape.States;
//using Engine;

//namespace Civlike.World.TectonicGeneration.Landscape.Old;

//[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<PlateBoundaryDistanceAndAgeStep>]
//public sealed class PlateAgeTravelTimeStep( TectonicGenerationParameters p ) : TectonicGlobeGenerationProcessingStepBase( p ) {

//	// Tunables
//	const int SmoothIterations = 64;     // Jacobi smoothing passes
//	const float RateEpsKmPerMa = 1e-3f;  // avoid divide-by-zero
//	const float BlendSelf = 0.0f;   // 0..0.3 keeps a little of previous value (damps ringing)

//	public override void Process( Globe globe ) {
//		List<NoiseProviders.SphericalVoronoiRegion> regions = globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions;
//		float radiusKm = (float) globe.RadiusKm;

//		ParallelProcessing.Range( regions.Count, ( start, end, _ ) => {
//			for (int i = start; i < end; i++) {
//				NoiseProviders.SphericalVoronoiRegion region = regions[ i ];
//				SphericalVoronoiRegionTectonicPlateState plate = region.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();
//				List<Node> nodes = plate.Nodes;
//				int N = nodes.Count;
//				if (N == 0)
//					continue;

//				// Build local index map
//				Dictionary<Node, int> indexOf = new Dictionary<Node, int>( N );
//				for (int k = 0; k < N; k++)
//					indexOf[ nodes[ k ] ] = k;

//				// ---- 1) Seed-fixed smoothing of half-rate into the plate ----
//				float[] rate = new float[ N ];
//				float[] next = new float[ N ];
//				bool[] isFixed = new bool[ N ];

//				// initial guess = plate mean of seed rates (helps convergence)
//				double sumSeed = 0;
//				int cntSeed = 0;
//				for (int k = 0; k < N; k++) {
//					NodeTectonicLandscapeState s = nodes[ k ].GetStateOrThrow<NodeTectonicLandscapeState>();
//					if (s.IsRidgeSeed && s.RidgeHalfRateKmPerMa > 0) {
//						isFixed[ k ] = true;
//						rate[ k ] =  s.RidgeHalfRateKmPerMa;
//						sumSeed += s.RidgeHalfRateKmPerMa;
//						cntSeed++;
//					}
//				}
//				float guess = cntSeed > 0 ? (float) (sumSeed / cntSeed) : 0f;
//				if (cntSeed == 0) 					// No ridge on this plate → nothing to do; leave Age at 0
//					continue;
//				for (int k = 0; k < N; k++)
//					if (!isFixed[ k ])
//						rate[ k ] = guess;

//				// Jacobi smoothing (Dirichlet on seeds)
//				for (int it = 0; it < SmoothIterations; it++) {
//					for (int k = 0; k < N; k++) {
//						if (isFixed[ k ]) { next[ k ] = rate[ k ]; continue; }

//						Node node = nodes[ k ];
//						float sum = 0f;
//						int cnt = 0;
//						foreach (Node nb in node.NeighbouringNodes) {
//							if (!indexOf.TryGetValue( nb, out int j ))
//								continue; // stay in plate
//							sum += rate[ j ];
//							cnt++;
//						}
//						if (cnt == 0) { next[ k ] = rate[ k ]; continue; }

//						float avg = sum / cnt;
//						next[ k ] = BlendSelf * rate[ k ] + (1f - BlendSelf) * avg;
//					}
//					// swap
//					float[] tmp = rate;
//					rate = next;
//					next = tmp;
//				}

//				// ---- 2) Weighted multi-source Dijkstra (travel time) ----
//				// time seeds: all ridge seeds (time=0)

//				Vector3<float> omegaA = plate.AngularVelocity; // rad/Ma
//				Vector3<float>[] uA = new Vector3<float>[ N ];
//				for (int k = 0; k < N; k++) {
//					Vector3<float> rHat = nodes[ k ].Vertex.Vector;
//					uA[ k ] = radiusKm * omegaA.Cross( rHat ); // km/Ma
//				}

//				Dictionary<Node, float> time = new Dictionary<Node, float>( N );
//				PriorityQueue<Node, float> pq = new PriorityQueue<Node, float>();

//				for (int k = 0; k < N; k++) {
//					NodeTectonicLandscapeState s = nodes[ k ].GetStateOrThrow<NodeTectonicLandscapeState>();
//					if (!s.IsRidgeSeed)
//						continue;
//					time[ nodes[ k ] ] = 0f;
//					pq.Enqueue( nodes[ k ], 0f );
//				}

//				const float RateEpsKmPerMa = 1e-3f;  // keep your floor
//				const float Alpha = 1.0f;            // 1=use plate flow only; 0=use smoothed half-rate only

//				while (pq.TryDequeue( out Node? u, out float tu )) {
//					if (!time.TryGetValue( u, out float curT ) || tu > curT)
//						continue;

//					int iu = indexOf[ u ];
//					Vector3<float> uVec = u.Vertex.Vector;

//					foreach (Node v in u.NeighbouringNodes) {
//						if (!indexOf.TryGetValue( v, out int iv ))
//							continue;

//						// edge length (radians)
//						float arc = ArcRadians( uVec, v.Vertex.Vector );

//						// edge tangent in the tangent plane at midpoint
//						Vector3<float> rMid = (uVec + v.Vertex.Vector).Normalize<Vector3<float>, float>();
//						Vector3<float> chord = v.Vertex.Vector - uVec;
//						Vector3<float> tHat = (chord - chord.Dot( rMid ) * rMid).Normalize<Vector3<float>, float>();

//						// plate-flow speed along the edge (km/Ma)
//						Vector3<float> flowEdge = 0.5f * (uA[ iu ] + uA[ iv ]);
//						float vFlow = MathF.Abs( flowEdge.Dot( tHat ) );

//						// optional blend with your smoothed half-rate magnitude
//						float vRate = 0.5f * (rate[ iu ] + rate[ iv ]);          // from your step (1)
//						float speed = MathF.Max( RateEpsKmPerMa, Alpha * vFlow + (1f - Alpha) * vRate );

//						float dPrev = u.GetStateOrThrow<NodeTectonicLandscapeState>().DistanceToDivergentRadians;
//						float dNext = v.GetStateOrThrow<NodeTectonicLandscapeState>().DistanceToDivergentRadians;
//						if (float.IsFinite( dPrev ) && float.IsFinite( dNext )) {
//							const float inwardEps = 1e-2f;           // radians (~meters at globe scale)
//							if (dNext + inwardEps < dPrev)
//								continue; // don’t step back toward the ridge
//						}

//						float dt = arc * radiusKm / speed;  // Ma
//						float alt = tu + dt;

//						if (!time.TryGetValue( v, out float tv ) || alt < tv) {
//							time[ v ] = alt;
//							pq.Enqueue( v, alt );
//						}
//					}
//				}

//				// ---- 3) Write AgeMa (travel time) & keep distances if you want ----
//				foreach (Node n in nodes) {
//					NodeTectonicLandscapeState s = n.GetStateOrThrow<NodeTectonicLandscapeState>();
//					if (time.TryGetValue( n, out float t ))
//						s.AgeMa = t;               // Ma
//					else
//						s.AgeMa = 0f;              // unreachable (no ridge on this plate)
//				}
//			}
//		} );
//	}

//	private static float ArcRadians( in Vector3<float> a, in Vector3<float> b ) {
//		float d = a.Dot( b );
//		d = float.Clamp( d, -1f, 1f );
//		return float.Acos( d );
//	}
//}

//[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<PlateEdgeInitializationStep>]
//public sealed class PlateAgeAdvectionStep : TectonicGlobeGenerationProcessingStepBase {
//	public PlateAgeAdvectionStep( TectonicGenerationParameters parameters ) : base( parameters ) { }

//	// --- Tunables ---
//	const float RateEpsKmPerMa = 1e-3f;   // floor to avoid divide-by-zero (≈ 1 m/Ma)
//	const float MaxAgeCapMa = 220f;    // optional visual cap; set to float.PositiveInfinity to disable

//	public override void Process( Globe globe ) {
//		var regions = globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions;
//		float radiusKm = (float) globe.RadiusKm;

//		// Per-plate parallelism is safe (nodes are unique to a plate).
//		ParallelProcessing.Range( regions.Count, ( start, end, _ ) => {
//			for (int i = start; i < end; i++) {
//				var region = regions[ i ];
//				var plate = region.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();
//				var nodes = plate.Nodes;
//				int N = nodes.Count;
//				if (N == 0)
//					continue;

//				// Build local index map for O(1) "stay within this plate" checks
//				var idxOf = new Dictionary<Node, int>( N );
//				for (int k = 0; k < N; k++)
//					idxOf[ nodes[ k ] ] = k;

//				// Precompute plate flow vector at each node: uA = R * (ωA × r̂) [km/Ma]
//				var uA = new Vector3<float>[ N ];
//				var omegaA = plate.AngularVelocity;  // rad/Ma
//				for (int k = 0; k < N; k++) {
//					var rHat = nodes[ k ].Vertex.Vector;
//					uA[ k ] = radiusKm * omegaA.Cross( rHat ); // km/Ma
//				}

//				// Multi-source Dijkstra in "time" with edge cost dt = (arc_km)/(speed_along_edge)
//				var time = new Dictionary<Node, float>( N );
//				var pq = new PriorityQueue<Node, float>();

//				// Seeds: all ridge nodes, Age=0 at the ridge
//				int seedCount = 0;
//				for (int k = 0; k < N; k++) {
//					var s = nodes[ k ].GetStateOrThrow<NodeTectonicLandscapeState>();
//					if (!s.IsRidgeSeed)
//						continue;
//					time[ nodes[ k ] ] = 0f;
//					pq.Enqueue( nodes[ k ], 0f );
//					seedCount++;
//				}
//				if (seedCount == 0)
//					continue; // no ridge on this plate; leave Age as-is (0)

//				while (pq.TryDequeue( out var u, out var tu )) {
//					if (!time.TryGetValue( u, out var curT ) || tu > curT)
//						continue;

//					int iu = idxOf[ u ];
//					var uVec = u.Vertex.Vector;

//					foreach (var v in u.NeighbouringNodes) {
//						if (!idxOf.TryGetValue( v, out int iv ))
//							continue; // don't leave plate
//						var vVec = v.Vertex.Vector;

//						// Geodesic arc (radians) and tangent direction at midpoint
//						float arc = ArcRadians( uVec, vVec );
//						var rMid = (uVec + vVec).Normalize<Vector3<float>, float>();
//						var chord = vVec - uVec;
//						var tHat = (chord - chord.Dot( rMid ) * rMid).Normalize<Vector3<float>, float>(); // along-edge direction in the tangent plane

//						// Flow along this edge = projection of average flow onto edge direction
//						var flowEdge = 0.5f * (uA[ iu ] + uA[ iv ]);
//						float speedAlong = MathF.Abs( flowEdge.Dot( tHat ) ); // km/Ma
//						if (speedAlong < RateEpsKmPerMa)
//							speedAlong = RateEpsKmPerMa;

//						float dt = (arc * radiusKm) / speedAlong; // Ma
//						float alt = tu + dt;

//						if (!time.TryGetValue( v, out var tv ) || alt < tv) {
//							time[ v ] = alt;
//							pq.Enqueue( v, alt );
//						}
//					}
//				}

//				// Write AgeMa
//				foreach (var n in nodes) {
//					var s = n.GetStateOrThrow<NodeTectonicLandscapeState>();
//					if (time.TryGetValue( n, out var t ))
//						s.AgeMa = (MaxAgeCapMa < float.PositiveInfinity) ? MathF.Min( t, MaxAgeCapMa ) : t;
//					else
//						s.AgeMa = 0f; // unreachable (shouldn’t happen with seeds present)
//				}
//			}
//		} );
//	}

//	private static float ArcRadians( in Vector3<float> a, in Vector3<float> b ) {
//		float d = a.Dot( b );
//		d = float.Clamp( d, -1f, 1f );
//		return float.Acos( d );
//	}
//}