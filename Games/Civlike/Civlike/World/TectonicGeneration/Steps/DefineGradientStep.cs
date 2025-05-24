using Civlike.World.GenerationState;
using Engine;

namespace Civlike.World.TectonicGeneration.Steps;

[Engine.Processing.Do<IGenerationStep>.After<GenerateLandmassStep>]
public sealed class DefineGradientStep : GlobeGenerationProcessingStepBase<TectonicGeneratingGlobe, TectonicGlobeParameters> {
	public override string StepDisplayName => "Defining gradient";
	public override void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters ) {
		ParallelProcessing.Range( globe.Faces.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face face = globe.Faces[ i ];
				Vector3<float> center = face.Center;
				TectonicFaceState state = face.Get<TectonicFaceState>();
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
