namespace Engine.Rendering.Contexts.Objects.Scenes;

public class DataBlockCollectionMerge : Identifiable, IDataBlockCollection {

	private readonly List<IDataBlockCollection> _collections;

	public DataBlockCollectionMerge() {
		this._collections = new List<IDataBlockCollection>();
	}

	public void AddMerger( IDataBlockCollection collection ) => this._collections.Add( collection );

	public void Clear() => this._collections.Clear();

	public void DirectUnbindBuffers() {
		for ( int i = 0; i < this._collections.Count; i++ )
			this._collections[ i ].DirectUnbindBuffers();
	}

	public void DirectBindShader( ShaderPipelineBase s ) {
		for ( int i = 0; i < this._collections.Count; i++ )
			this._collections[ i ].DirectBindShader( s );
	}
}
