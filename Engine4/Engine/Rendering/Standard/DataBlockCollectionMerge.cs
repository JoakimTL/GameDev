namespace Engine.Rendering.Standard;

public class DataBlockCollectionMerge : Identifiable, IDataBlockCollection {

	private readonly List<DataBlockCollection> _collections;

	public DataBlockCollectionMerge() {
		this._collections = new List<DataBlockCollection>();
	}

	public void AddMerger( DataBlockCollection collection ) => this._collections.Add( collection );

	public void Clear() => this._collections.Clear();

	public void DirectUnbindBuffers() {
		for ( int i = 0; i < this._collections.Count; i++ )
			this._collections[ i ].DirectUnbindBuffers();
	}

	public void DirectBindShader( ShaderPipeline s ) {
		for ( int i = 0; i < this._collections.Count; i++ )
			this._collections[ i ].DirectBindShader( s );
	}
}
