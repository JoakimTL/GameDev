//using Civlike.World;
//using Engine;
//using OldGen.World.GenerationState;
//using System.Numerics;

//namespace OldGen.World.TectonicGeneration.Steps;

//[Engine.Processing.Do<IGenerationStep>.After<GenerateLandmassStep>]
//public sealed class DefineGradientStep : GlobeGenerationProcessingStepBase<TectonicGeneratingGlobe, TectonicGlobeParameters> {
//	public override string StepDisplayName => "Defining gradient";
//	public override void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters ) {
//		ParallelProcessing.Range( globe.TectonicFaces.Count, ( start, end, taskId ) => {
//			for (int i = start; i < end; i++) {
//				Face<TectonicFaceState> face = globe.TectonicFaces[ i ];
//				TectonicFaceState state = face.State;
//				Vector3 center = face.Center;
//				Vector3 gradient = Vector3.Zero;
//				foreach (Vertex vertex in face.Vertices) {
//					Vector3 direction = Vector3.Normalize(vertex.Vector.ToNumerics() - center);
//					gradient += direction * (vertex.Height - state.BaselineValues.ElevationMean);
//				}
//				gradient /= (float) globe.TileLength;
//				state.BaselineValues.Gradient = gradient;

//				Face<TectonicFaceState> downslopeNeighbour = null!;
//				float maxGradientDot = float.MinValue;
//				float slope = 0;
//				for (int j = 0; j < face.Neighbours.Count; j++) {
//					NeighbouringFace<TectonicFaceState> neighbour = face.Neighbours[ j ];
//					Face<TectonicFaceState> nbrFace = neighbour.Face;

//					if (nbrFace.State.BaselineValues.ElevationMean >= state.BaselineValues.ElevationMean)
//						continue;

//					float gradientNbrDirectionDot = -Vector3.Dot(gradient, neighbour.NormalizedDirection );

//					if (gradientNbrDirectionDot > maxGradientDot) {
//						maxGradientDot = gradientNbrDirectionDot;
//						downslopeNeighbour = nbrFace;
//						slope = -gradient.Length() / (float) globe.TileLength;
//					}
//				}

//				if (downslopeNeighbour is null)
//					continue;

//				state.DownslopeNeighbour = downslopeNeighbour;
//				state.DownslopeSlope = float.Max( slope, 1e-6f );
//				state.DownslopeSlopeSqrt = float.Sqrt( state.DownslopeSlope );
//			}
//		} );
//	}
//}
