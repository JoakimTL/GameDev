using Civlike.Messages;
using Civlike.World.GameplayState;
using Civlike.World.GenerationState;
using Civlike.World.TectonicGeneration;
using Engine;
using Engine.Modularity;
using Engine.Structures;

namespace Civlike.World;
public static class GlobeGenerator {

	public static Globe Generate<TGlobeType, TParameters>( TParameters parameters, Type[]? stepsToIgnore = null ) 
		where TGlobeType : GeneratingGlobeBase, new()
		where TParameters : GlobeGeneratorParameterBase {
		TGlobeType globeInProgress = new();
		globeInProgress.Initialize( parameters );

		List<Type> appropriateStepTypes = [ .. TypeManager.Registry
			.GetAllNonAbstractSubclassesOf( typeof( GlobeGenerationProcessingStepBase<,> ) )
			.Except( stepsToIgnore ?? Type.EmptyTypes )
			.Where( p =>
				TypeManager.Registry.HasExactGenericArguments( p, typeof( GlobeGenerationProcessingStepBase<,> ), typeof( GeneratingGlobeBase ), typeof( GlobeGeneratorParameterBase ) ) ||
				TypeManager.Registry.HasAssignableGenericArguments( p, typeof( GlobeGenerationProcessingStepBase<,> ), typeof( TGlobeType ), typeof( TParameters ) ) ) ];


		TypeDigraph<IGenerationStep> typeDigraph = new();
		foreach (Type? stepType in appropriateStepTypes)
			typeDigraph.Add( stepType );
		IReadOnlyList<Type> typeSequence = typeDigraph.GetTypes();

		object[] steps = [ .. typeSequence.Select( p => p.Resolve().CreateInstance( null ) ?? throw new Exception( $"Failed to create instance of {p}" ) ) ];

		/*
		 * Insolation & radiation → net SW/LW
		 * Energy balance → update temperature
		 * Evaporation → update humidity
		 * Pressure & density → ideal-gas law
		 * Wind solver → pressure gradients, Coriolis, drag
		 * Ocean currents → wind stress → Ekman drift
		 * Advection → T, humidity, SST
		 * Cloud/precipitation → RH threshold → rain/snow
		 * Hydrology → infiltration, runoff routing, soil-moisture update
		 * Vegetation & z0z0​ update
		 * Accumulate daily metrics → check annual drift
		 */

		for (int i = 0; i < steps.Length; i++) {
			object step = steps[ i ];
			if (step is GlobeGenerationProcessingStepBase<TGlobeType, TParameters> stepAsSpecific) {
				MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( $"Step {i + 1}/{steps.Length}: {stepAsSpecific.StepDisplayName}" ) );
				RunStep( stepAsSpecific, globeInProgress, parameters );
				continue;
			}
			if (step is GlobeGenerationProcessingStepBase<GeneratingGlobeBase, GlobeGeneratorParameterBase> stepAsBase) {
				MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( $"Step {i + 1}/{steps.Length}: {stepAsBase.StepDisplayName}" ) );
				RunStep( stepAsBase, globeInProgress, parameters );
				continue;
			}
			throw new InvalidOperationException( $"Step {i + 1}/{steps.Length} is not of the expected type." );
		}

		return ConvertInProgressToFinishedGlobe<TGlobeType, TParameters>( globeInProgress );
	}

	private static void RunStep<TGlobeType, TParameters>( GlobeGenerationProcessingStepBase<TGlobeType, TParameters> step, TGlobeType globe, TParameters parameters ) where TGlobeType : GeneratingGlobeBase where TParameters : GlobeGeneratorParameterBase {
		int loopCount = step.GetLoopCount( globe, parameters );
		if (loopCount <= 0)
			throw new ArgumentOutOfRangeException( nameof( loopCount ), "Loop count must be greater than zero." );
		if (loopCount == 1) {
			step.Process( globe, parameters );
			return;
		}

		for (int i = 0; i < loopCount; i++) {
			MessageBus.PublishAnonymously( new WorldGenerationSubProgressMessage( $"Step {step.StepDisplayName} iteration {i + 1}/{loopCount}" ) );
			step.Process( globe, parameters );
		}
	}

	public static Globe ConvertInProgressToFinishedGlobe<TGlobeType, TParameters>( TGlobeType globeInProgress ) where TGlobeType : GeneratingGlobeBase where TParameters : GlobeGeneratorParameterBase {
		MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( $"Building world" ) );
		(GenerationState.Vertex generated, GameplayState.Vertex.Builder builder)[] vertexBuilders = [ .. globeInProgress.Vertices.Select( p => (generated: p, builder: new GameplayState.Vertex.Builder( p.PackedNormal, p.Height, globeInProgress.Radius )) ) ];
		(FaceBase generated, GameplayState.Face.Builder builder)[] faceBuilders = [ .. globeInProgress.Faces.Select( p => (generated: p, builder: new GameplayState.Face.Builder( p.Id, p.TerrainType )) ) ];
		GameplayState.Edge[] edges = [ .. globeInProgress.Edges.Select( p => new GameplayState.Edge( vertexBuilders[ p.VertexA.Id ].builder.Vertex, vertexBuilders[ p.VertexB.Id ].builder.Vertex, faceBuilders[ p.FaceA.Id ].builder.Face, faceBuilders[ p.FaceB.Id ].builder.Face ) ) ];
		MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( $"Populating vertices and faces" ) );
		foreach ((GenerationState.FaceBase generated, GameplayState.Face.Builder builder) in faceBuilders) {
			builder.Vertices.AddRange( generated.Vertices.Select( p => vertexBuilders[ p.Id ].builder.Vertex ) );
			builder.Edges.AddRange( generated.Edges.Select( p => edges[ p.Id ] ) );
			builder.Neighbours.AddRange( generated.Edges.Select( p => faceBuilders[p.GetOther( generated ).Id ].builder.Face ) );
			generated.Apply( builder );
			builder.GenerationFace = generated;
			builder.Complete();
		}
		foreach ((GenerationState.Vertex generated, GameplayState.Vertex.Builder builder) in vertexBuilders) {
			builder.ConnectedFaces.AddRange( generated.ConnectedFaces.Select( p => faceBuilders[ p.Id ].builder.Face ) );
			builder.Complete();
		}

		GameplayState.Vertex[] vertices = [ .. vertexBuilders.Select( p => p.builder.Vertex ) ];
		GameplayState.Face[] faces = [ .. faceBuilders.Select( p => p.builder.Face ) ];

		MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( $"Creating clusters" ) );
		BoundedRenderCluster[] renderClusters = GetRenderClusters( edges, faces );

		MessageBus.PublishAnonymously( new WorldGenerationCompleteMessage( $"Globe generation complete!" ) );
		return new Globe( Guid.NewGuid(), vertices, faces, renderClusters, globeInProgress.Radius, globeInProgress.TileArea, globeInProgress.TileLength );
	}

	public static BoundedRenderCluster[] GetRenderClusters( GameplayState.Edge[] edges, GameplayState.Face[] faces ) {
		int clustersPerAxis = 16; // This can be adjusted based on the desired granularity of the clusters
		int totalClusters = clustersPerAxis * clustersPerAxis * clustersPerAxis;
		int GetIndex(Vector3<int> xyz) => xyz.X + xyz.Y * clustersPerAxis + xyz.Z * clustersPerAxis * clustersPerAxis;
		Vector3<int> TurnIntoXyz( Vector3<float> vector ) => vector.Add(1).ScalarMultiply(clustersPerAxis / 2).CastSaturating<float,int>();
		AABB<Vector3<float>> baseBounds = AABB.Create<Vector3<float>>( [ 0, 2f / clustersPerAxis ] );
		Vector3<float> length = baseBounds.GetLengths() * clustersPerAxis; // Assuming uniform length for simplicity

		BoundedRenderCluster.Builder[] clusterBuilders = new BoundedRenderCluster.Builder[totalClusters];
		for (int x = 0; x < clustersPerAxis; x++) {
			for (int y = 0; y < clustersPerAxis; y++) {
				for (int z = 0; z < clustersPerAxis; z++) {
					Vector3<float> offset = new Vector3<float>( x, y, z ) - clustersPerAxis / 2;
					clusterBuilders[ GetIndex( (x, y, z) ) ] = new BoundedRenderCluster.Builder( baseBounds.MoveBy(offset.ScalarDivide( clustersPerAxis / 2)) );
				}
			}
		}
		foreach (GameplayState.Edge edge in edges) {
			Vector3<float> center = (edge.VertexA.Vector + edge.VertexB.Vector) / 2;
			Vector3<int> xyz = TurnIntoXyz( center );
			int index = GetIndex( xyz );
			clusterBuilders[ index ].Edges.Add( edge );
		}
		foreach (GameplayState.Face face in faces) {
			Vector3<float> center = face.Blueprint.GetCenter();
			Vector3<int> xyz = TurnIntoXyz( center );
			int index = GetIndex( xyz );
			clusterBuilders[ index ].Faces.Add( face );
		}

		List<BoundedRenderCluster> clusters = [];

		for (int i = 0; i < clusterBuilders.Length; i++) 
			if (clusterBuilders[ i ].HasFaces) 
				clusters.Add( clusterBuilders[ i ].Build( (uint) clusters.Count ) );

		return [ .. clusters ];
	}

}
