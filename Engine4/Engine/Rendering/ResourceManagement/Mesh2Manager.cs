using System.Numerics;
using Engine.Rendering.Colors;
using Engine.Rendering.Standard.Meshing;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;

namespace Engine.Rendering.ResourceManagement;
public class Mesh2Manager : Identifiable, IContextInitializable {

	private bool _initialized;
	private VertexMesh<Vertex2>? _square;
	private VertexMesh<Vector2>? _squarePfx;
	public VertexMesh<Vertex2> Square => this._square ?? throw new NullReferenceException( "Manager not initialized!" );
	public VertexMesh<Vector2> SquarePFX => this._squarePfx ?? throw new NullReferenceException( "Manager not initialized!" );

	public void InitializeInContext() {
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
		this._squarePfx = new VertexMesh<Vector2>( "squarePfx",
			new Vector2[] {
					new Vector2(-1, -1),
					new Vector2( 1, -1),
					new Vector2( 1,  1),
					new Vector2(-1,  1)
			},
			new uint[] { 0, 1, 2, 2, 3, 0 }
		);
	}
}
