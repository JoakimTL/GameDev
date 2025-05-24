using Civlike.Messages;
using Civlike.World.GameplayState;
using Civlike.World.GenerationState;
using Civlike.World.TectonicGeneration;
using Engine;
using Engine.Modularity;
using Engine.Structures;

namespace Civlike.World;
public static class GlobeGenerator {

	public static Globe Generate<TGlobeType, TParameters>( TParameters parameters, Type[]? stepsToIgnore = null ) where TGlobeType : GeneratingGlobeBase, new() where TParameters : GlobeGeneratorParameterBase {
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

		for (int i = 0; i < steps.Length; i++) {
			object step = steps[ i ];
			if (step is GlobeGenerationProcessingStepBase<TGlobeType, TParameters> stepAsSpecific) {
				MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( $"Step {i + 1}/{steps.Length}: {stepAsSpecific.StepDisplayName}" ) );
				stepAsSpecific.Process( globeInProgress, parameters );
				continue;
			}
			if (step is GlobeGenerationProcessingStepBase<GeneratingGlobeBase, GlobeGeneratorParameterBase> stepAsBase) {
				MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( $"Step {i + 1}/{steps.Length}: {stepAsBase.StepDisplayName}" ) );
				stepAsBase.Process( globeInProgress, parameters );
				continue;
			}
			throw new InvalidOperationException( $"Step {i + 1}/{steps.Length} is not of the expected type." );
		}

		MessageBus.PublishAnonymously( new WorldGenerationProgressMessage( $"Generation complete, converting to gameplay world" ) );
		return ConvertInProgressToFinishedGlobe<TGlobeType, TParameters>( globeInProgress );
	}

	public static Globe ConvertInProgressToFinishedGlobe<TGlobeType, TParameters>( TGlobeType globeInProgress ) where TGlobeType : GeneratingGlobeBase where TParameters : GlobeGeneratorParameterBase {
		(GenerationState.Vertex generated, GameplayState.Vertex.Builder builder)[] vertexBuilders = [ .. globeInProgress.Vertices.Select( p => (generated: p, builder: new GameplayState.Vertex.Builder( p.PackedNormal, p.Height )) ) ];
		(GenerationState.Face generated, GameplayState.Face.Builder builder)[] faceBuilders = [ .. globeInProgress.Faces.Select( p => (generated: p, builder: new GameplayState.Face.Builder( p.Id, p.TerrainType )) ) ];
		GameplayState.Edge[] edges = [ .. globeInProgress.Edges.Select( p => new GameplayState.Edge( vertexBuilders[ p.VertexA.Id ].builder.Vertex, vertexBuilders[ p.VertexB.Id ].builder.Vertex, faceBuilders[ p.FaceA.Id ].builder.Face, faceBuilders[ p.FaceB.Id ].builder.Face ) ) ];
		foreach ((GenerationState.Face generated, GameplayState.Face.Builder builder) in faceBuilders) {
			builder.Vertices.AddRange( generated.Vertices.Select( p => vertexBuilders[ p.Id ].builder.Vertex ) );
			builder.Edges.AddRange( generated.Edges.Select( p => edges[ p.Id ] ) );
			builder.Neighbours.AddRange( generated.Neighbours.Select( p => faceBuilders[ p.Id ].builder.Face ) );
			builder.Debug_Arrow = generated.Get<TectonicFaceState>().BaselineValues.Gradient;
			builder.Debug_Color = (generated.Get<TectonicFaceState>().BaselineValues.MeanSlope / 0.35f, generated.Get<TectonicFaceState>().BaselineValues.RuggednessFactor, generated.Get<TectonicFaceState>().BaselineValues.SeismicActivity, 1);
			builder.Complete();
		}
		foreach ((GenerationState.Vertex generated, GameplayState.Vertex.Builder builder) in vertexBuilders) {
			builder.ConnectedFaces.AddRange( generated.ConnectedFaces.Select( p => faceBuilders[ p.Id ].builder.Face ) );
			builder.Complete();
		}

		GameplayState.Vertex[] vertices = [ .. vertexBuilders.Select( p => p.builder.Vertex ) ];
		GameplayState.Face[] faces = [ .. faceBuilders.Select( p => p.builder.Face ) ];

		BoundedRenderCluster[] renderClusters = GetRenderClusters( edges, faces );

		return new Globe( Guid.NewGuid(), vertices, faces, renderClusters, globeInProgress.Radius, globeInProgress.TileArea, globeInProgress.ApproximateTileLength );
	}

	public static BoundedRenderCluster[] GetRenderClusters( GameplayState.Edge[] edges, GameplayState.Face[] faces ) {
		OcTree<GameplayState.Face, float> faceTree = new( AABB.Create<Vector3<float>>( [ -1, 1 ] ), 3, false );
		OcTree<GameplayState.Edge, float> edgeTree = new( AABB.Create<Vector3<float>>( [ -1, 1 ] ), 3, false );

		foreach (GameplayState.Edge edge in edges)
			edgeTree.Add( edge );
		foreach (GameplayState.Face face in faces)
			faceTree.Add( face );

		List<IReadOnlyBranch<GameplayState.Face, float>> faceBranches = [ .. faceTree.GetBranches().Where( p => p.Contents.Count > 0 ) ];
		List<IReadOnlyBranch<GameplayState.Edge, float>> edgeBranches = [ .. edgeTree.GetBranches().Where( p => p.Contents.Count > 0 ) ];
		List<BoundedRenderCluster> clusters = new( faceBranches.Count );
		foreach ((AABB<Vector3<float>> bounds, IReadOnlyBranch<GameplayState.Face, float> faces, IReadOnlyBranch<GameplayState.Edge, float>? edges) pair in BoundedRenderCluster.CreateClusterPairs( faceBranches, edgeBranches ))
			clusters.Add( new BoundedRenderCluster( (uint) clusters.Count, pair.bounds, [ .. pair.faces.Contents ], pair.edges?.Contents.ToList() ?? [] ) );
		return [ .. clusters ];
	}

}
