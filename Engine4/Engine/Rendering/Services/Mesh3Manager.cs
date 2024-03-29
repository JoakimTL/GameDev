﻿using System.Numerics;
using Engine.Rendering.Standard.Meshing;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;

namespace Engine.Rendering.Services;

public class Mesh3Manager : ModuleService, IInitializable {

	private bool _initialized;
	private VertexMesh<Vertex3>? _cube;
	private VertexMesh<Vector3>? _cubePfx;
	private VertexMesh<Vertex3>? _boxY;
	private VertexMesh<Vertex3>? _boxZ;
	private VertexMesh<Vertex3>? _icosphere0;
	private VertexMesh<Vertex3>? _icosphere1;
	private VertexMesh<Vertex3>? _icosphere2;
	private VertexMesh<Vertex3>? _icosphere3;
	private VertexMesh<Vertex3>? _faceUp;
	private VertexMesh<Vertex3>? _faceDown;
	private VertexMesh<Vertex3>? _faceRight;
	private VertexMesh<Vertex3>? _faceLeft;
	private VertexMesh<Vertex3>? _faceForward;
	private VertexMesh<Vertex3>? _faceBackward;
	public VertexMesh<Vertex3> Cube => this._cube ?? throw new NullReferenceException( "Manager not initialized!" );
	public VertexMesh<Vector3> CubePfx => this._cubePfx ?? throw new NullReferenceException( "Manager not initialized!" );
	public VertexMesh<Vertex3> BoxY => this._boxY ?? throw new NullReferenceException( "Manager not initialized!" );
	public VertexMesh<Vertex3> BoxZ => this._boxZ ?? throw new NullReferenceException( "Manager not initialized!" );
	public VertexMesh<Vertex3> Icosphere0 => this._icosphere0 ?? throw new NullReferenceException( "Manager not initialized!" );
	public VertexMesh<Vertex3> Icosphere1 => this._icosphere1 ?? throw new NullReferenceException( "Manager not initialized!" );
	public VertexMesh<Vertex3> Icosphere2 => this._icosphere2 ?? throw new NullReferenceException( "Manager not initialized!" );
	public VertexMesh<Vertex3> Icosphere3 => this._icosphere3 ?? throw new NullReferenceException( "Manager not initialized!" );
	public VertexMesh<Vertex3> FaceUp => this._faceUp ?? throw new NullReferenceException( "Manager not initialized!" );
	public VertexMesh<Vertex3> FaceDown => this._faceDown ?? throw new NullReferenceException( "Manager not initialized!" );
	public VertexMesh<Vertex3> FaceRight => this._faceRight ?? throw new NullReferenceException( "Manager not initialized!" );
	public VertexMesh<Vertex3> FaceLeft => this._faceLeft ?? throw new NullReferenceException( "Manager not initialized!" );
	public VertexMesh<Vertex3> FaceForward => this._faceForward ?? throw new NullReferenceException( "Manager not initialized!" );
	public VertexMesh<Vertex3> FaceBackward => this._faceBackward ?? throw new NullReferenceException( "Manager not initialized!" );

	public void Initialize() {
		if ( this._initialized ) {
			this.LogError( "Already initialized!" );
			return;
		}
		this._initialized = true;

		this._cube = VertexMeshUtilities.Mesh3.CreateNormalized( "cube",
			new Vertex3[] {
					new Vertex3(new Vector3(-1.0f, -1.0f, -1.0f)).SetUV(new Vector2(0, 0)),
					new Vertex3(new Vector3(-1.0f,  1.0f, -1.0f)).SetUV(new Vector2(0, 1)),
					new Vertex3(new Vector3( 1.0f, -1.0f, -1.0f)).SetUV(new Vector2(1, 0)),

					new Vertex3(new Vector3( 1.0f,  1.0f, -1.0f)).SetUV(new Vector2(1, 1)),
					new Vertex3(new Vector3( 1.0f, -1.0f, -1.0f)).SetUV(new Vector2(1, 0)),
					new Vertex3(new Vector3(-1.0f,  1.0f, -1.0f)).SetUV(new Vector2(0, 1)),

					new Vertex3(new Vector3(-1.0f, -1.0f,  1.0f)).SetUV(new Vector2(0, 0)),
					new Vertex3(new Vector3( 1.0f, -1.0f,  1.0f)).SetUV(new Vector2(1, 0)),
					new Vertex3(new Vector3(-1.0f,  1.0f,  1.0f)).SetUV(new Vector2(0, 1)),

					new Vertex3(new Vector3( 1.0f,  1.0f,  1.0f)).SetUV(new Vector2(1, 1)),
					new Vertex3(new Vector3(-1.0f,  1.0f,  1.0f)).SetUV(new Vector2(0, 1)),
					new Vertex3(new Vector3( 1.0f, -1.0f,  1.0f)).SetUV(new Vector2(1, 0)),

					new Vertex3(new Vector3(-1.0f, -1.0f, -1.0f)).SetUV(new Vector2(0, 0)),
					new Vertex3(new Vector3( 1.0f, -1.0f, -1.0f)).SetUV(new Vector2(1, 0)),
					new Vertex3(new Vector3(-1.0f, -1.0f,  1.0f)).SetUV(new Vector2(0, 1)),

					new Vertex3(new Vector3( 1.0f, -1.0f,  1.0f)).SetUV(new Vector2(1, 1)),
					new Vertex3(new Vector3(-1.0f, -1.0f,  1.0f)).SetUV(new Vector2(0, 1)),
					new Vertex3(new Vector3( 1.0f, -1.0f, -1.0f)).SetUV(new Vector2(1, 0)),

					new Vertex3(new Vector3(-1.0f,  1.0f, -1.0f)).SetUV(new Vector2(0, 0)),
					new Vertex3(new Vector3(-1.0f,  1.0f,  1.0f)).SetUV(new Vector2(0, 1)),
					new Vertex3(new Vector3( 1.0f,  1.0f, -1.0f)).SetUV(new Vector2(1, 0)),

					new Vertex3(new Vector3( 1.0f,  1.0f,  1.0f)).SetUV(new Vector2(1, 1)),
					new Vertex3(new Vector3( 1.0f,  1.0f, -1.0f)).SetUV(new Vector2(1, 0)),
					new Vertex3(new Vector3(-1.0f,  1.0f,  1.0f)).SetUV(new Vector2(0, 1)),

					new Vertex3(new Vector3(-1.0f, -1.0f, -1.0f)).SetUV(new Vector2(0, 0)),
					new Vertex3(new Vector3(-1.0f, -1.0f,  1.0f)).SetUV(new Vector2(0, 1)),
					new Vertex3(new Vector3(-1.0f,  1.0f, -1.0f)).SetUV(new Vector2(1, 0)),

					new Vertex3(new Vector3(-1.0f,  1.0f,  1.0f)).SetUV(new Vector2(1, 1)),
					new Vertex3(new Vector3(-1.0f,  1.0f, -1.0f)).SetUV(new Vector2(1, 0)),
					new Vertex3(new Vector3(-1.0f, -1.0f,  1.0f)).SetUV(new Vector2(0, 1)),

					new Vertex3(new Vector3( 1.0f, -1.0f, -1.0f)).SetUV(new Vector2(0, 0)),
					new Vertex3(new Vector3( 1.0f,  1.0f, -1.0f)).SetUV(new Vector2(1, 0)),
					new Vertex3(new Vector3( 1.0f, -1.0f,  1.0f)).SetUV(new Vector2(0, 1)),

					new Vertex3(new Vector3( 1.0f,  1.0f,  1.0f)).SetUV(new Vector2(1, 1)),
					new Vertex3(new Vector3( 1.0f, -1.0f,  1.0f)).SetUV(new Vector2(0, 1)),
					new Vertex3(new Vector3( 1.0f,  1.0f, -1.0f)).SetUV(new Vector2(1, 0)),
			}, new uint[] {
					0, 1, 2,
					3, 4, 5,
					6, 7, 8,
					9, 10, 11,
					12,13,14,
					15,16,17,
					18,19,20,
					21,22,23,
					24,25,26,
					27,28,29,
					30, 31,32,
					33,34,35
			}
		);

		this._cubePfx = new VertexMesh<Vector3>( "cubePfx",
			new Vector3[] {
					new Vector3(-1.0f, -1.0f, -1.0f),
					new Vector3(-1.0f,  1.0f, -1.0f),
					new Vector3( 1.0f, -1.0f, -1.0f),

					new Vector3( 1.0f,  1.0f, -1.0f),
					new Vector3( 1.0f, -1.0f, -1.0f),
					new Vector3(-1.0f,  1.0f, -1.0f),

					new Vector3(-1.0f, -1.0f,  1.0f),
					new Vector3( 1.0f, -1.0f,  1.0f),
					new Vector3(-1.0f,  1.0f,  1.0f),

					new Vector3( 1.0f,  1.0f,  1.0f),
					new Vector3(-1.0f,  1.0f,  1.0f),
					new Vector3( 1.0f, -1.0f,  1.0f),

					new Vector3(-1.0f, -1.0f, -1.0f),
					new Vector3( 1.0f, -1.0f, -1.0f),
					new Vector3(-1.0f, -1.0f,  1.0f),

					new Vector3( 1.0f, -1.0f,  1.0f),
					new Vector3(-1.0f, -1.0f,  1.0f),
					new Vector3( 1.0f, -1.0f, -1.0f),

					new Vector3(-1.0f,  1.0f, -1.0f),
					new Vector3(-1.0f,  1.0f,  1.0f),
					new Vector3( 1.0f,  1.0f, -1.0f),

					new Vector3( 1.0f,  1.0f,  1.0f),
					new Vector3( 1.0f,  1.0f, -1.0f),
					new Vector3(-1.0f,  1.0f,  1.0f),

					new Vector3(-1.0f, -1.0f, -1.0f),
					new Vector3(-1.0f, -1.0f,  1.0f),
					new Vector3(-1.0f,  1.0f, -1.0f),

					new Vector3(-1.0f,  1.0f,  1.0f),
					new Vector3(-1.0f,  1.0f, -1.0f),
					new Vector3(-1.0f, -1.0f,  1.0f),

					new Vector3( 1.0f, -1.0f, -1.0f),
					new Vector3( 1.0f,  1.0f, -1.0f),
					new Vector3( 1.0f, -1.0f,  1.0f),

					new Vector3( 1.0f,  1.0f,  1.0f),
					new Vector3( 1.0f, -1.0f,  1.0f),
					new Vector3( 1.0f,  1.0f, -1.0f),
			}, new uint[] {
					0, 1, 2,
					3, 4, 5,
					6, 7, 8,
					9, 10, 11,
					12,13,14,
					15,16,17,
					18,19,20,
					21,22,23,
					24,25,26,
					27,28,29,
					30, 31,32,
					33,34,35
			}
		);

		this._boxY = VertexMeshUtilities.Mesh3.CreateNormalized( "boxY",
			new Vertex3[] {
					new Vertex3(new Vector3(-1.0f,0.0f,-1.0f)),
					new Vertex3(new Vector3(-1.0f,0.0f, 1.0f)),
					new Vertex3(new Vector3(-1.0f, 1.0f, 1.0f)),
					new Vertex3(new Vector3(1.0f, 1.0f,-1.0f)),
					new Vertex3(new Vector3(-1.0f,0.0f,-1.0f)),
					new Vertex3(new Vector3(-1.0f, 1.0f,-1.0f)),
					new Vertex3(new Vector3(1.0f,0.0f, 1.0f)),
					new Vertex3(new Vector3(-1.0f,0.0f,-1.0f)),
					new Vertex3(new Vector3(1.0f,0.0f,-1.0f)),
					new Vertex3(new Vector3(1.0f, 1.0f,-1.0f)),
					new Vertex3(new Vector3(1.0f,0.0f,-1.0f)),
					new Vertex3(new Vector3(-1.0f,0.0f,-1.0f)),
					new Vertex3(new Vector3(-1.0f,0.0f,-1.0f)),
					new Vertex3(new Vector3(-1.0f, 1.0f, 1.0f)),
					new Vertex3(new Vector3(-1.0f, 1.0f,-1.0f)),
					new Vertex3(new Vector3(1.0f,0.0f, 1.0f)),
					new Vertex3(new Vector3(-1.0f,0.0f, 1.0f)),
					new Vertex3(new Vector3(-1.0f,0.0f,-1.0f)),
					new Vertex3(new Vector3(-1.0f, 1.0f, 1.0f)),
					new Vertex3(new Vector3(-1.0f,0.0f, 1.0f)),
					new Vertex3(new Vector3(1.0f,0.0f, 1.0f)),
					new Vertex3(new Vector3(1.0f, 1.0f, 1.0f)),
					new Vertex3(new Vector3(1.0f,0.0f,-1.0f)),
					new Vertex3(new Vector3(1.0f, 1.0f,-1.0f)),
					new Vertex3(new Vector3(1.0f,0.0f,-1.0f)),
					new Vertex3(new Vector3(1.0f, 1.0f, 1.0f)),
					new Vertex3(new Vector3(1.0f,0.0f, 1.0f)),
					new Vertex3(new Vector3(1.0f, 1.0f, 1.0f)),
					new Vertex3(new Vector3(1.0f, 1.0f,-1.0f)),
					new Vertex3(new Vector3(-1.0f, 1.0f,-1.0f)),
					new Vertex3(new Vector3(1.0f, 1.0f, 1.0f)),
					new Vertex3(new Vector3(-1.0f, 1.0f,-1.0f)),
					new Vertex3(new Vector3(-1.0f, 1.0f, 1.0f)),
					new Vertex3(new Vector3(1.0f, 1.0f, 1.0f)),
					new Vertex3(new Vector3(-1.0f, 1.0f, 1.0f)),
					new Vertex3(new Vector3(1.0f,0.0f, 1.0f))
			}, new uint[] {
					0, 1, 2,
					3, 4, 5,
					6, 7, 8,
					9, 10, 11,
					12,13,14,
					15,16,17,
					18,19,20,
					21,22,23,
					24,25,26,
					27,28,29,
					30, 31,32,
					33,34,35
			}
		);

		this._boxZ = VertexMeshUtilities.Mesh3.CreateNormalized( "boxZ",
			new Vertex3[] {
					new Vertex3(new Vector3(-1.0f,-1.0f,-1.0f)),
					new Vertex3(new Vector3(-1.0f,-1.0f, 0.0f)),
					new Vertex3(new Vector3(-1.0f, 1.0f, 0.0f)),
					new Vertex3(new Vector3( 1.0f, 1.0f,-1.0f)),
					new Vertex3(new Vector3(-1.0f,-1.0f,-1.0f)),
					new Vertex3(new Vector3(-1.0f, 1.0f,-1.0f)),
					new Vertex3(new Vector3( 1.0f,-1.0f, 0.0f)),
					new Vertex3(new Vector3(-1.0f,-1.0f,-1.0f)),
					new Vertex3(new Vector3( 1.0f,-1.0f,-1.0f)),
					new Vertex3(new Vector3( 1.0f, 1.0f,-1.0f)),
					new Vertex3(new Vector3( 1.0f,-1.0f,-1.0f)),
					new Vertex3(new Vector3(-1.0f,-1.0f,-1.0f)),
					new Vertex3(new Vector3(-1.0f,-1.0f,-1.0f)),
					new Vertex3(new Vector3(-1.0f, 1.0f, 0.0f)),
					new Vertex3(new Vector3(-1.0f, 1.0f,-1.0f)),
					new Vertex3(new Vector3( 1.0f,-1.0f, 0.0f)),
					new Vertex3(new Vector3(-1.0f,-1.0f, 0.0f)),
					new Vertex3(new Vector3(-1.0f,-1.0f,-1.0f)),
					new Vertex3(new Vector3(-1.0f, 1.0f, 0.0f)),
					new Vertex3(new Vector3(-1.0f,-1.0f, 0.0f)),
					new Vertex3(new Vector3( 1.0f,-1.0f, 0.0f)),
					new Vertex3(new Vector3( 1.0f, 1.0f, 0.0f)),
					new Vertex3(new Vector3( 1.0f,-1.0f,-1.0f)),
					new Vertex3(new Vector3( 1.0f, 1.0f,-1.0f)),
					new Vertex3(new Vector3( 1.0f,-1.0f,-1.0f)),
					new Vertex3(new Vector3( 1.0f, 1.0f, 0.0f)),
					new Vertex3(new Vector3( 1.0f,-1.0f, 0.0f)),
					new Vertex3(new Vector3( 1.0f, 1.0f, 0.0f)),
					new Vertex3(new Vector3( 1.0f, 1.0f,-1.0f)),
					new Vertex3(new Vector3(-1.0f, 1.0f,-1.0f)),
					new Vertex3(new Vector3( 1.0f, 1.0f, 0.0f)),
					new Vertex3(new Vector3(-1.0f, 1.0f,-1.0f)),
					new Vertex3(new Vector3(-1.0f, 1.0f, 0.0f)),
					new Vertex3(new Vector3( 1.0f, 1.0f, 0.0f)),
					new Vertex3(new Vector3(-1.0f, 1.0f, 0.0f)),
					new Vertex3(new Vector3( 1.0f,-1.0f, 0.0f))
			}, new uint[] {
					0, 1, 2,
					3, 4, 5,
					6, 7, 8,
					9, 10, 11,
					12,13,14,
					15,16,17,
					18,19,20,
					21,22,23,
					24,25,26,
					27,28,29,
					30, 31,32,
					33,34,35
			}
		);

		this._icosphere0 = Utilities.MeshGenerator.GenerateIcosphere( 0 );

		this._icosphere1 = Utilities.MeshGenerator.GenerateIcosphere( 1 );

		this._icosphere2 = Utilities.MeshGenerator.GenerateIcosphere( 2 );

		this._icosphere3 = Utilities.MeshGenerator.GenerateIcosphere( 3 );

		this._faceUp = VertexMeshUtilities.Mesh3.CreateNormalized( "faceUp",
			new Vertex3[] {
				new Vertex3(new Vector3( 0.0f, 1.0f, 0.0f )).SetUV(new Vector2(0, 0)),
				new Vertex3(new Vector3( 0.0f, 1.0f, 1.0f )).SetUV(new Vector2(0, 1)),
				new Vertex3(new Vector3( 1.0f, 1.0f, 0.0f )).SetUV(new Vector2(1, 0)),
				new Vertex3(new Vector3( 1.0f, 1.0f, 1.0f )).SetUV(new Vector2(1, 1)),
			}, new uint[] {
				1, 2, 0,
				3, 2, 1,
			}
		);
		this._faceDown = VertexMeshUtilities.Mesh3.CreateNormalized( "faceDown",
			new Vertex3[] {
				new Vertex3(new Vector3( 0.0f, 0.0f, 0.0f )).SetUV(new Vector2(0, 0)),
				new Vertex3(new Vector3( 0.0f, 0.0f, 1.0f )).SetUV(new Vector2(0, 1)),
				new Vertex3(new Vector3( 1.0f, 0.0f, 0.0f )).SetUV(new Vector2(1, 0)),
				new Vertex3(new Vector3( 1.0f, 0.0f, 1.0f )).SetUV(new Vector2(1, 1)),
			}, new uint[] {
				0, 2, 1,
				1, 2, 3,
			}
		);
		this._faceRight = VertexMeshUtilities.Mesh3.CreateNormalized( "faceRight",
			new Vertex3[] {
				new Vertex3(new Vector3( 1.0f, 0.0f, 0.0f )).SetUV(new Vector2(0, 0)),
				new Vertex3(new Vector3( 1.0f, 0.0f, 1.0f )).SetUV(new Vector2(0, 1)),
				new Vertex3(new Vector3( 1.0f, 1.0f, 0.0f )).SetUV(new Vector2(1, 0)),
				new Vertex3(new Vector3( 1.0f, 1.0f, 1.0f )).SetUV(new Vector2(1, 1)),
			}, new uint[] {
				0, 2, 1,
				1, 2, 3,
			}
		);
		this._faceLeft = VertexMeshUtilities.Mesh3.CreateNormalized( "faceLeft",
			new Vertex3[] {
				new Vertex3(new Vector3( 0.0f, 0.0f, 0.0f )).SetUV(new Vector2(0, 0)),
				new Vertex3(new Vector3( 0.0f, 0.0f, 1.0f )).SetUV(new Vector2(0, 1)),
				new Vertex3(new Vector3( 0.0f, 1.0f, 0.0f )).SetUV(new Vector2(1, 0)),
				new Vertex3(new Vector3( 0.0f, 1.0f, 1.0f )).SetUV(new Vector2(1, 1)),
			}, new uint[] {
				1, 2, 0,
				3, 2, 1,
			}
		);
		this._faceForward = VertexMeshUtilities.Mesh3.CreateNormalized( "faceForward",
			new Vertex3[] {
				new Vertex3(new Vector3( 0.0f, 0.0f, 1.0f )).SetUV(new Vector2(0, 0)),
				new Vertex3(new Vector3( 0.0f, 1.0f, 1.0f )).SetUV(new Vector2(0, 1)),
				new Vertex3(new Vector3( 1.0f, 0.0f, 1.0f )).SetUV(new Vector2(1, 0)),
				new Vertex3(new Vector3( 1.0f, 1.0f, 1.0f )).SetUV(new Vector2(1, 1)),
			}, new uint[] {
				0, 2, 1,
				1, 2, 3,
			}
		);
		this._faceBackward = VertexMeshUtilities.Mesh3.CreateNormalized( "faceBackward",
			new Vertex3[] {
				new Vertex3(new Vector3( 0.0f, 0.0f, 0.0f )).SetUV(new Vector2(0, 0)),
				new Vertex3(new Vector3( 0.0f, 1.0f, 0.0f )).SetUV(new Vector2(0, 1)),
				new Vertex3(new Vector3( 1.0f, 0.0f, 0.0f )).SetUV(new Vector2(1, 0)),
				new Vertex3(new Vector3( 1.0f, 1.0f, 0.0f )).SetUV(new Vector2(1, 1)),
			}, new uint[] {
				1, 2, 0,
				3, 2, 1,
			}
		);
	}

	protected override bool OnDispose() => true;
}
