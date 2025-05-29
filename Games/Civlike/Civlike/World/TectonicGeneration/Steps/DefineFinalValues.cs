using Civlike.World.GenerationState;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;

namespace Civlike.World.TectonicGeneration.Steps;

[Engine.Processing.Do<IGenerationStep>.After<SimulationStep>]
public sealed class DefineFinalValues : GlobeGenerationProcessingStepBase<TectonicGeneratingGlobe, TectonicGlobeParameters> {
	public override string StepDisplayName => "Defining final values per tile";

	public override void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters ) {
		var adp = globe.AtmosphericDynamicsParameters;

		float daysSimulated = (float) (parameters.GenerationParameters.SpinUpDurationSeconds / parameters.GenerationParameters.SimulationTimeStepSeconds);

		ParallelProcessing.Range( globe.TectonicFaces.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face<TectonicFaceState> face = globe.TectonicFaces[ i ];
				TectonicFaceState state = face.State;

				state.AverageAirTemperature /= daysSimulated;
				state.AverageSurfaceTemperature /= daysSimulated;
				state.TangentialWind = state.Wind - Vector3.Dot( state.Wind, face.CenterNormalized ) * face.CenterNormalized;
			}
		} );
	}
}
