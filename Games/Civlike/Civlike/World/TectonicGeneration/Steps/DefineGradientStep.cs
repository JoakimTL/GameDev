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

				Face<TectonicFaceState> downslopeNeighbour = null!;
				float maxGradientDot = float.MinValue;
				float slope = 0;
				for (int j = 0; j < face.Neighbours.Count; j++) {
					NeighbouringFace neighbour = face.Neighbours[ j ];
					Face<TectonicFaceState> nbrFace = neighbour.Face as Face<TectonicFaceState> ?? throw new InvalidCastException( $"Neighbouring face at index {j} is not of type TectonicFaceState." );

					if (nbrFace.State.BaselineValues.ElevationMean >= state.BaselineValues.ElevationMean)
						continue;

					float gradientNbrDirectionDot = -gradient.Dot( neighbour.NormalizedDirection );

					if (gradientNbrDirectionDot > maxGradientDot) {
						maxGradientDot = gradientNbrDirectionDot;
						downslopeNeighbour = nbrFace;
						slope = -gradient.Magnitude<Vector3<float>, float>() / (float) globe.ApproximateTileLength;
					}
				}

				if (downslopeNeighbour is null)
					continue;

				state.DownslopeNeighbour = downslopeNeighbour;
				state.DownslopeSlope = float.Max( slope, 1e-6f );
				state.DownslopeSlopeSqrt = float.Sqrt( state.DownslopeSlope );
			}
		} );
	}
}
