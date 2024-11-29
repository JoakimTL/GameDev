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
		_sceneLayersByLayer = [];
		_sortedLayers = new();
		_sortedLayersReadOnly = _sortedLayers.AsReadOnly();
	}


	public T CreateInstance<T>( uint renderLayer = 0, bool overrideSetupLayer = true ) where T : SceneInstanceBase, new() {
		T instance = new();
		instance.Setup();
		if (overrideSetupLayer || instance.RenderLayer == 0)
			instance.SetLayer( renderLayer );
		if (!_sceneLayersByLayer.TryGetValue( renderLayer, out SceneLayer? layer )) {
			_sceneLayersByLayer.Add( renderLayer, layer = new( renderLayer, _bufferService ) );
			_sortedLayers.Add( layer );
			layer.OnChanged += OnLayerChanged;
		}
		layer.AddSceneInstance( instance );
		return instance;
	}

	private void OnLayerChanged() => _needsUpdate = true;

	public void Render( string shaderIndex, IDataBlockCollection? dataBlocks, Action<bool>? blendActivationFunction, PrimitiveType primitiveType ) {
		if (_sceneRender is null)
			_sceneRender = new();
		if (_needsUpdate)
			_sceneRender.PrepareForRender( _sortedLayersReadOnly );
		_needsUpdate = false;
		_sceneRender.Render( shaderIndex, dataBlocks, blendActivationFunction, primitiveType );
	}

	protected override bool InternalDispose() {

		foreach (SceneLayer layer in _sortedLayersReadOnly)
			layer.Dispose();
		_sortedLayers.Clear();
		_sceneLayersByLayer.Clear();
		_sceneRender?.Dispose();
		return true;
	}
}
