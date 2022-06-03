namespace Engine.Rendering.Standard.Meshing;
public class VertexMesh<V> : BufferedMesh where V : unmanaged {
	private VertexMesh( string name ) : base( name, Resources.Render.RDOs.Get<V>() ) { }

	public VertexMesh( string name, uint vertexCount, uint elementCount ) : this( name ) {
		AllocateForVertices( vertexCount );
		AllocateElements( elementCount );
	}

	public VertexMesh( string name, V[] vertices, uint[] indices ) : this( name, (uint) vertices.Length, (uint) indices.Length ) {
		SetVertexData( vertices );
		SetElementData( indices );
	}

	public VertexMesh( string name, byte[] data ) : this( name ) {
		LoadMeshData( data );
	}

	protected void AllocateForVertices( uint vertexCount ) {
		unsafe {
			AllocateVertexSegment( vertexCount * (uint) sizeof( V ) );
		}
	}
}
