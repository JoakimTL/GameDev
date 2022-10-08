using System.Numerics;
using Engine.Rendering.Colors;
using Engine.Rendering.Standard.Meshing;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;

namespace Engine.Rendering.Services;
public class Mesh2Manager : ModuleService, IInitializable {

	private bool _initialized;
	private VertexMesh<Vertex2>? _square;
	private VertexMesh<Vertex2>? _unitSquare;
	private VertexMesh<Vector2>? _squarePfx;
	private VertexMesh<Vector2>? _unitSquarePfx;
	public VertexMesh<Vertex2> Square => this._square ?? throw new NullReferenceException( "Manager not initialized!" );
	public VertexMesh<Vertex2> UnitSquare => this._unitSquare ?? throw new NullReferenceException( "Manager not initialized!" );
	public VertexMesh<Vector2> SquarePFX => this._squarePfx ?? throw new NullReferenceException( "Manager not initialized!" );
	public VertexMesh<Vector2> UnitSquarePFX => this._unitSquarePfx ?? throw new NullReferenceException( "Manager not initialized!" );

	public void Initialize() {
		if ( this._initialized ) {
			this.LogError( "Already initialized!" );
			return;
		}
		this._initialized = true;

		this._square = new VertexMesh<Vertex2>( "square",
			new Vertex2[] {
					new Vertex2( new Vector2(-1, -1), new Vector2( 0, 0 ), Color16x4.White ),
					new Vertex2( new Vector2( 1, -1), new Vector2( 1, 0 ), Color16x4.White ),
					new Vertex2( new Vector2( 1,  1), new Vector2( 1, 1 ), Color16x4.White ),
					new Vertex2( new Vector2(-1,  1), new Vector2( 0, 1 ), Color16x4.White )
			},
			new uint[] { 0, 1, 2, 2, 3, 0 }
		);
		this._unitSquare = new VertexMesh<Vertex2>( "unitsquare",
			new Vertex2[] {
					new Vertex2( new Vector2(0, 0), new Vector2( 0, 0 ), Color16x4.White ),
					new Vertex2( new Vector2(1, 0), new Vector2( 1, 0 ), Color16x4.White ),
					new Vertex2( new Vector2(1, 1), new Vector2( 1, 1 ), Color16x4.White ),
					new Vertex2( new Vector2(0, 1), new Vector2( 0, 1 ), Color16x4.White )
			},
			new uint[] { 0, 1, 2, 2, 3, 0 }
		);
		this._squarePfx = new VertexMesh<Vector2>( "squarePfx",
			new Vector2[] {
					new Vector2(-1, -1),
					new Vector2( 1, -1),
					new Vector2( 1,  1),
					new Vector2(-1,  1)
			},
			new uint[] { 0, 1, 2, 2, 3, 0 }
		);
		this._unitSquarePfx = new VertexMesh<Vector2>( "unitSquarePfx",
			new Vector2[] {
					new Vector2(0, 0),
					new Vector2(1, 0),
					new Vector2(1, 1),
					new Vector2(0, 1)
			},
			new uint[] { 0, 1, 2, 2, 3, 0 }
		);
	}

	protected override bool OnDispose() => true;
}
