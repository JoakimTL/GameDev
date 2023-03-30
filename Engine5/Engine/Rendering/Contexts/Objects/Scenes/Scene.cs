using Engine.Structure.Interfaces;
using OpenGL;

namespace Engine.Rendering.Contexts.Objects.Scenes;

public abstract class Scene : Identifiable, ISceneRender, IDisposable {

	private HashSet<ISceneObject> _sceneObjects;
	private Dictionary<string, SortedScene> _sortedSceneByShaderIndex;

	public bool Populated => _sceneObjects.Count > 0;

	public event Action<ISceneObject>? SceneObjectAdded;
	public event Action<ISceneObject>? SceneObjectRemoved;

	//Use the shaderbudleservice AllIndices?
	public Scene( IEnumerable<string> shaderIndices ) {
		_sceneObjects = new();
		_sortedSceneByShaderIndex = new();
		foreach ( var index in shaderIndices )
			if ( !_sortedSceneByShaderIndex.ContainsKey( index ) )
				_sortedSceneByShaderIndex.Add( index, new( this, index, 4096 ) );
	}

	public void Add( ISceneObject so ) {
		if ( _sceneObjects.Add( so ) ) {
			so.SceneObjectDisposed += Remove;
			SceneObjectAdded?.Invoke( so );
		}
	}

	public void Remove( ISceneObject so ) {
		if ( _sceneObjects.Remove( so ) ) {
			so.SceneObjectDisposed -= Remove;
			SceneObjectRemoved?.Invoke( so );
		}
	}

	public abstract int SortMethod( SceneObjectUsage x, SceneObjectUsage y );

	//ISceneObject no longer has "HasTransparency", cause this should be handled on a per shaderpipeline basis rather than shaderbundle level. This adds some complexity to the "sorting" of the sceneobjects. 
	public void Render( string shaderIndex, IDataBlockCollection? dataBlocks, Action<bool>? blendActivationFunction, PrimitiveType prim = PrimitiveType.Triangles ) {
		if ( _sortedSceneByShaderIndex.TryGetValue( shaderIndex, out SortedScene? sortedScene ) )
			sortedScene.Render( dataBlocks, blendActivationFunction, prim );
	}

	public void Dispose()
	{
		foreach (var sortedScene in _sortedSceneByShaderIndex.Values)
			sortedScene.Dispose();
		_sortedSceneByShaderIndex.Clear();
		foreach (var so in _sceneObjects)
			so.SceneObjectDisposed -= Remove;
		_sceneObjects.Clear();
	}
}
