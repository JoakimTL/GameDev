using Civlike.World.State;
using Civlike.World.TectonicGeneration.Landscape.States;
using Civlike.World.TectonicGeneration.NoiseProviders;
using Engine;
using Engine.Logging;

namespace Civlike.World.TectonicGeneration.Landscape.Plates;

[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<PlateMergingStep>]
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

			var platePosition = region.Position.Normalize<Vector3<float>, float>();
			var rotationPole = (rng.NextSingle() * float.Pi * 2).ToUnitVector2Radians();
			var omegaPlane = rotationPole.RotateToPlane( platePosition );

			float speed = rng.NextSingle() * (highVelocity - lowVelocity) + lowVelocity;
			state.AngularVelocity = omegaPlane * speed / (float) globe.RadiusKm;

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
