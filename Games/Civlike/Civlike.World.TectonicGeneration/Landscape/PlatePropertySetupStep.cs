using Civlike.World.State;
using Civlike.World.TectonicGeneration.Landscape.States;
using Civlike.World.TectonicGeneration.NoiseProviders;
using Engine;
using Engine.Logging;

namespace Civlike.World.TectonicGeneration.Landscape;

[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<PlateEdgeDetectionStep>]
public sealed class PlatePropertySetupStep( TectonicGenerationParameters parameters ) : TectonicGlobeGenerationProcessingStepBase( parameters ) {
	public override void Process( Globe globe ) {
		List<SphericalVoronoiRegion> regions = globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions;

		const float lowVelocity = 20;
		const float highVelocity = 160;

		Random rng = new( Parameters.SeedProvider.Next() );

		float totalCountedArea = 0;
		float areaPerNode = (float) globe.Area / globe.Nodes.Count;
		float accumulatedLandAreaPercentage = 0;

		foreach (SphericalVoronoiRegion region in regions) {
			SphericalVoronoiRegionTectonicPlateState state = region.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();

			state.PlateColor = (rng.NextSingle(), rng.NextSingle(), rng.NextSingle(), 1);

			float yawMovement = (rng.NextSingle() * 2 - 1) * float.Pi;
			float pitchMovement = (rng.NextSingle() * 2 - 1) * float.Pi;
			Vector2<float> movementSpherical = (yawMovement, pitchMovement);
			float speed = rng.NextSingle() * (highVelocity - lowVelocity) + lowVelocity;
			state.AngularVelocity = movementSpherical.ToCartesianFromPolar( speed / (float) (globe.Radius / 1000) );

			int nodes = state.Nodes.Count;
			totalCountedArea += areaPerNode * nodes;
			float plateAreaPercentage = areaPerNode * nodes / (float) globe.Area;

			bool shouldBeOceanic = accumulatedLandAreaPercentage > parameters.LandscapeParameters.ApproximateLandPercentage;
			state.IsOceanicPlate = shouldBeOceanic;
			if (!shouldBeOceanic)
				accumulatedLandAreaPercentage += plateAreaPercentage;
		}

		this.LogLine( $"Area: {totalCountedArea} / {globe.Area}" );
		this.LogLine( $"Land percentage: {accumulatedLandAreaPercentage}" );
	}

}
