using Civlike.World.State;
using Civlike.World.TectonicGeneration.Landscape.Old;
using Civlike.World.TectonicGeneration.Landscape.States;

namespace Civlike.World.TectonicGeneration.Landscape.Heightmapping;

[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<CrustThicknessAndDensityStep>]
public sealed class HeightCompositionStep : TectonicGlobeGenerationProcessingStepBase {
	public HeightCompositionStep( TectonicGenerationParameters parameters ) : base( parameters ) { }

	// ----------- Tunables (km, g/cm^3, Ma) -----------
	const float RhoMantle = 3.30f;

	// Ocean/continent split (by crustal thickness)
	const float OceanicThicknessKm = 12f;

	// Thermal subsidence: h_th = -k * sqrt(age), capped
	const float ThermalK_KmPerSqrtMa = 0.33f;   // ~0.33 km * sqrt(Ma) ⇒ ~3.3 km at 100 Ma
	const float ThermalMinKm = -5.5f;   // clamp (deeper than this won’t grow)

	// Ridge bump near divergent boundaries
	const float RidgeAmpKm = 1.0f;
	const float RidgeSigmaKm = 60f;            // ~ half-width of ridge swell

	// Convergent features (choose per-plate based on crust thickness)
	// Oceanic side: trench only
	const float TrenchAmpKm = -3.5f;
	const float TrenchSigmaKm = 35f;

	// Continental side: orogen + foreland sag (inboard)
	const float OrogenAmpKm = 2.2f;
	const float OrogenCenterKm = 120f;
	const float OrogenSigmaKm = 80f;
	const float ForelandAmpKm = -0.8f;
	const float ForelandCenterKm = 220f;
	const float ForelandSigmaKm = 120f;

	// Scale your low-frequency base/dynamic field
	const float DynScale = 1.0f;

	// Minimal erosion: explicit Laplacian smoothing
	const int DiffuseIterations = 2;
	const float DiffuseLambda = 0.15f;      // 0..1; small = gentle

	// post-rift subsidence (continental)
	const float Apr_Km = 1.2f;   // amplitude
	const float TauPr_Ma = 60f;    // e-fold time
	const float Lpr_Km = 250f;   // inland taper width

	public override void Process( Globe globe ) {
		List<NoiseProviders.SphericalVoronoiRegion> regions = globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions;
		IReadOnlyList<Node> allNodes = globe.Nodes; // assuming you have this; otherwise collect from regions
		int N = allNodes.Count;
		float radiusKm = (float) globe.RadiusKm; // adjust if your Radius already in km

		// 1) Compose raw elevation per node (parallel per plate is safe; nodes are unique to a plate)
		//    We write into a temp buffer to keep erosion clean.
		float[] raw = new float[ N ];

		ParallelProcessing.Range( regions.Count, ( start, end, _ ) => {
			for (int i = start; i < end; i++) {
				NoiseProviders.SphericalVoronoiRegion region = regions[ i ];
				SphericalVoronoiRegionTectonicPlateState plate = region.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();
				foreach (Node n in plate.Nodes) {
					NodeTectonicLandscapeState s = n.GetStateOrThrow<NodeTectonicLandscapeState>();

					// Isostasy (km): ((ρm - ρc)/ρm) * T
					float hIso = (RhoMantle - s.CrustDensity) / RhoMantle * s.CrustThicknessKm;

					// Thermal (km): only oceanic nodes
					bool isOceanic = plate.IsOceanicPlate || s.CrustThicknessKm <= OceanicThicknessKm;
					float hTherm = 0f;
					if (isOceanic && s.AgeMa > 0f) {
						hTherm = -ThermalK_KmPerSqrtMa * MathF.Sqrt( s.AgeMa );
						if (hTherm < ThermalMinKm)
							hTherm = ThermalMinKm;
					} else {
						// passive-margin thermal subsidence
						float t = s.PostRiftAgeMa;
						if (t > 0f && float.IsFinite( s.DistanceToDivergentRadians )) {
							float dKm = s.DistanceToDivergentRadians * radiusKm;   // inland distance on this plate
							float taper = MathF.Exp( -0.5f * (dKm / Lpr_Km) * (dKm / Lpr_Km) );
							hTherm += -Apr_Km * (1f - MathF.Exp( -t / TauPr_Ma )) * taper;
						}
					}

					// Boundary features (km) from per-plate distance fields
					float hBoundary = 0f;

					// Ridge (divergent) bump
					if (float.IsFinite( s.DistanceToDivergentRadians )) {
						float dRkm = s.DistanceToDivergentRadians * radiusKm;
						hBoundary += RidgeAmpKm * Gaussian01( dRkm, RidgeSigmaKm );
					}

					// Convergent-side features
					if (float.IsFinite( s.DistanceToConvergentRadians )) {
						float dCkm = s.DistanceToConvergentRadians * radiusKm;

						if (isOceanic)                          // Oceanic plate side: trench only near the boundary
							hBoundary += TrenchAmpKm * Gaussian01( dCkm, TrenchSigmaKm );
						else {
							// Continental plate side: mountain belt + foreland sag inboard
							hBoundary += OrogenAmpKm * GaussianCentered01( dCkm, OrogenCenterKm, OrogenSigmaKm );
							hBoundary += ForelandAmpKm * GaussianCentered01( dCkm, ForelandCenterKm, ForelandSigmaKm );
						}
					}

					// Dynamic/base (your low-freq global field)
					float hDyn = DynScale * s.BaseElevation;

					// Stack
					float h = hIso + hTherm + hBoundary + hDyn;

					raw[ n.Vertex.Id ] = h;
				}
			}
		} );

		// 2) Minimal erosion (diffusive smoothing) across the whole mesh
		//    new = old + λ*(avg(neighbours) - old)
		float[] cur = raw;
		float[] nxt = new float[ N ];

		for (int it = 0; it < DiffuseIterations; it++) {
			ParallelProcessing.Range( N, ( start, end, _ ) => {
				for (int idx = start; idx < end; idx++) {
					Node node = allNodes[ idx ];
					float h0 = cur[ idx ];

					IReadOnlyList<Node> neigh = node.NeighbouringNodes;
					float sum = 0f;
					int cnt = 0;

					for (int k = 0; k < neigh.Count; k++) {
						sum += cur[ neigh[ k ].Vertex.Id ];
						cnt++;
					}

					if (cnt == 0) { nxt[ idx ] = h0; continue; }

					float avg = sum / cnt;
					nxt[ idx ] = h0 + DiffuseLambda * (avg - h0);
				}
			} );

			// swap buffers
			float[] tmp = cur;
			cur = nxt;
			nxt = tmp;
		}

		// 3) Write back
		foreach (Node n in allNodes)
			n.GetStateOrThrow<NodeTectonicLandscapeState>().FinalElevation = cur[ n.Vertex.Id ];
	}

	// ----- small helpers -----

	// Unit-peak Gaussian from distance to boundary (centered at 0)
	private static float Gaussian01( float dKm, float sigmaKm ) {
		float x = dKm / MathF.Max( 1e-3f, sigmaKm );
		return MathF.Exp( -0.5f * x * x );
	}

	// Unit-peak Gaussian centered at 'centerKm'
	private static float GaussianCentered01( float dKm, float centerKm, float sigmaKm ) {
		float x = (dKm - centerKm) / MathF.Max( 1e-3f, sigmaKm );
		return MathF.Exp( -0.5f * x * x );
	}
}