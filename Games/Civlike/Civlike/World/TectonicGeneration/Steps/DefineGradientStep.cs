using Civlike.World.GenerationState;
using Engine;

namespace Civlike.World.TectonicGeneration.Steps;

[Engine.Processing.Do<IGenerationStep>.After<GenerateLandmassStep>]
public sealed class DefineGradientStep : GlobeGenerationProcessingStepBase<TectonicGeneratingGlobe, TectonicGlobeParameters> {
	public override string StepDisplayName => "Defining gradient";
	public override void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters ) {
		ParallelProcessing.Range( globe.Faces.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face<TectonicFaceState> face = globe.Faces[ i ] as Face<TectonicFaceState> ?? throw new InvalidCastException( $"Face at index {i} is not of type TectonicFaceState." );
				TectonicFaceState state = face.State;
				Vector3<float> center = face.Center;
				Vector3<float> gradient = 0;
				foreach (Vertex vertex in face.Vertices) {
					Vector3<float> direction = (vertex.Vector - center).Normalize<Vector3<float>, float>();
					gradient += direction * (vertex.Height - state.BaselineValues.ElevationMean);
				}
				gradient /= (float) globe.ApproximateTileLength;
				state.BaselineValues.Gradient = gradient;
			}
		} );
	}
}
