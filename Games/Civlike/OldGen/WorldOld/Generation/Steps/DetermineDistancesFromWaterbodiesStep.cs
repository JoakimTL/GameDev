using Engine;
using Engine.Logging;

//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

namespace OldGen.WorldOld.Generation.Steps;

public sealed class DetermineDistancesFromWaterbodiesStep : ITerrainGenerationProcessingStep {
	public string ProcessingStepMessage => "Determining distances from water bodies";

	public void Process( WorldGenerationParameters parameters, GlobeModel globe ) {
		SetLinearDistancesFromOcean( globe );
		SetUpwindDistancesFromOcean( globe );
	}

	private void SetLinearDistancesFromOcean( GlobeModel globe ) {
		Queue<Face> linearQueue = [];
		Queue<Face> unsetIdQueue = [];
		Dictionary<uint, int> linearAttempts = [];
		HashSet<uint> enqueuedLinear = [];

		for (uint i = 0; i < globe.FaceCount; i++) {
			Face face = globe.Faces[ (int) i ];
			if (face.State.Height >= 0)
				if (face.Blueprint.Connections.Any( p => p.GetOther( face ).State.Height < 0 ))
					if (enqueuedLinear.Add( face.Id ))
						linearQueue.Enqueue( face );
					else
						face.State.SetLinearDistanceFromOcean( 0 );
		}

		int linearAttemptLimit = 16; // Lowered and now enforced

		Face[] neighbours = new Face[ 3 ];
		while (linearQueue.TryDequeue( out Face? face )) {
			int tries = linearAttempts.GetValueOrDefault( face.Id );
			if (tries > linearAttemptLimit) {
				this.LogLine( $"[SetDistancesFromOcean] Linear attempt limit hit for face {face.Id}" );
				continue;
			}
			linearAttempts[ face.Id ] = tries + 1;

			IReadOnlyList<Connection> connections = face.Blueprint.Connections;
			float lowestDistance = face.State.LinearDistanceFromOcean;
			bool foundLower = false;
			bool undefinedNeighbour = false;

			for (int i = 0; i < 3; i++) {
				Face neighbour = connections[ i ].GetOther( face );
				neighbours[ i ] = neighbour;
				float neighbourDistance = neighbour.State.LinearDistanceFromOcean;
				if (neighbourDistance < lowestDistance + (float) globe.ApproximateTileLength) {
					lowestDistance = neighbourDistance + (float) globe.ApproximateTileLength;
					foundLower = true;
				}
				if (neighbourDistance == float.PositiveInfinity)
					undefinedNeighbour = true;
			}

			if (!foundLower) {
				if (undefinedNeighbour && enqueuedLinear.Add( face.Id ))
					linearQueue.Enqueue( face );
				continue;
			}

			face.State.SetLinearDistanceFromOcean( lowestDistance );

			for (int i = 0; i < 3; i++) {
				Face neighbour = neighbours[ i ];
				if (neighbour.State.LinearDistanceFromOcean > lowestDistance + (float) globe.ApproximateTileLength)
					if (enqueuedLinear.Add( neighbour.Id ))
						linearQueue.Enqueue( neighbour );
			}
		}
	}

	private void SetUpwindDistancesFromOcean( GlobeModel globe ) {
		Queue<Face> setQueue = [];
		Queue<Face> unsetIdQueue = [];
		Dictionary<uint, int> attempts = [];
		HashSet<uint> enqueuedUpwind = [];

		// Initialize unsetIdQueue for land faces
		for (uint i = 0; i < globe.FaceCount; i++) {
			Face face = globe.Faces[ (int) i ];
			if (face.State.Height >= 0)
				unsetIdQueue.Enqueue( face );
			else
				face.State.SetUpwindDistanceFromOcean( 0 );
		}

		int attemptLimit = 64; // Lowered from 128
		this.LogLine( $"Land: {(double) unsetIdQueue.Count / globe.FaceCount * 100:N2}% {unsetIdQueue.Count} tiles" );

		bool HandleFaceUpwind( Face face, Queue<Face> queue, Dictionary<uint, int> attempts, int attemptLimit, HashSet<uint> enqueuedUpwind ) {
			if (face.State.UpwindDistanceFromOcean != float.PositiveInfinity)
				return true;
			Face upwindFace = face.Blueprint.GetFaceInDirection( -face.State.WindDirection );
			if (upwindFace.State.UpwindDistanceFromOcean == float.PositiveInfinity) {
				int numberOfAttempts = attempts.GetValueOrDefault( face.Id );
				if (numberOfAttempts > attemptLimit)
					return true;
				attempts[ face.Id ] = numberOfAttempts + 1;
				return false;
			}
			Vector3<float> positionDiff = (face.Blueprint.GetCenter() - upwindFace.Blueprint.GetCenter()).Normalize<Vector3<float>, float>();
			float aligment = 2 - float.Max( upwindFace.State.WindDirection.Dot( positionDiff ), 0 );
			face.State.SetUpwindDistanceFromOcean( upwindFace.State.UpwindDistanceFromOcean + (float) globe.ApproximateTileLength * aligment );
			Face nextFace = face.Blueprint.GetFaceInDirection( face.State.WindDirection );
			if (enqueuedUpwind.Add( nextFace.Id ))
				queue.Enqueue( nextFace );
			return true;
		}

		while (unsetIdQueue.TryDequeue( out Face? face )) {
			if (!HandleFaceUpwind( face, setQueue, attempts, attemptLimit, enqueuedUpwind ))
				unsetIdQueue.Enqueue( face );

			while (setQueue.TryDequeue( out face ))
				HandleFaceUpwind( face, setQueue, attempts, attemptLimit, enqueuedUpwind );
		}
	}
}