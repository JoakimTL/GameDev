using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Services;
using Engine.Structures;
using OpenGL;

namespace Engine.Module.Render.Ogl.Scenes;

public sealed class Scene : DisposableIdentifiable, ISceneRender {

	private readonly BufferService _bufferService;

	private readonly Dictionary<uint, SceneLayer> _sceneLayersByLayer;
	private readonly SimpleSortedList<SceneLayer> _sortedLayers;
	private readonly IReadOnlyList<SceneLayer> _sortedLayersReadOnly;
	private SceneRender? _sceneRender = null;
	private bool _needsUpdate;

	public string SceneName { get; }

	public Scene( string name, BufferService bufferService ) {
		this._bufferService = bufferService;
		this.SceneName = name;
		this._sceneLayersByLayer = [];
		this._sortedLayers = new();
		this._sortedLayersReadOnly = this._sortedLayers.AsReadOnly();
	}

	public T CreateInstance<T>( uint renderLayer = 0, bool overrideSetupLayer = true ) where T : SceneInstanceBase, new() {
		T instance = new();
		instance.Setup();
		if (overrideSetupLayer || instance.RenderLayer == 0)
			instance.SetLayer( renderLayer );
		GetLayer( renderLayer ).AddSceneInstance( instance );
		return instance;
	}

	public SceneInstanceCollection<TVertexData, TInstanceData> CreateInstanceCollection<TVertexData, TInstanceData>( uint layer, OglVertexArrayObjectBase vertexArrayObject, ShaderBundleBase shaderBundle )
		where TVertexData : unmanaged
		where TInstanceData : unmanaged
		=> new( this, layer, vertexArrayObject, shaderBundle );


	public SceneObjectFixedCollection<TVertexData, TInstanceData> CreateFixedCollection<TVertexData, TInstanceData>( uint renderLayer, OglVertexArrayObjectBase vao, ShaderBundleBase shaderBundle, IMesh mesh, uint count )
		where TVertexData : unmanaged
		where TInstanceData : unmanaged {
		return GetLayer( renderLayer ).CreateFixedCollection<TVertexData, TInstanceData>( vao, shaderBundle, mesh, count );
	}

	private SceneLayer GetLayer( uint renderLayer ) {
		if (!this._sceneLayersByLayer.TryGetValue( renderLayer, out SceneLayer? layer )) {
			this._sceneLayersByLayer.Add( renderLayer, layer = new( renderLayer, this._bufferService ) );
			this._sortedLayers.Add( layer );
			layer.OnChanged += OnLayerChanged;
			layer.OnInstanceRemoved += OnLayerInstanceRemoved;
		}
		return layer;
	}

	private void OnLayerInstanceRemoved( SceneInstanceBase instance ) {
		if (!instance.Removed)
			GetLayer( instance.RenderLayer ).AddSceneInstance( instance );
		this._needsUpdate = true;
	}

	private void OnLayerChanged() {
		this._needsUpdate = true;
	}

	public void Render( string shaderIndex, IDataBlockCollection? dataBlocks, Action<bool>? blendActivationFunction, PrimitiveType primitiveType ) {
		this._sceneRender ??= new();
		if (this._needsUpdate)
			this._sceneRender.PrepareForRender( this._sortedLayersReadOnly );
		this._needsUpdate = false;
		this._sceneRender.Render( shaderIndex, dataBlocks, blendActivationFunction, primitiveType );
	}

	protected override bool InternalDispose() {
		foreach (SceneLayer layer in this._sortedLayersReadOnly)
			layer.Dispose();
		this._sortedLayers.Clear();
		this._sceneLayersByLayer.Clear();
		this._sceneRender?.Dispose();
		return true;
	}
}
