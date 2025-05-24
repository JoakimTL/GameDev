//using Civlike.World.GenerationState;
//using Civlike.World.NoiseProviders;
//using Engine;

//namespace Civlike.World.TectonicGeneration.Steps;

//[Engine.Processing.Do<IGenerationStep>.After<GenerateLandmassStep>]
//public sealed class DefineFaceSoilDepthValues : GlobeGenerationProcessingStepBase<TectonicGeneratingGlobe, TectonicGlobeParameters> {
//	public override string StepDisplayName => "Defining tile soil depth";

//	public override void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters ) {
//		Noise3 soilCoarseNoise = new( globe.SeedProvider.Next(), 18 );
//		Noise3 soilFineNoise = new( globe.SeedProvider.Next(), 79 );
//		Noise3 porosityCoarseNoise = new( globe.SeedProvider.Next(), 12 );
//		Noise3 porosityFineNoise = new( globe.SeedProvider.Next(), 88 );

//		ParallelProcessing.Range( globe.Faces.Count, ( start, end, taskId ) => {
//			for (int i = start; i < end; i++) {
//				Face face = globe.Faces[ i ];
//				Vector3<float> center = face.GetCenter();
//				TectonicFaceState state = face.Get<TectonicFaceState>();

//				double S = state.BaselineValues.Gradient.Magnitude<Vector3<float>, float>();
//				double baseDepth = globe.SoilDepthGenerationConstants.MaxSoilDepth * Math.Exp( -globe.SoilDepthGenerationConstants.SlopeDecayConstant * S );

//				double latitude = double.RadiansToDegrees( Math.Asin( center.Y ) );
//				double latNorm = Math.Abs( latitude ) / 90.0;
//				double climateFactor = 1.0 - 0.5 * latNorm;

//				double soilNoise = soilCoarseNoise.Noise( center ) * 0.65 + soilFineNoise.Noise( center ) * 0.35;
//				soilNoise = soilNoise * 2 - 1;
//				baseDepth *= 1 + globe.SoilDepthGenerationConstants.SoilNoiseAmplitude * soilNoise;

//				state.SoilDepth = (float) (baseDepth * climateFactor);

//				double porosityNoise = porosityCoarseNoise.Noise( center ) * 0.65 + porosityFineNoise.Noise( center ) * 0.35;
//				porosityNoise = porosityNoise * 2 - 1;
//				float porosity = (float) (globe.SoilDepthGenerationConstants.SoilPorosityBase * (1 + globe.SoilDepthGenerationConstants.SoilPorosityNoiseAmplitude * porosityNoise));
//				state.SoilMoistureCapacity = state.SoilDepth * porosity * 1000.0f;
//			}
//		} );
//	}
//}
