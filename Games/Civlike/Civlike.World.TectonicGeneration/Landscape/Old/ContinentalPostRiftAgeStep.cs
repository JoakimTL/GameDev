//using Civlike.World.State;
//using Civlike.World.TectonicGeneration.Landscape.Plates;
//using Civlike.World.TectonicGeneration.Landscape.States;
//using Civlike.World.TectonicGeneration.NoiseProviders;
//using Engine;

//namespace Civlike.World.TectonicGeneration.Landscape.Old;

//[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<PlateEdgeInitializationStep>]
//public sealed class ContinentalPostRiftAgeStep( TectonicGenerationParameters p ) : TectonicGlobeGenerationProcessingStepBase( p ) {
//	const float OceanicThicknessKm = 12f;
//	const float RateEpsKmPerMa = 1e-3f;

//	public override void Process( Globe globe ) {
//		List<SphericalVoronoiRegion> regions = globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions;
//		float radiusKm = (float) globe.RadiusKm;

//		ParallelProcessing.Range( regions.Count, ( start, end, _ ) => {
//			for (int i = start; i < end; i++) {
//				SphericalVoronoiRegion region = regions[ i ];
//				SphericalVoronoiRegionTectonicPlateState plate = region.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();
//				List<Node> nodes = plate.Nodes;
//				if (nodes.Count == 0)
//					continue;

//				// local index
//				Dictionary<Node, int> idxOf = new Dictionary<Node, int>( nodes.Count );
//				for (int k = 0; k < nodes.Count; k++)
//					idxOf[ nodes[ k ] ] = k;

//				// plate flow at nodes: uA = R * (ωA × r̂)
//				Vector3<float>[] uA = new Vector3<float>[ nodes.Count ];
//				Vector3<float> omegaA = plate.AngularVelocity; // rad/Ma
//				for (int k = 0; k < nodes.Count; k++)
//					uA[ k ] = radiusKm * omegaA.Cross( nodes[ k ].Vertex.Vector ); // km/Ma

//				// multi-source Dijkstra in "time"
//				Dictionary<Node, float> time = new Dictionary<Node, float>( nodes.Count );
//				PriorityQueue<Node, float> pq = new PriorityQueue<Node, float>();

//				// seeds: continental nodes that were flagged as ridge seeds
//				int seeds = 0;
//				foreach (Node n in nodes) {
//					NodeTectonicLandscapeState s = n.GetStateOrThrow<NodeTectonicLandscapeState>();
//					bool isContinental = s.CrustThicknessKm > OceanicThicknessKm;
//					if (isContinental && s.IsRidgeSeed) {
//						time[ n ] = 0f;
//						pq.Enqueue( n, 0f );
//						seeds++;
//					}
//				}
//				if (seeds == 0)
//					continue; // no passive margin on this plate

//				while (pq.TryDequeue( out Node? u, out float tu )) {
//					if (!time.TryGetValue( u, out float cur ) || tu > cur)
//						continue;

//					int iu = idxOf[ u ];
//					Vector3<float> uVec = u.Vertex.Vector;

//					foreach (Node v in u.NeighbouringNodes) {
//						if (!idxOf.TryGetValue( v, out int iv ))
//							continue;

//						// Only propagate inside continent
//						NodeTectonicLandscapeState vs = v.GetStateOrThrow<NodeTectonicLandscapeState>();
//						if (vs.CrustThicknessKm <= OceanicThicknessKm)
//							continue;

//						float arc = ArcRadians( uVec, v.Vertex.Vector );

//						// edge tangent at midpoint
//						Vector3<float> rMid = (uVec + v.Vertex.Vector).Normalize<Vector3<float>, float>();
//						Vector3<float> chord = v.Vertex.Vector - uVec;
//						Vector3<float> tHat = (chord - chord.Dot( rMid ) * rMid)
//								   .Normalize<Vector3<float>, float>();

//						// speed along edge = |(uA_avg · tHat)|
//						Vector3<float> flowEdge = 0.5f * (uA[ iu ] + uA[ iv ]);
//						float speed = MathF.Abs( flowEdge.Dot( tHat ) );
//						if (speed < RateEpsKmPerMa)
//							speed = RateEpsKmPerMa;

//						float dt = arc * radiusKm / speed; // Ma
//						float alt = tu + dt;

//						if (!time.TryGetValue( v, out float tv ) || alt < tv) {
//							time[ v ] = alt;
//							pq.Enqueue( v, alt );
//						}
//					}
//				}

//				// write PostRiftAgeMa
//				foreach (Node n in nodes) {
//					NodeTectonicLandscapeState s = n.GetStateOrThrow<NodeTectonicLandscapeState>();
//					s.PostRiftAgeMa = time.TryGetValue( n, out float t ) ? t : 0f;
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
//using Civlike.World.State;
//using Civlike.World.TectonicGeneration.Landscape.Plates;
//using Civlike.World.TectonicGeneration.Landscape.States;
//using Civlike.World.TectonicGeneration.NoiseProviders;
//using Engine;

//namespace Civlike.World.TectonicGeneration.Landscape.Old;

//[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<PlateEdgeInitializationStep>]
//public sealed class ContinentalPostRiftAgeStep( TectonicGenerationParameters p ) : TectonicGlobeGenerationProcessingStepBase( p ) {
//	const float OceanicThicknessKm = 12f;
//	const float RateEpsKmPerMa = 1e-3f;

//	public override void Process( Globe globe ) {
//		List<SphericalVoronoiRegion> regions = globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions;
//		float radiusKm = (float) globe.RadiusKm;

//		ParallelProcessing.Range( regions.Count, ( start, end, _ ) => {
//			for (int i = start; i < end; i++) {
//				SphericalVoronoiRegion region = regions[ i ];
//				SphericalVoronoiRegionTectonicPlateState plate = region.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();
//				List<Node> nodes = plate.Nodes;
//				if (nodes.Count == 0)
//					continue;

//				// local index
//				Dictionary<Node, int> idxOf = new Dictionary<Node, int>( nodes.Count );
//				for (int k = 0; k < nodes.Count; k++)
//					idxOf[ nodes[ k ] ] = k;

//				// plate flow at nodes: uA = R * (ωA × r̂)
//				Vector3<float>[] uA = new Vector3<float>[ nodes.Count ];
//				Vector3<float> omegaA = plate.AngularVelocity; // rad/Ma
//				for (int k = 0; k < nodes.Count; k++)
//					uA[ k ] = radiusKm * omegaA.Cross( nodes[ k ].Vertex.Vector ); // km/Ma

//				// multi-source Dijkstra in "time"
//				Dictionary<Node, float> time = new Dictionary<Node, float>( nodes.Count );
//				PriorityQueue<Node, float> pq = new PriorityQueue<Node, float>();

//				// seeds: continental nodes that were flagged as ridge seeds
//				int seeds = 0;
//				foreach (Node n in nodes) {
//					NodeTectonicLandscapeState s = n.GetStateOrThrow<NodeTectonicLandscapeState>();
//					bool isContinental = s.CrustThicknessKm > OceanicThicknessKm;
//					if (isContinental && s.IsRidgeSeed) {
//						time[ n ] = 0f;
//						pq.Enqueue( n, 0f );
//						seeds++;
//					}
//				}
//				if (seeds == 0)
//					continue; // no passive margin on this plate

//				while (pq.TryDequeue( out Node? u, out float tu )) {
//					if (!time.TryGetValue( u, out float cur ) || tu > cur)
//						continue;

//					int iu = idxOf[ u ];
//					Vector3<float> uVec = u.Vertex.Vector;

//					foreach (Node v in u.NeighbouringNodes) {
//						if (!idxOf.TryGetValue( v, out int iv ))
//							continue;

//						// Only propagate inside continent
//						NodeTectonicLandscapeState vs = v.GetStateOrThrow<NodeTectonicLandscapeState>();
//						if (vs.CrustThicknessKm <= OceanicThicknessKm)
//							continue;

//						float arc = ArcRadians( uVec, v.Vertex.Vector );

//						// edge tangent at midpoint
//						Vector3<float> rMid = (uVec + v.Vertex.Vector).Normalize<Vector3<float>, float>();
//						Vector3<float> chord = v.Vertex.Vector - uVec;
//						Vector3<float> tHat = (chord - chord.Dot( rMid ) * rMid)
//								   .Normalize<Vector3<float>, float>();

//						// speed along edge = |(uA_avg · tHat)|
//						Vector3<float> flowEdge = 0.5f * (uA[ iu ] + uA[ iv ]);
//						float speed = MathF.Abs( flowEdge.Dot( tHat ) );
//						if (speed < RateEpsKmPerMa)
//							speed = RateEpsKmPerMa;

//						float dt = arc * radiusKm / speed; // Ma
//						float alt = tu + dt;

//						if (!time.TryGetValue( v, out float tv ) || alt < tv) {
//							time[ v ] = alt;
//							pq.Enqueue( v, alt );
//						}
//					}
//				}

//				// write PostRiftAgeMa
//				foreach (Node n in nodes) {
//					NodeTectonicLandscapeState s = n.GetStateOrThrow<NodeTectonicLandscapeState>();
//					s.PostRiftAgeMa = time.TryGetValue( n, out float t ) ? t : 0f;
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