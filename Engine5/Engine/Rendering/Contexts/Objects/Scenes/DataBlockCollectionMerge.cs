namespace Engine.Rendering.Contexts.Objects.Scenes;

public class DataBlockCollectionMerge : Identifiable, IDataBlockCollection {

	private readonly List<IDataBlockCollection> _collections;

	public DataBlockCollectionMerge() {
		_collections = new List<IDataBlockCollection>();
	}

	public void AddMerger( IDataBlockCollection collection ) => _collections.Add( collection );

	public void Clear() => _collections.Clear();

	public void UnbindBuffers() {
		for ( int i = 0; i < _collections.Count; i++ )
			_collections[ i ].UnbindBuffers();
	}

	public void BindShader( ShaderPipelineBase s ) {
		for ( int i = 0; i < _collections.Count; i++ )
			_collections[ i ].BindShader( s );
	}
}
