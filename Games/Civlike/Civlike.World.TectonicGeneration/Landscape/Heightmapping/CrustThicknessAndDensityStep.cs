using Civlike.World.State;
using Civlike.World.TectonicGeneration.Landscape.Ages;
using Civlike.World.TectonicGeneration.Landscape.Old;
using Civlike.World.TectonicGeneration.Landscape.States;

namespace Civlike.World.TectonicGeneration.Landscape.Heightmapping;

[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<PlateAgeTravelTimeStep>]
[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<ContinentalPostRiftAgeStep>]
public sealed class CrustThicknessAndDensityStep( TectonicGenerationParameters p ) : TectonicGlobeGenerationProcessingStepBase( p ) {

	// ---- Tunables ----
	// Oceanic
	const float OceanicT_MeanKm = 7.8f;
	const float OceanicT_JitterKm = 1.0f;   // small, keeps oceans ~7–10 km
	const float OceanicRho = 2.90f;

	// Continental core & transitional (passive margin)
	const float ContCoreT_Km = 42f;    // thick interior
	const float ContTransT_Km = 20f;    // thin transitional at passive margin
	const float ContTaperL_Km = 220f;   // 150–300 km typical taper scale
	const float ContCoreJitterKm = 2.5f;   // gentle wobble

	// Convergent thickening & foreland sag (apply to thickness)
	const float Orogen_d0_Km = 100f;   // center inboard
	const float Orogen_sigma_Km = 70f;
	const float Orogen_amp_Km = 12f;    // +10..20 km

	const float Foreland_d0_Km = 220f;   // farther inboard
	const float Foreland_sigma_Km = 120f;
	const float Foreland_amp_Km = -5f;    // −3..−8 km

	// Density mapping from thickness (thick felsic → light; thin/mafic → dense)
	const float RhoThick = 2.80f;  // at ≥ 50 km
	const float RhoThin = 2.90f;  // at ≤ 20 km
	const float TrefThickKm = 50f;
	const float TrefThinKm = 20f;
	const float OrogenExtraLightPerKm = 0.0015f; // extra lightening per km of orogenic thickening

	// Ocean/continent split (used by height later too)
	const float OceanicCutoffKm = 12f; // if you don’t have plate.IsOceanic, this is a safe node mask

	public override void Process( Globe globe ) {
		var regions = globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions;
		float radiusKm = (float) globe.RadiusKm;

		// Per-plate parallelism is safe: nodes belong to exactly one plate.
		ParallelProcessing.Range( regions.Count, ( start, end, _ ) => {
			for (int i = start; i < end; i++) {
				var region = regions[ i ];
				var plate = region.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();
				bool plateIsOceanic = plate.IsOceanicPlate;

				foreach (var n in plate.Nodes) {
					var s = n.GetStateOrThrow<NodeTectonicLandscapeState>();

					if (plateIsOceanic) {
						// --- OCEANIC: near-uniform thickness + tiny jitter
						float t = OceanicT_MeanKm + JitterSigned( n.Vertex.Id ) * OceanicT_JitterKm;
						s.CrustThicknessKm = ClampPositive( t );
						s.CrustDensity = OceanicRho;
						continue;
					}

					// --- CONTINENTAL: core thickness tapered toward passive margins (divergent edges)
					float dDiv_km = float.IsFinite( s.DistanceToDivergentRadians ) ? s.DistanceToDivergentRadians * radiusKm : 1e9f;
					float dConv_km = float.IsFinite( s.DistanceToConvergentRadians ) ? s.DistanceToConvergentRadians * radiusKm : 1e9f;

					// taper function: 0 at boundary (d=0) → 1 inboard (large d)
					float taper01 = 1f - Gaussian01( dDiv_km, ContTaperL_Km ); // smooth 0→1 with ~L scale
					float tCore = ContCoreT_Km + JitterSigned( n.Vertex.Id ) * ContCoreJitterKm;
					float tPassive = Lerp( ContTransT_Km, tCore, taper01 );      // at d=0 → Trans; far → Core

					// orogenic thickening (positive) and foreland sag (negative) as thickness modifiers
					float thickOrog = Orogen_amp_Km * GaussianCentered01( dConv_km, Orogen_d0_Km, Orogen_sigma_Km );
					float thickFore = Foreland_amp_Km * GaussianCentered01( dConv_km, Foreland_d0_Km, Foreland_sigma_Km );

					float T = tPassive + thickOrog + thickFore;
					T = MathF.Max( 5f, T ); // sanity floor

					// density from thickness (linear between refs), with extra lightening in orogens
					float rho = MapThicknessToDensity( T );
					rho += -OrogenExtraLightPerKm * MathF.Max( 0f, thickOrog ); // keep orogens lighter
					rho = float.Clamp( rho, 2.70f, 2.95f );

					s.CrustThicknessKm = T;
					s.CrustDensity = rho;
				}
			}
		} );
	}

	private static float MapThicknessToDensity( float Tkm ) {
		// Linear map: T >= TrefThick → RhoThick ; T <= TrefThin → RhoThin
		float t = (Tkm - TrefThickKm) / (TrefThinKm - TrefThickKm); // [Thick..Thin] → [0..1]
		t = float.Clamp( t, 0f, 1f );
		return Lerp( RhoThick, RhoThin, t );
	}

	private static float Gaussian01( float dKm, float sigmaKm ) {
		float s = MathF.Max( 1e-3f, sigmaKm );
		float x = dKm / s;
		return MathF.Exp( -0.5f * x * x );
	}

	private static float GaussianCentered01( float dKm, float centerKm, float sigmaKm ) {
		float s = MathF.Max( 1e-3f, sigmaKm );
		float x = (dKm - centerKm) / s;
		return MathF.Exp( -0.5f * x * x );
	}

	private static float Lerp( float a, float b, float t ) => a + (b - a) * float.Clamp( t, 0f, 1f );
	private static float ClampPositive( float x ) => x < 0f ? 0f : x;

	// Deterministic tiny jitter in [-1,1]
	private static float JitterSigned( int vertexId ) {
		unchecked {
			uint x = (uint) vertexId;
			x ^= 2747636419u;
			x *= 2654435769u;
			x ^= x >> 16;
			x *= 2246822519u;
			x ^= x >> 13;
			x *= 3266489917u;
			x ^= x >> 16;
			// 24-bit fraction → [0,1)
			float u01 = (x & 0x00FFFFFFu) * (1f / 16777216f);
			return u01 * 2f - 1f;
		}
	}
}
