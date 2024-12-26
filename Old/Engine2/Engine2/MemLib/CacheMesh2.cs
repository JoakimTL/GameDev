using Engine.Graphics.Objects.Default.Meshes;
using Engine.Graphics.Objects.Default.Meshes.VertexMeshing;
using Engine.LMath;
using Engine.Utilities.Graphics.Utilities;

namespace Engine.MemLib {
	public class CacheMesh2 : Cache<string, Mesh2> {

		public Mesh2 Square { get; private set; }

		private readonly string baseDirectory, extensionName;

		public CacheMesh2( VaultReferences refVault, string baseDirectory, string extensionName ) : base( refVault, false ) {
			this.baseDirectory = baseDirectory;
			this.extensionName = extensionName;
		}

		protected override Mesh2 HandleNewObject( string key ) {
			return UnhandledNewObject();
			/*cf = CACHEFAULT.NONE;
			return Add( key, new Mesh2( key, $"{baseDirectory}\\{key}.{extensionName}" ) );*/
		}

		public Mesh2 Set( Mesh2 mesh ) {
			if( !( mesh is null ) )
				return Add( mesh.Name, mesh );
			return mesh;
		}

		public Mesh2 Set( string key, Vertex2[] vertices, int[] indices ) {
			return Set( new Mesh2( key, vertices, indices ) );
		}

		public void Initialize() {
			Square = new Mesh2( "square", 
				new Vertex2[] {
					new Vertex2().SetPosition(new Vector2(-1, -1)).SetUV(new Vector2(0, 0)),
					new Vertex2().SetPosition(new Vector2(1, -1)).SetUV( new Vector2(1, 0)),
					new Vertex2().SetPosition(new Vector2(1, 1)).SetUV(new Vector2(1, 1)),
					new Vertex2().SetPosition(new Vector2(-1, 1)).SetUV( new Vector2(0, 1))
				},
				new int[] {
					0, 1, 2, 2, 3, 0
				}
			);
			Add( Square.Name, Square );
			GLUtil.CheckError( "mesh2" );
		}

	}
}
