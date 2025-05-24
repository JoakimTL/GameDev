using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;
using System.Collections.Frozen;

namespace Engine.Standard.Render.Meshing.Services;

public sealed class PrimitiveMesh2Provider( MeshService meshService ) : IServiceProvider, IInitializable {

	private FrozenDictionary<Primitive2, ReadOnlyVertexMesh<Vertex2>> _primitives = new Dictionary<Primitive2, ReadOnlyVertexMesh<Vertex2>>().ToFrozenDictionary();

	public IMesh Get( Primitive2 primitive ) => this._primitives[ primitive ];

	public void Initialize() {
		Dictionary<Primitive2, ReadOnlyVertexMesh<Vertex2>> primitives = new() {
			{
				Primitive2.Rectangle,
				meshService.CreateReadOnlyMesh(
					[
						new Vertex2((-1, -1), (0, 0),   255),
						new Vertex2((1, -1), (1, 0),    255),
						new Vertex2((1, 1), (1, 1),     255),
						new Vertex2((-1, 1), (0, 1),    255),
					],
					[
						0, 1, 2,
						0, 2, 3,
					], "P-Rectangle"
				)
			}
		};

		this._primitives = primitives.ToFrozenDictionary();
	}
}