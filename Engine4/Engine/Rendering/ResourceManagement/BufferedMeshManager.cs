using Engine.Rendering.Standard;
using Engine.Rendering.Standard.Meshing;

namespace Engine.Rendering.ResourceManagement;

[Structure.ProcessBefore( typeof( Window ), typeof( IDisposable ) )]
public class BufferedMeshManager : DisposableIdentifiable {

	private readonly Dictionary<string, BufferedMesh> _meshes;

	public BufferedMeshManager() {
		this._meshes = new Dictionary<string, BufferedMesh>();
	}

	public void Add( BufferedMesh mesh ) {
		if ( this._meshes.ContainsKey( mesh.Name ) ) {
			this.LogWarning( $"Duplicate mesh, {mesh} will not be saved!" );
			return;
		}
		this._meshes.Add( mesh.Name, mesh );
	}

	public VertexMesh<T>? Get<T>( string filename ) where T : unmanaged {
		if ( this._meshes.TryGetValue( filename, out BufferedMesh? mesh ) ) {
			if ( mesh is VertexMesh<T> outMesh )
				return outMesh;
			this.LogLine( $"Requested mesh {filename} is of type {mesh.GetType().Name} not {typeof( VertexMesh<T> ).Name}", Log.Level.HIGH );
			return null;
		}

		byte[]? data;
		try {
			Directory.CreateDirectory( "meshes" );
			data = File.ReadAllBytes( $"meshes/{filename}.msh" );
		} catch ( Exception e ) {
			this.LogError( e );
			return null;
		}

		VertexMesh<T> newMesh = new( filename, data );
		this._meshes.Add( filename, newMesh );
		return newMesh;
	}

	protected override bool OnDispose() {
		foreach ( BufferedMesh mesh in this._meshes.Values ) {
			mesh.Dispose();
		}
		return true;
	}
}
