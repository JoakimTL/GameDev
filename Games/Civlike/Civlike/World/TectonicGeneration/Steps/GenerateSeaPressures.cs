using Civlike.World.GenerationState;
using Civlike.World.TectonicGeneration.Parameters;
using Engine;

namespace Civlike.World.TectonicGeneration.Steps;

[Engine.Processing.Do<IGenerationStep>.After<DefineStartingValues>]
[Engine.Processing.Do<IGenerationStep>.After<CreateOceansStep>]
[Engine.Processing.Do<IGenerationStep>.Before<SimulationStep>]
public sealed class SetDistancesFromLand : GlobeGenerationProcessingStepBase<TectonicGeneratingGlobe, TectonicGlobeParameters> {
	public override string StepDisplayName => "Generating sea pressures";
	public override void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters ) {
		Queue<Face<TectonicFaceState>> nextOceanFace = [];
		HashSet<Face<TectonicFaceState>> processedOceanFaces = [];
		for (int i = 0; i < globe.TectonicFaces.Count; i++) {
			Face<TectonicFaceState> face = globe.TectonicFaces[ i ];
			TectonicFaceState state = face.State;
			if (face.IsOcean)
				continue;
			state.DistanceFromLand = 0;
			state.NearestLandFace = face;
			for (int j = 0; j < face.Neighbours.Count; j++) {
				NeighbouringFace<TectonicFaceState> neighbour = face.Neighbours[ j ];
				Face<TectonicFaceState> nbrFace = neighbour.Face;

				if (!nbrFace.IsOcean)
					continue;
				float distance = globe.GetGreatCircleDistance( face.CenterNormalized, nbrFace.CenterNormalized );
				if (distance < nbrFace.State.DistanceFromLand || nbrFace.State.DistanceFromLand == 0) {
					nbrFace.State.DistanceFromLand = distance;
					nbrFace.State.NearestLandFace = face;
				}
				if (processedOceanFaces.Add( nbrFace ))
					nextOceanFace.Enqueue( nbrFace );
			}
		}

		while (nextOceanFace.TryDequeue( out Face<TectonicFaceState>? face )) {
			TectonicFaceState state = face.State;

			if (state.NearestLandFace is null)
				throw new InvalidOperationException( $"Face {face.Id} has no nearest land face set." );

			for (int j = 0; j < face.Neighbours.Count; j++) {
				NeighbouringFace<TectonicFaceState> neighbour = face.Neighbours[ j ];
				Face<TectonicFaceState> nbrFace = neighbour.Face;

				if (!nbrFace.IsOcean)
					continue;
				float distance = globe.GetGreatCircleDistance( state.NearestLandFace.CenterNormalized, nbrFace.CenterNormalized );
				if (distance < nbrFace.State.DistanceFromLand || nbrFace.State.DistanceFromLand == 0) {
					nbrFace.State.DistanceFromLand = distance;
					nbrFace.State.NearestLandFace = state.NearestLandFace;
				}
				if (processedOceanFaces.Add( nbrFace ))
					nextOceanFace.Enqueue( nbrFace );
			}
		}
		float spb = (float) globe.AtmosphericDynamicsParameters.SeaPressureBuildup;
		float spdd = (float) globe.AtmosphericDynamicsParameters.SeaPressureDecayDistance;

		ParallelProcessing.Range( globe.TectonicFaces.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face<TectonicFaceState> face = globe.TectonicFaces[ i ];
				TectonicFaceState state = face.State;

				float factor = (1 - float.Exp( -state.DistanceFromLand * spdd ));

				state.SeaPressure = spb * factor * (1 - float.Abs( face.CenterNormalized.Y ));
			}
		} );
	}
}

[Engine.Processing.Do<IGenerationStep>.After<DefineStartingValues>]
[Engine.Processing.Do<IGenerationStep>.Before<SimulationStep>]
public sealed class SetHadleyCells : GlobeGenerationProcessingStepBase<TectonicGeneratingGlobe, TectonicGlobeParameters> {
	public override string StepDisplayName => "Creating Hadley circulation";
	public override void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters ) {
		ParallelProcessing.Range( globe.TectonicFaces.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face<TectonicFaceState> face = globe.TectonicFaces[ i ];
				TectonicFaceState state = face.State;
				state.HadleyWinds = GetHadleyComponent(face.CenterNormalized, Vector3<float>.UnitY, face, globe.AtmosphericDynamicsParameters);
			}
		} );
	}
	private Vector3<float> GetHadleyComponent( Vector3<float> normal, Vector3<float> upAxis, Face<TectonicFaceState> face, AtmosphericDynamicsParameters adp ) {
		float φ0 = adp.HadleyCellLatitudeWidth; // in radians
		float Ah = adp.HadleyHorizontalStrength; // m/s
		float Av = adp.HadleyVerticalStrength; // m/s
		float lat = face.LatitudeRads; // in radians
		float latAbs = face.LatitudeRadsAbs;
		float sin = float.Sin( latAbs / φ0 * float.Pi );
		float cos = float.Cos( lat / φ0 * float.Pi );

		//var weakeningFactor = MathF.Exp( -(lat * lat) / (φ0 * φ0) );
		Vector3<float> sideways = upAxis.Cross( normal );

		Vector3<float> hadleyWind = Av * normal * cos - Ah * sideways * sin;
		return hadleyWind;
	}
}
