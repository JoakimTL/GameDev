using Engine.Graphics.Objects.Default.Meshes;
using Engine.Graphics.Objects.Default.Meshes.VertexMeshing;
using Engine.LinearAlgebra;
using Engine.Utilities.Graphics.Utilities;
using Engine.Utilities.Tools;

namespace Engine.MemLib {
	public class CacheMesh3 : Cache<string, Mesh3> {

		public Mesh3 Icosphere { get; private set; }
		public Mesh3 Cube { get; private set; }
		public Mesh3 BoxY { get; private set; }
		public Mesh3 BoxZ { get; private set; }

		private readonly string baseDirectory, extensionName;

		public CacheMesh3( VaultReferences refVault, string baseDirectory, string extensionName ) : base( refVault, true ) {
			this.baseDirectory = baseDirectory;
			this.extensionName = extensionName;
		}

		protected override Mesh3 HandleNewObject( string key ) {
			Mesh3 m = new Mesh3( key, $"{baseDirectory}\\{key}.{extensionName}" );
			Add( key, m );
			return m;
		}

		public Mesh3 Set( Mesh3 mesh ) {
			if( !( mesh is null ) )
				return Add( mesh.Name, mesh );
			return mesh;
		}

		public Mesh3 Set( string key, Vertex3[] vertices, int[] indices ) {
			return Set( new Mesh3( key, vertices, indices ) );
		}

		public void Initialize() {
			Cube = new Mesh3( "cube", 
				new Vertex3[] {
					new Vertex3((-1.0f, -1.0f, -1.0f)).SetUV(new Vector2(0, 0)),
					new Vertex3((-1.0f,  1.0f, -1.0f)).SetUV(new Vector2(0, 1)),
					new Vertex3(( 1.0f, -1.0f, -1.0f)).SetUV(new Vector2(1, 0)),

					new Vertex3(( 1.0f,  1.0f, -1.0f)).SetUV(new Vector2(1, 1)),
					new Vertex3(( 1.0f, -1.0f, -1.0f)).SetUV(new Vector2(1, 0)),
					new Vertex3((-1.0f,  1.0f, -1.0f)).SetUV(new Vector2(0, 1)),

					new Vertex3((-1.0f, -1.0f,  1.0f)).SetUV(new Vector2(0, 0)),
					new Vertex3(( 1.0f, -1.0f,  1.0f)).SetUV(new Vector2(1, 0)),
					new Vertex3((-1.0f,  1.0f,  1.0f)).SetUV(new Vector2(0, 1)),

					new Vertex3(( 1.0f,  1.0f,  1.0f)).SetUV(new Vector2(1, 1)),
					new Vertex3((-1.0f,  1.0f,  1.0f)).SetUV(new Vector2(0, 1)),
					new Vertex3(( 1.0f, -1.0f,  1.0f)).SetUV(new Vector2(1, 0)),

					new Vertex3((-1.0f, -1.0f, -1.0f)).SetUV(new Vector2(0, 0)),
					new Vertex3(( 1.0f, -1.0f, -1.0f)).SetUV(new Vector2(1, 0)),
					new Vertex3((-1.0f, -1.0f,  1.0f)).SetUV(new Vector2(0, 1)),

					new Vertex3(( 1.0f, -1.0f,  1.0f)).SetUV(new Vector2(1, 1)),
					new Vertex3((-1.0f, -1.0f,  1.0f)).SetUV(new Vector2(0, 1)),
					new Vertex3(( 1.0f, -1.0f, -1.0f)).SetUV(new Vector2(1, 0)),

					new Vertex3((-1.0f,  1.0f, -1.0f)).SetUV(new Vector2(0, 0)),
					new Vertex3((-1.0f,  1.0f,  1.0f)).SetUV(new Vector2(0, 1)),
					new Vertex3(( 1.0f,  1.0f, -1.0f)).SetUV(new Vector2(1, 0)),

					new Vertex3(( 1.0f,  1.0f,  1.0f)).SetUV(new Vector2(1, 1)),
					new Vertex3(( 1.0f,  1.0f, -1.0f)).SetUV(new Vector2(1, 0)),
					new Vertex3((-1.0f,  1.0f,  1.0f)).SetUV(new Vector2(0, 1)),

					new Vertex3((-1.0f, -1.0f, -1.0f)).SetUV(new Vector2(0, 0)),
					new Vertex3((-1.0f, -1.0f,  1.0f)).SetUV(new Vector2(0, 1)),
					new Vertex3((-1.0f,  1.0f, -1.0f)).SetUV(new Vector2(1, 0)),

					new Vertex3((-1.0f,  1.0f,  1.0f)).SetUV(new Vector2(1, 1)),
					new Vertex3((-1.0f,  1.0f, -1.0f)).SetUV(new Vector2(1, 0)),
					new Vertex3((-1.0f, -1.0f,  1.0f)).SetUV(new Vector2(0, 1)),

					new Vertex3(( 1.0f, -1.0f, -1.0f)).SetUV(new Vector2(0, 0)),
					new Vertex3(( 1.0f,  1.0f, -1.0f)).SetUV(new Vector2(1, 0)),
					new Vertex3(( 1.0f, -1.0f,  1.0f)).SetUV(new Vector2(0, 1)),

					new Vertex3(( 1.0f,  1.0f,  1.0f)).SetUV(new Vector2(1, 1)),
					new Vertex3(( 1.0f, -1.0f,  1.0f)).SetUV(new Vector2(0, 1)),
					new Vertex3(( 1.0f,  1.0f, -1.0f)).SetUV(new Vector2(1, 0)),
				}, new int[] {
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

			BoxY = new Mesh3( "boxY",
				new Vertex3[] {
					new Vertex3((-1.0f,0.0f,-1.0f)),
					new Vertex3((-1.0f,0.0f, 1.0f)),
					new Vertex3((-1.0f, 1.0f, 1.0f)),
					new Vertex3((1.0f, 1.0f,-1.0f)),
					new Vertex3((-1.0f,0.0f,-1.0f)),
					new Vertex3((-1.0f, 1.0f,-1.0f)),
					new Vertex3((1.0f,0.0f, 1.0f)),
					new Vertex3((-1.0f,0.0f,-1.0f)),
					new Vertex3((1.0f,0.0f,-1.0f)),
					new Vertex3((1.0f, 1.0f,-1.0f)),
					new Vertex3((1.0f,0.0f,-1.0f)),
					new Vertex3((-1.0f,0.0f,-1.0f)),
					new Vertex3((-1.0f,0.0f,-1.0f)),
					new Vertex3((-1.0f, 1.0f, 1.0f)),
					new Vertex3((-1.0f, 1.0f,-1.0f)),
					new Vertex3((1.0f,0.0f, 1.0f)),
					new Vertex3((-1.0f,0.0f, 1.0f)),
					new Vertex3((-1.0f,0.0f,-1.0f)),
					new Vertex3((-1.0f, 1.0f, 1.0f)),
					new Vertex3((-1.0f,0.0f, 1.0f)),
					new Vertex3((1.0f,0.0f, 1.0f)),
					new Vertex3((1.0f, 1.0f, 1.0f)),
					new Vertex3((1.0f,0.0f,-1.0f)),
					new Vertex3((1.0f, 1.0f,-1.0f)),
					new Vertex3((1.0f,0.0f,-1.0f)),
					new Vertex3((1.0f, 1.0f, 1.0f)),
					new Vertex3((1.0f,0.0f, 1.0f)),
					new Vertex3((1.0f, 1.0f, 1.0f)),
					new Vertex3((1.0f, 1.0f,-1.0f)),
					new Vertex3((-1.0f, 1.0f,-1.0f)),
					new Vertex3((1.0f, 1.0f, 1.0f)),
					new Vertex3((-1.0f, 1.0f,-1.0f)),
					new Vertex3((-1.0f, 1.0f, 1.0f)),
					new Vertex3((1.0f, 1.0f, 1.0f)),
					new Vertex3((-1.0f, 1.0f, 1.0f)),
					new Vertex3((1.0f,0.0f, 1.0f))
				}, new int[] {
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

			BoxZ = new Mesh3( "boxZ",
				new Vertex3[] {
					new Vertex3((-1.0f,-1.0f,-1.0f)),
					new Vertex3((-1.0f,-1.0f, 0.0f)),
					new Vertex3((-1.0f, 1.0f, 0.0f)),
					new Vertex3(( 1.0f, 1.0f,-1.0f)),
					new Vertex3((-1.0f,-1.0f,-1.0f)),
					new Vertex3((-1.0f, 1.0f,-1.0f)),
					new Vertex3(( 1.0f,-1.0f, 0.0f)),
					new Vertex3((-1.0f,-1.0f,-1.0f)),
					new Vertex3(( 1.0f,-1.0f,-1.0f)),
					new Vertex3(( 1.0f, 1.0f,-1.0f)),
					new Vertex3(( 1.0f,-1.0f,-1.0f)),
					new Vertex3((-1.0f,-1.0f,-1.0f)),
					new Vertex3((-1.0f,-1.0f,-1.0f)),
					new Vertex3((-1.0f, 1.0f, 0.0f)),
					new Vertex3((-1.0f, 1.0f,-1.0f)),
					new Vertex3(( 1.0f,-1.0f, 0.0f)),
					new Vertex3((-1.0f,-1.0f, 0.0f)),
					new Vertex3((-1.0f,-1.0f,-1.0f)),
					new Vertex3((-1.0f, 1.0f, 0.0f)),
					new Vertex3((-1.0f,-1.0f, 0.0f)),
					new Vertex3(( 1.0f,-1.0f, 0.0f)),
					new Vertex3(( 1.0f, 1.0f, 0.0f)),
					new Vertex3(( 1.0f,-1.0f,-1.0f)),
					new Vertex3(( 1.0f, 1.0f,-1.0f)),
					new Vertex3(( 1.0f,-1.0f,-1.0f)),
					new Vertex3(( 1.0f, 1.0f, 0.0f)),
					new Vertex3(( 1.0f,-1.0f, 0.0f)),
					new Vertex3(( 1.0f, 1.0f, 0.0f)),
					new Vertex3(( 1.0f, 1.0f,-1.0f)),
					new Vertex3((-1.0f, 1.0f,-1.0f)),
					new Vertex3(( 1.0f, 1.0f, 0.0f)),
					new Vertex3((-1.0f, 1.0f,-1.0f)),
					new Vertex3((-1.0f, 1.0f, 0.0f)),
					new Vertex3(( 1.0f, 1.0f, 0.0f)),
					new Vertex3((-1.0f, 1.0f, 0.0f)),
					new Vertex3(( 1.0f,-1.0f, 0.0f))
				}, new int[] {
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

			Icosphere = MeshGenerator.GenerateIcosphere( 3, false );

			Add( Cube.Name, Cube );
			Add( BoxY.Name, BoxY );
			Add( BoxZ.Name, BoxZ );
			Add( Icosphere.Name, Icosphere );
		}

	}
}
