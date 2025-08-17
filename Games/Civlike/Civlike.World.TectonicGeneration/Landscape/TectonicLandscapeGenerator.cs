using Civlike.World.State;
using Civlike.World.State.States;
using Civlike.World.TectonicGeneration.Landscape.States;
using Civlike.World.TectonicGeneration.NoiseProviders;
using Engine;
using System.Drawing;

namespace Civlike.World.TectonicGeneration.Landscape;

[Engine.Processing.Do<IGlobeGenerationProcessingStep>.Last]
public sealed class TectonicLandscapeGenerationStep( TectonicGenerationParameters parameters ) : TectonicGlobeGenerationProcessingStepBase( parameters ) {
	public override void Process( Globe globe ) {
		List<SphericalVoronoiRegion> sphericalRegions = globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions;

		//// constants you already have somewhere:
		//const float contMin = 25f, contMax = 50;  // typical continental range
		//const float oceanMin = 7f, oceanMax = 10f;  // typical oceanic range
		//const float noiseAmp = 2.0f;                 // gentle wobble
		//const float Ttrans = 20f;                  // “transitional” target near passive margins

		//Noise3 plateThicknessNoise = new( Parameters.SeedProvider.Next(), 5.6f );
		//Noise3 plateWobbleNoise = new( Parameters.SeedProvider.Next(), 4.8f );

		//FiniteSphericalVoronoiNoise3 ridgeNoise = new( new( Parameters.SeedProvider.Next() ), 48, Parameters.LandscapeParameters.MinimumPlateMidpointDistance );
		//Noise3 xShiftNoise = new( Parameters.SeedProvider.Next(), 11 );
		//Noise3 yShiftNoise = new( Parameters.SeedProvider.Next(), 11 );
		//Noise3 zShiftNoise = new( Parameters.SeedProvider.Next(), 11 );

		//foreach (SphericalVoronoiRegion plate in sphericalRegions) {
		//	Random rng2 = new( Parameters.SeedProvider.Next() );
		//	SphericalVoronoiRegionTectonicPlateState pState = plate.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();

		//	// NEW: normalize by the deepest interior distance of this plate (radians)
		//	float maxEdgeDistRad = 0f;
		//	foreach (Node n in pState.Nodes)
		//		maxEdgeDistRad = MathF.Max( maxEdgeDistRad, n.GetStateOrThrow<NodeTectonicLandscapeState>().DistanceToPlateEdgeRadians );
		//	if (maxEdgeDistRad <= 1e-6f)
		//		maxEdgeDistRad = 1e-6f;

		//	// helper: smoothstep 0..1
		//	static float Smooth01( float u ) {
		//		u = float.Clamp( u, 0f, 1f );
		//		return u * u * (3f - 2f * u);
		//	}

		//	// Width of the passive-margin taper (in *radians* to match your distance field)
		//	// (e.g., ~220 km on Earth -> km / R)
		//	float wPassiveRad = 220f / (float) globe.RadiusKm;

		//	// Assign thickness & density from edge distance
		//	foreach (Node node in pState.Nodes) {
		//		NodeTectonicLandscapeState? nState = node.GetStateOrThrow<NodeTectonicLandscapeState>();
		//		Vector3<float> v = node.Vertex.Vector;

		//		float thick = 0;
		//		float dens = 0;
		//		float thicknessNoiseValue = plateThicknessNoise.Noise( v );   // assume 0..1

		//		if (!nState.Region.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>().IsOceanicPlate) {
		//			// edgeNorm: 0 at the plate edge … 1 deep in the interior
		//			float edgeNorm = nState.DistanceToPlateEdgeRadians / maxEdgeDistRad;
		//			edgeNorm = float.Clamp( edgeNorm, 0f, 1f );
		//			float t = 1f - edgeNorm;
		//			// Core target thickness (continental) modulated by low-freq noise
		//			float Tcore = thicknessNoiseValue * (contMax - contMin) + contMin;

		//			// Smooth taper from transitional near the edge to continental inland
		//			// Using smoothstep on distance within a passive-margin width
		//			float sPassive = Smooth01( nState.DistanceToPlateEdgeRadians / wPassiveRad );
		//			float thickCont = float.Lerp( Ttrans, Tcore, sPassive );

		//			// Preserve your original 4-band behavior by letting *very* edge-proximal
		//			// nodes drop toward oceanic values, otherwise use the tapered continental
		//			thick =
		//				t < 0.60f ? thickCont :
		//				t < 0.80f ? float.Lerp( 20f, 10f, (t - .60f) / .20f ) :
		//							(thicknessNoiseValue * (oceanMax - oceanMin) + oceanMin);
		//			dens =
		//				t < 0.30f ? 2.80f :
		//				t < 0.60f ? float.Lerp( 2.80f, 2.85f, (t - .30f) / .30f ) :
		//				t < 0.80f ? float.Lerp( 2.85f, 2.90f, (t - .60f) / .20f ) :
		//							2.90f;
		//		} else {
		//			thick = (thicknessNoiseValue * (oceanMax - oceanMin) + oceanMin);
		//			dens = 2.90f;
		//		}

		//		// Gentle coastline wobble so contours aren't perfect rings
		//		thick += (plateWobbleNoise.Noise( v ) * 2f - 1f) * noiseAmp;

		//		Vector3<float> shift = new Vector3<float>( xShiftNoise.Noise( v ), yShiftNoise.Noise( v ), zShiftNoise.Noise( v ) ) * 2 - 1;
		//		Vector3<float> shifted = v + shift * 0.2f;
		//		shifted = shifted.Normalize<Vector3<float>, float>();
		//		float tt = ridgeNoise.Noise( shifted ) * 3;
		//		thick += tt;

		//		nState.CrustThicknessKm = thick;
		//		nState.CrustDensity = dens;
		//	}

		//	// Per-plate summaries unchanged
		//	pState.MeanCrustThicknessKm = pState.Nodes.Average( n => n.GetStateOrThrow<NodeTectonicLandscapeState>().CrustThicknessKm );
		//	pState.MeanCrustDensity = pState.Nodes.Average( n => n.GetStateOrThrow<NodeTectonicLandscapeState>().CrustDensity );
		//}

		ParallelProcessing.Range( globe.Nodes.Count, ( start, end, _ ) => {
			for (int i = start; i < end; i++) {
				Node node = globe.Nodes[ i ];
				NodeTectonicLandscapeState? nState = node.GetStateOrThrow<NodeTectonicLandscapeState>();

				// simple Pratt–Airy:  h = (ρ_mantle − ρ_crust)/ρ_mantle × thickness
				const float ρMantle = 3.30f;           // g cm⁻³, constant
				float Δρ = ρMantle - nState.CrustDensity;
				nState.BaseElevation = Δρ / ρMantle * nState.CrustThicknessKm * 1000f;  // → metres
			}
		} );

		const float mantleNoiseFreq = 0.30f;             // 1 / π ≈ 2 000 km on unit sphere
		const float mantleAmpMetres = 1500f;             // ±1.5 km bulge range
		Noise3 dynamicTopoNoise = new( Parameters.SeedProvider.Next(), mantleNoiseFreq );

		// 3-B.  Parallel pass – sample noise and add to each node
		ParallelProcessing.Range( globe.Nodes.Count, ( start, end, _ ) => {
			for (int idx = start; idx < end; idx++) {
				Node node = globe.Nodes[ idx ];
				NodeTectonicLandscapeState? nState = node.GetStateOrThrow<NodeTectonicLandscapeState>();

				// sample simplex / Perlin noise in unit-sphere coordinates
				float swell = dynamicTopoNoise.Noise( node.Vertex.Vector ) * mantleAmpMetres;

				// add to the previously stored isostatic height
				nState.BaseElevation += swell;
			}
		} );

		float lowestBaseElevation = globe.Nodes.Min( n => n.GetStateOrThrow<NodeTectonicLandscapeState>().FinalElevation );
		float highestBaseElevation = globe.Nodes.Max( n => n.GetStateOrThrow<NodeTectonicLandscapeState>().FinalElevation );
		float elevationRange = highestBaseElevation - lowestBaseElevation;

		ParallelProcessing.Range( globe.Tiles.Count, ( start, end, taskId ) => {
			HashSet<SphericalVoronoiRegion> regions = new();
			for (int i = start; i < end; i++) {
				Tile t = globe.Tiles[ i ];

				Vector4<float> color = Vector4<float>.Zero;
				float elev = 0f;
				bool isRidge = false;
				bool isConvergent = false;
				bool isLand = false;
				regions.Clear();
				float age = 0;
				float prAge = 0;
				float crustThickness = 0;
				foreach (Node n in t.Nodes) {
					color += n.GetStateOrThrow<NodeTectonicLandscapeState>()
							  .Region.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>()
							  .PlateColor;
					elev += n.GetStateOrThrow<NodeTectonicLandscapeState>().FinalElevation;
					isRidge |= n.GetStateOrThrow<NodeTectonicLandscapeState>().IsRidgeSeed;
					isConvergent |= n.GetStateOrThrow<NodeTectonicLandscapeState>().IsConvergentSeed;
					age += n.GetStateOrThrow<NodeTectonicLandscapeState>().AgeMa;
					prAge += n.GetStateOrThrow<NodeTectonicLandscapeState>().PostRiftAgeMa;
					regions.Add( n.GetStateOrThrow<NodeTectonicLandscapeState>().Region );
					crustThickness += n.GetStateOrThrow<NodeTectonicLandscapeState>().CrustThicknessKm;
				}
				isLand = !regions.All( r => r.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>().IsOceanicPlate );
				color /= 3;
				elev /= 3;
				age /= 3;
				prAge /= 3;
				crustThickness /= 3;
				t.GetStateOrThrow<TileColorState>().Color = color * float.Sqrt( (elev - lowestBaseElevation) / elevationRange );
				t.GetStateOrThrow<TileColorState>().Color = (age / 200, elev < 0 ? 0 : elev / highestBaseElevation, elev >= 0 ? 0 : elev / lowestBaseElevation, 1);
				t.GetStateOrThrow<TileColorState>().Color = (age / 200, isLand ? 1 : 0, elev >= 0 ? 0 : elev / lowestBaseElevation, 1);
				t.GetStateOrThrow<TileColorState>().Color = (age / 200, prAge / 200, elev >= 0 ? 0 : elev / lowestBaseElevation, 1);
				t.GetStateOrThrow<TileColorState>().Color = (crustThickness / 30, prAge / 200, elev >= 0 ? 0 : elev / lowestBaseElevation, 1);
				//if (edgeCount == 3)
				//	t.GetStateOrThrow<TileColorState>().Color = color;

				////if (isRidge || isConvergent)
				////	t.GetStateOrThrow<TileColorState>().Color = (isRidge ? 1 : 0, isConvergent ? 1 : 0, 0, 1f);
				//if (regions.Count > 1) {
				//	t.GetStateOrThrow<TileColorState>().Color = (.3f, .3f, .3f, 1f);
				//} else {
				t.GetStateOrThrow<TileColorState>().Color = (isRidge ? 1 : 0, isConvergent ? 1 : 0, age / 200, 1f);
				//}

				// (Optionally: store average elevation per tile as well)
				//t.GetState<TileHeightState>().BaseHeight = elev / t.Nodes.Count;

			}
		} );
	}
}
