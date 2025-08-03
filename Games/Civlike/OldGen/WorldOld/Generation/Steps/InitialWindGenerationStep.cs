using Engine;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace OldGen.WorldOld.Generation.Steps;

public sealed class InitialWindGenerationStep : ITerrainGenerationProcessingStep {
	public string ProcessingStepMessage => "Generating initial winds";

	public void Process( WorldGenerationParameters parameters, GlobeModel globe ) {
		float tileSpacing = (float) globe.ApproximateTileLength;    // meters, adjust if you compute it exactly
		float avgWindSpeed = 8f;                                    // m/s, tune to taste
		float dt = tileSpacing / avgWindSpeed;                      // seconds per tile-hop

		float tradeSpeed = 5;
		float westerlySpeed = 10;
		float polarSpeed = 5;

		for (uint i = 0; i < globe.FaceCount; i++) {
			Face face = globe.Faces[ (int) i ];
			Vector3<float> center = (face.Blueprint.VectorA + face.Blueprint.VectorB + face.Blueprint.VectorC) / 3;

			float latitude = MathF.Asin( center.Y );
			float longitude = MathF.Atan2( center.Z, center.X );

			Rotor3<float> rotor = Rotor3<float>.MultiplicativeIdentity;
			rotor = Rotor3.FromAxisAngle( Vector3<float>.UnitY, float.Pi * 3 / 2 - longitude ) * rotor;
			rotor = Rotor3.FromAxisAngle( rotor.Left, float.Pi * 3 / 2 - latitude ) * rotor;
			float windSpeedMs = 0;

			if (latitude < 0)
				rotor = Rotor3.FromAxisAngle( center, float.Pi ) * rotor;
			rotor = rotor.Normalize<Rotor3<float>, float>();
			if (float.Abs( latitude ) > float.Pi / 2 - float.Pi / 6) {
				rotor = Rotor3.FromAxisAngle( center, float.Pi * 2 / 3 * float.Sign( latitude ) ) * rotor;
				windSpeedMs = polarSpeed;
			} else if (float.Abs( latitude ) > float.Pi / 2 - float.Pi / 6 * 2) {
				rotor = Rotor3.FromAxisAngle( center, -float.Pi / 3 * float.Sign( latitude ) ) * rotor;
				windSpeedMs = westerlySpeed;
			} else {
				rotor = Rotor3.FromAxisAngle( center, float.Pi * 2 / 3 * float.CopySign( 1, latitude ) ) * rotor;
				windSpeedMs = tradeSpeed;
			}

			Vector3<float> direction = rotor.Forward;

			float dtheta = dt * 2 * (float) parameters.RevolutionsPerSecond * -float.Abs( center.Y );

			// fast sin/cos:
			float cosTh = float.Cos( dtheta );
			float sinTh = float.Sin( dtheta );

			Vector3<float> vRot = direction * cosTh + Vector3<float>.UnitY.Cross( direction ) * sinTh;

			direction = (vRot - vRot.Dot( center ) * center).Normalize<Vector3<float>, float>();

			face.State.SetWindDirection( direction );
			face.State.SetWindSpeed( windSpeedMs );
			//vectorToProject - vectorToProject.Dot( axisVector ) * axisVector
		}
	}
}
