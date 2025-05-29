using Engine;
using Engine.Generation.Meshing;

namespace Civlike.World.GenerationState.Steps;

[Engine.Processing.Do<IGenerationStep>.After<CreateVerticesStep>]
public sealed class CreateFacesStep : GlobeGenerationProcessingStepBase<GeneratingGlobeBase, GlobeGeneratorParameterBase> {
	public override string StepDisplayName => "Creating faces";

	public override void Process( GeneratingGlobeBase globe, GlobeGeneratorParameterBase parameters ) {
		if (globe.Icosphere is null)
			throw new InvalidOperationException( "Icosphere is null." );
		Icosphere sphere = globe.Icosphere;

		if (globe.Vertices is null)
			throw new InvalidOperationException( "Vertices are null." );
		IReadOnlyList<Vertex> vertices = globe.Vertices;

		Type faceWithStateType = typeof( Face<> ).MakeGenericType(globe.FaceStateType);
		ResolvedType resolvedType = faceWithStateType.Resolve();
		IReadOnlyList<uint> indices = sphere.GetIndices();
		FaceBase[] faces = new FaceBase[ indices.Count / 3 ];
		Edge[] edges = new Edge[ indices.Count / 2 ];
		int edgeId = 0;
		Dictionary<EdgeIndices, Edge> allEdges = [];
		Span<EdgeIndices> currentEdges = stackalloc EdgeIndices[ 3 ];
		for (int i = 0; i < indices.Count; i += 3) {
			int id = i / 3;
			uint vertexA = indices[ i ];
			uint vertexB = indices[ i + 1 ];
			uint vertexC = indices[ i + 2 ];

			Vertex[] faceVertices = [ vertices[ (int) vertexA ], vertices[ (int) vertexB ], vertices[ (int) vertexC ] ];

			Edge[] faceEdges = new Edge[ 3 ];
			currentEdges[ 0 ] = new EdgeIndices( vertexA, vertexB );
			currentEdges[ 1 ] = new EdgeIndices( vertexB, vertexC );
			currentEdges[ 2 ] = new EdgeIndices( vertexC, vertexA );

			for (int j = 0; j < currentEdges.Length; j++) {
				if (!allEdges.TryGetValue( currentEdges[ j ], out Edge? edge )) {
					edge = new( edgeId, vertices[ (int) currentEdges[ j ].VertexA ], vertices[ (int) currentEdges[ j ].VertexB ] );
					allEdges[currentEdges[ j ] ] = edge;
					edges[ edgeId++ ] = edge;
				}
				faceEdges[ j ] = edge;
			}

			FaceBase face = resolvedType.CreateInstance( [(uint) id, globe, faceVertices, faceEdges] ) as FaceBase ?? throw new InvalidOperationException( $"Failed to create instance of {faceWithStateType}" );
			faces[ id ] = face;
		}

		globe.SetEdges( edges );
		globe.SetFaces( faces );
	}
}
