//using OldGen.World.GenerationState;
//using OldGen.World.TectonicGeneration.Parameters;
//using System.Numerics;

//namespace OldGen.World.TectonicGeneration.Steps;

//[Engine.Processing.Do<IGenerationStep>.After<GenerateLandmassStep>]
//[Engine.Processing.Do<IGenerationStep>.Before<SimulationStep>]
//public sealed class SetHadleyCells : GlobeGenerationProcessingStepBase<TectonicGeneratingGlobe, TectonicGlobeParameters> {
//	public override string StepDisplayName => "Creating Hadley circulation";
//	public override void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters ) {
//		var adp = globe.AtmosphericDynamicsParameters;
//		Vector3 upAxis = adp.UpAxis;

//		ParallelProcessing.Range( globe.TectonicFaces.Count, ( start, end, taskId ) => {
//			for (int i = start; i < end; i++) {
//				Face<TectonicFaceState> face = globe.TectonicFaces[ i ];
//				TectonicFaceState state = face.State;
//				state.HadleyWinds = GetHadleyComponent( face.CenterNormalized, upAxis, face, adp );
//			}
//		} );
//	}

//	private Vector3 GetHadleyComponent( Vector3 normal, Vector3 upAxis, Face<TectonicFaceState> face, AtmosphericDynamicsParameters adp ) {
//		float φ0 = adp.HadleyCellLatitudeWidth; // in radians
//		float Ah = adp.HadleyHorizontalStrength; // m/s
//		float Av = adp.HadleyVerticalStrength; // m/s
//		float lat = face.LatitudeRads; // in radians
//		float latAbs = face.LatitudeRadsAbs;
//		float sin = MathF.Sin( latAbs / φ0 * float.Pi );
//		float cos = MathF.Cos( lat / φ0 * float.Pi );

//		//var weakeningFactor = MathF.Exp( -(lat * lat) / (φ0 * φ0) );
//		Vector3 sideways = Vector3.Cross( upAxis, normal );

//		Vector3 hadleyWind = Av * normal * cos - Ah * sideways * sin;
//		return hadleyWind;
//	}
//}
