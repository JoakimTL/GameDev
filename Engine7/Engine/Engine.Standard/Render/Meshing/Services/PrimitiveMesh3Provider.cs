using Engine.Module.Render.Ogl.Scenes.Services;
using Engine.Module.Render.Ogl.Scenes;
using System.Collections.Frozen;
using Engine.Module.Entities.Render;

namespace Engine.Standard.Render.Meshing.Services;

public sealed class PrimitiveMesh3Provider( MeshService meshService ) : Identifiable, IInitializable, IRenderEntityServiceProvider {

	private FrozenDictionary<Primitive3, ReadOnlyVertexMesh<Vertex3>> _primitives = new Dictionary<Primitive3, ReadOnlyVertexMesh<Vertex3>>().ToFrozenDictionary();

	public IMesh Get( Primitive3 primitive ) => _primitives[ primitive ];

	public void Initialize() {
		Dictionary<Primitive3, ReadOnlyVertexMesh<Vertex3>> primitives = new() {
			{
				Primitive3.Cube,
				meshService.CreateReadOnlyMesh(
					[
						new Vertex3((-1, -1, -1), (0, 0), (0, 0, -1), 255),
						new Vertex3((1, -1, -1), (1, 0), (0, 0, -1), 255),
						new Vertex3((1, 1, -1), (1, 1), (0, 0, -1), 255),
						new Vertex3((-1, 1, -1), (0, 1), (0, 0, -1), 255),
						new Vertex3((-1, -1, 1), (0, 0), (0, 0, 1), 255),
						new Vertex3((1, -1, 1), (1, 0), (0, 0, 1), 255),
						new Vertex3((1, 1, 1), (1, 1), (0, 0, 1), 255),
						new Vertex3((-1, 1, 1), (0, 1), (0, 0, 1), 255)
					],
					[
						0, 1, 2,
						2, 0, 3,
						4, 5, 6,
						6, 4, 7,
					], "P-Cube"
				)
			}
		};

		_primitives = primitives.ToFrozenDictionary();
		//Move to Engine.Standard.
	}
}