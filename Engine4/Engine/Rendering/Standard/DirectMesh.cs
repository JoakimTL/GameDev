namespace Engine.Rendering.Standard;
public class DirectMesh : Identifiable, IMesh {

	private readonly VertexBufferObject _vertexDataBuffer;
	private readonly VertexBufferObject _elementDataBuffer;

	public DirectMesh( string name, VertexBufferObject vertexData, VertexBufferObject elementData ) : base( name ) {
		this._vertexDataBuffer = vertexData;
		this._elementDataBuffer = elementData;
	}

	public void SetElementData<T>( T[] data, uint offset = 0 ) where T : unmanaged {
		unsafe {
			fixed ( T* dataPtr = data )
				this._vertexDataBuffer.DirectWrite( new IntPtr( dataPtr ), offset, 0, (uint) ( data.Length * sizeof( T ) ) );
		}
	}

	public void SetVertexData<T>( T[] data, uint offset = 0 ) where T : unmanaged {
		unsafe {
			fixed ( T* dataPtr = data )
				this._elementDataBuffer.DirectWrite( new IntPtr( dataPtr ), offset, 0, (uint) ( data.Length * sizeof( T ) ) );
		}
	}
}
