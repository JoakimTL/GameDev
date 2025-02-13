﻿using Engine.Module.Render.Ogl.Scenes;
using System.Collections.Frozen;
using Engine.Module.Render.Ogl.Services;
using Engine.Module.Render;

namespace Engine.Standard.Render.Meshing.Services;

public sealed class PrimitiveMesh3Provider( MeshService meshService ) : Identifiable, IInitializable, IRenderServiceProvider {

	private FrozenDictionary<Primitive3, ReadOnlyVertexMesh<Vertex3>> _primitives = new Dictionary<Primitive3, ReadOnlyVertexMesh<Vertex3>>().ToFrozenDictionary();

	public IMesh Get( Primitive3 primitive ) => _primitives[ primitive ];

	public void Initialize() {
		Dictionary<Primitive3, ReadOnlyVertexMesh<Vertex3>> primitives = new() {
			{
				Primitive3.Cube,
				meshService.CreateReadOnlyMesh(
					[
						new Vertex3((-1, -1, -1), (0, 0), (0, 0, -1),	255),
						new Vertex3((1, -1, -1), (1, 0), (0, 0, -1),	255),
						new Vertex3((1, 1, -1), (1, 1), (0, 0, -1),		255),
						new Vertex3((-1, 1, -1), (0, 1), (0, 0, -1),	255),
						new Vertex3((-1, -1, 1), (0, 0), (0, 0, 1),		255),
						new Vertex3((1, -1, 1), (1, 0), (0, 0, 1),		255),
						new Vertex3((1, 1, 1), (1, 1), (0, 0, 1),		255),
						new Vertex3((-1, 1, 1), (0, 1), (0, 0, 1),		255),
						new Vertex3((-1, -1, -1), (0, 0), (0, -1, 0),	255),
						new Vertex3((1, -1, -1), (1, 0), (0, -1, 0),	255),
						new Vertex3((1, -1, 1), (1, 1), (0, -1, 0),		255),
						new Vertex3((-1, -1, 1), (0, 1), (0, -1, 0),	255),
						new Vertex3((-1, 1, -1), (0, 0), (0, 1, 0),		255),
						new Vertex3((1, 1, -1), (1, 0), (0, 1, 0),		255),
						new Vertex3((1, 1, 1), (1, 1), (0, 1, 0),		255),
						new Vertex3((-1, 1, 1), (0, 1), (0, 1, 0),		255),
						new Vertex3((-1, -1, -1), (0, 0), (-1, 0, 0),	255),
						new Vertex3((-1, 1, -1), (1, 0), (-1, 0, 0),	255),
						new Vertex3((-1, 1, 1), (1, 1), (-1, 0, 0),		255),
						new Vertex3((-1, -1, 1), (0, 1), (-1, 0, 0),	255),
						new Vertex3((1, -1, -1), (0, 0), (1, 0, 0),		255),
						new Vertex3((1, 1, -1), (1, 0), (1, 0, 0),		255),
						new Vertex3((1, 1, 1), (1, 1), (1, 0, 0),		255),
						new Vertex3((1, -1, 1), (0, 1), (1, 0, 0),		255)
					],
					[
						0, 2, 1,
						0, 3, 2,
						4, 5, 6,
						4, 6, 7,
						8, 9, 10,
						8, 10, 11,
						12, 14, 13,
						12, 15, 14,
						16, 18, 17,
						16, 19, 18,
						20, 21, 22,
						20, 22, 23
					], "P-Cube"
				)
			}
		};

		_primitives = primitives.ToFrozenDictionary();
		//Move to Engine.Standard.
	}
}