////TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

////TODO have the globes be generated using a separate module. That module will then communicate new and deleted globes and changes that occur on them such that all other modules can update their data. The globe module will really only update once a seldom terrain edit or something similar happens. Tile resources, cities, buildings, whatever else is handled by the logic module using tile ids.

//using Civs.World.Generation;
//using Engine;

//namespace Civs.World.CurrentWorld;

//public sealed class GlobeStateModel {

//	private float[] _tileHeights;

//	public GlobeStateModel( Guid id, double radius, int seed, double maxTerrainHeight, double sealevel, GlobeBlueprintModel blueprint ) {
//		Id = id;
//		Radius = radius;
//		SurfaceArea = radius * radius * 4 * double.Pi;
//		TileSurfaceArea = SurfaceArea / blueprint.FaceCount;
//		_tileHeights = new float[ blueprint.FaceCount ];

//		GenerateLandmass( seed, maxTerrainHeight, sealevel, blueprint );
//	}

//	//Find a way to do this without procedural generation...
//	private void GenerateLandmass( int seed, double maxTerrainHeight, double sealevel, GlobeBlueprintModel blueprint ) {
//		double maxSeaDepth = sealevel;
//		double maxLandHeight = maxTerrainHeight - sealevel;

//		Random seedProvider = new( seed );
//		Noise3 coarseHeightNoise = new( seedProvider.Next(), 5 );
//		Noise3 fineHeightNoise = new( seedProvider.Next(), 23 );
//		Noise3 coarseHeightNoise2 = new( seedProvider.Next(), 3 );
//		Noise3 fineHeightNoise2 = new( seedProvider.Next(), 17 );

//		Noise3 landLargeNoise = new( seedProvider.Next(), 2 );
//		Noise3 landFineNoise = new( seedProvider.Next(), 13 );
//		Noise3 landTranslationStrengthNoise = new( seedProvider.Next(), 7 );

//		FiniteVoronoiNoise3 voronoiTranslation = new( new( seedProvider.Next() ), 0.5f, 1 );
//		FiniteVoronoiNoise3 voronoiRidgeNoise = new( new( seedProvider.Next() ), 0.25f, 1 );
//		FiniteVoronoiNoise3 voronoiRidgeNoiseFine = new( new( seedProvider.Next() ), 0.0625f, 1 );
//		//double minPresentHeight = float.MaxValue;
//		//double maxPresentHeight = float.MinValue;
//		for (uint i = 0; i < blueprint.FaceCount; i++) {
//			//https://www.youtube.com/watch?v=WumyfLEa6bU
//			//https://iquilezles.org/articles/morenoise/
//			FaceRenderModelWithIdAndVertices faceWithVertices = blueprint.GetFaceWithVertices( i );
//			Vector3<float> center = (faceWithVertices.VertexA + faceWithVertices.VertexB + faceWithVertices.VertexC) / 3;

//			Vector3<float> translation = voronoiTranslation.NoiseVector( center );
//			Vector3<float> smallTranslation = translation * 0.05f;

//			float lTs = landTranslationStrengthNoise.Noise( center );
//			float landmassN = landLargeNoise.Noise( center + smallTranslation * lTs ) * 0.85f + landFineNoise.Noise( center + translation * lTs ) * 0.15f;
//			landmassN = float.Sqrt( landmassN );
//			bool isLand = landmassN > 0.73f;

//			float n = (coarseHeightNoise.Noise( center ) * 0.9f + fineHeightNoise.Noise( center ) * 0.1f) * (coarseHeightNoise2.Noise( center ) * 0.7f + fineHeightNoise2.Noise( center ) * 0.3f);
//			float rN = voronoiRidgeNoise.Noise( center ) * 0.85f + voronoiRidgeNoiseFine.Noise( center ) * 0.15f;
//			n *= rN;

//			if (isLand) 				_tileHeights[ faceWithVertices.Id ] = (float) (n * landmassN * maxLandHeight);
//else 				_tileHeights[ faceWithVertices.Id ] = (float) (-n * landmassN * maxSeaDepth);

//		}
//	}

//	public Guid Id { get; }
//	public double Radius { get; }
//	public double SurfaceArea { get; }
//	public double TileSurfaceArea { get; }

//	public float GetHeight( uint tileId ) {
//		if (tileId >= _tileHeights.Length)
//			throw new ArgumentOutOfRangeException( nameof( tileId ), "Tile ID is out of range." );
//		return _tileHeights[ tileId ];
//	}

//}
