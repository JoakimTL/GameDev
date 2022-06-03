using Engine.Data;
using Engine.Data.Buffers;

namespace Engine.Rendering.Standard;
public abstract class BufferedMesh : DisposableIdentifiable, IMesh {

	private readonly RenderDataObject _rdo;
	private IDataSegment? _vertexDataSegment;
	private IDataSegment? _elementDataSegment;

	public IDataSegmentInformation VertexSegmentData => this._vertexDataSegment ?? throw new NullReferenceException( "Mesh is not complete!" );
	public IDataSegmentInformation ElementSegmentData => this._elementDataSegment ?? throw new NullReferenceException( "Mesh is not complete!" );

	public bool AutoSaved { get; set; }

	public BufferedMesh( string name, RenderDataObject rdo ) : base( name ) {
		this._rdo = rdo;
		this.AutoSaved = true;
		Resources.Render.BufferedMesh.Add( this );
	}

	protected void AllocateElements( uint numElements ) => AllocateElementSegment( numElements * sizeof( uint ) );
	protected void AllocateElementSegment( uint sizeBytes ) {
		if ( this._elementDataSegment is not null )
			this._elementDataSegment.Dispose();
		this._elementDataSegment = Resources.Render.RDOs.ElementBuffer.Buffer.AllocateSynchronized( sizeBytes );
	}

	protected void AllocateVertexSegment( uint sizeBytes ) {
		if ( this._vertexDataSegment is not null )
			this._vertexDataSegment.Dispose();
		this._vertexDataSegment = this._rdo.Buffer.AllocateSynchronized( sizeBytes );
	}

	public void SetVertexData<T>( T[] data, uint offset = 0 ) where T : unmanaged {
		if ( this._vertexDataSegment is null ) {
			this.LogWarning( "Can't set vertex data when buffer segment is null!" );
			return;
		}
		this._vertexDataSegment.Write<T>( offset, data );
	}

	public void SetElementData<T>( T[] data, uint offset = 0 ) where T : unmanaged {
		if ( this._elementDataSegment is null ) {
			this.LogWarning( "Can't set element data when buffer segment is null!" );
			return;
		}
		this._elementDataSegment.Write<T>( offset, data );
	}

	public byte[]? GetMeshData() {
		if ( this._vertexDataSegment is null || this._elementDataSegment is null )
			return null;
		return Segmentation.Segment( 
			this._vertexDataSegment.Read<byte>( 0, this._vertexDataSegment.SizeBytes ).ToArray(), 
			this._elementDataSegment.Read<byte>( 0, this._elementDataSegment.SizeBytes ).ToArray() 
		);
	}

	protected void LoadMeshData( byte[] segmentedData ) {
		byte[][]? data = Segmentation.Parse( segmentedData );
		if ( data is null || data.Length != 2 ) {
			this.LogError( "Corrupt data." );
			return;
		}
		AllocateVertexSegment( (uint) data[ 0 ].Length );
		AllocateElements( (uint) data[ 1 ].Length );
		SetVertexData( data[ 0 ] );
		SetElementData( data[ 1 ] );
	}

	public void Save() {
		byte[]? data = GetMeshData();
		if ( data is null )
			return;
		try {
			Directory.CreateDirectory( "meshes" );
			File.WriteAllBytes( $"meshes/{this.Name}.msh", data );
		} catch ( Exception e ) {
			this.LogError( e );
		}
	}

	protected override bool OnDispose() {
		if ( this.AutoSaved )
			Save();
		if ( this._vertexDataSegment is not null )
			this._vertexDataSegment.Dispose();
		if ( this._elementDataSegment is not null )
			this._elementDataSegment.Dispose();
		return true;
	}
}
