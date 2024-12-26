using Engine.Module.Entities.Render;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Standard.Render.Meshing.Services;

public sealed class PrimitiveMesh2Provider( MeshService meshService ) : IRenderEntityServiceProvider, IInitializable {

	private ReadOnlyVertexMesh<Vertex3>? _triangle;
	public ReadOnlyVertexMesh<Vertex3> Triangle => _triangle ?? throw new InvalidOperationException( "Triangle mesh not initialized" );

	public void Initialize() {
		_triangle = CreateTriangle();
	}

	private ReadOnlyVertexMesh<Vertex3> CreateTriangle()
		=> meshService.CreateReadOnlyMesh( [ new Vertex3( new( .5f, 0, 0f ), (1, 1), (0, 0, -1), 255 ), new Vertex3( new( 0f, 0f, 1f ), (0.5f, 0), (0, 0, -1), 255 ), new Vertex3( new( -.5f, 0, 0f ), (0, 1), (0, 0, -1), 255 ) ], [ 0, 1, 2 ] );
}