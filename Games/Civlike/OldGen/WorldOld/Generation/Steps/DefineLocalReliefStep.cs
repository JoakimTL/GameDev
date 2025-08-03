//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.


//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.


//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.


//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace OldGen.WorldOld.Generation.Steps;

public sealed class DefineLocalReliefStep : ITerrainGenerationProcessingStep {
	public string ProcessingStepMessage => "Defining local relief";

	public void Process( WorldGenerationParameters parameters, GlobeModel globe ) {
		for (uint i = 0; i < globe.FaceCount; i++) {
			Face face = globe.Faces[ (int) i ];

			float minHeight = face.State.Height;
			float maxHeight = face.State.Height;
			float minPressureHeight = face.State.PressureHeight;
			float maxPressureHeight = face.State.PressureHeight;

			foreach (Face neighbourFace in face.Blueprint.Neighbours) {
				float height = neighbourFace.State.Height;
				float pressureHeight = neighbourFace.State.PressureHeight;
				if (height < minHeight)
					minHeight = height;
				if (height > maxHeight)
					maxHeight = height;
				if (pressureHeight < minPressureHeight)
					minPressureHeight = pressureHeight;
				if (pressureHeight > maxPressureHeight)
					maxPressureHeight = pressureHeight;
			}

			face.State.SetLocalRelief( maxHeight - minHeight );
			face.State.SetLocalPressureRelief( maxPressureHeight - minPressureHeight );
		}
	}
}
