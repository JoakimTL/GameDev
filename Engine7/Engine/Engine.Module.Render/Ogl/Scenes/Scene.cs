using Engine;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Services;
using Engine.Structures;
using OpenGL;
using System.Runtime.CompilerServices;

namespace Engine.Module.Render.Ogl.Scenes;

public sealed class Scene : DisposableIdentifiable {

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


	public T CreateInstance<T>( uint renderLayer = 0 ) where T : SceneInstanceBase, new() {
		T instance = new();
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

	public void Render( string shaderIndex, IDataBlockCollection? dataBlocks, PrimitiveType primitiveType ) {
		if (_sceneRender is null)
			_sceneRender = new();
		if (_needsUpdate)
			_sceneRender.PrepareForRender( _sortedLayersReadOnly );
		_needsUpdate = false;
		_sceneRender.Render( shaderIndex, dataBlocks, primitiveType );
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

public sealed class SceneRender : DisposableIdentifiable {
	private readonly OOP.Buffers.OglDynamicBuffer _commandBuffer;
	private readonly List<IndirectCommand> _indirectCommands;
	private readonly List<RenderStage> _stages;

	public SceneRender() {
		_commandBuffer = new( BufferUsage.DynamicDraw, (uint) Unsafe.SizeOf<IndirectCommand>() * 4096 );
		_indirectCommands = [];
		_stages = [];
	}

	public void PrepareForRender( IReadOnlyList<SceneLayer> layers ) {
		_indirectCommands.Clear();
		_stages.Clear();
		foreach (SceneLayer layer in layers)
			foreach (SceneObject sceneObject in layer.SceneObjects) {
				uint countBeforeAddition = (uint) _indirectCommands.Count;
				sceneObject.AddIndirectCommands( _indirectCommands );
				_stages.Add( new( sceneObject.VertexArrayObject, sceneObject.ShaderBundle, countBeforeAddition, _indirectCommands.Count - (int) countBeforeAddition ) );
			}
		unsafe {
			Span<IndirectCommand> commands = stackalloc IndirectCommand[ _indirectCommands.Count ];
			_indirectCommands.CopyTo( commands );
			fixed (IndirectCommand* srcPtr = commands) {
				_commandBuffer.WriteRange( srcPtr, (uint) (commands.Length * sizeof( IndirectCommand )), 0 );
			}
		}
	}

	internal void Render( string shaderIndex, IDataBlockCollection? dataBlocks, PrimitiveType primitiveType ) {
		Gl.BindBuffer( BufferTarget.DrawIndirectBuffer, _commandBuffer.BufferId );
		foreach (RenderStage stage in _stages) {
			OglShaderPipelineBase? shader = stage.ShaderBundle.Get( shaderIndex );
			if (shader is null)
				continue;
			shader.Bind();
			stage.VertexArrayObject.Bind();
			dataBlocks?.BindShader( shader );
			Gl.MultiDrawElementsIndirect( primitiveType, DrawElementsType.UnsignedInt, new nint( stage.StageCommandStart * Unsafe.SizeOf<IndirectCommand>()), stage.StageCommandCount, 0  );
			dataBlocks?.UnbindBuffers();
		}
		OglShaderPipelineBase.Unbind();
		OglVertexArrayObjectBase.Unbind();
	}

	protected override bool InternalDispose() {
		_commandBuffer.Dispose();
		return true;
	}
}

public sealed class RenderStage {
	public OglVertexArrayObjectBase VertexArrayObject { get; }
	public ShaderBundleBase ShaderBundle { get; }
	public uint StageCommandStart { get; }
	public int StageCommandCount { get; }

	public RenderStage( OglVertexArrayObjectBase vertexArrayObject, ShaderBundleBase shaderBundle, uint stageCommandStart, int stageCommandCount ) {
		this.VertexArrayObject = vertexArrayObject;
		this.ShaderBundle = shaderBundle;
		this.StageCommandStart = stageCommandStart;
		this.StageCommandCount = stageCommandCount;
	}
}

/// <summary>
/// Represents a collection of identical instances in a scene.
/// </summary>
public interface ISceneObjectCollection {
	bool TryGetIndirectCommand( out IndirectCommand command );
}


public interface ISceneRender {
	public void Render( string shaderIndex, IDataBlockCollection? dataBlocks, Action<bool>? blendActivationFunction, PrimitiveType prim = PrimitiveType.Triangles );
}

public interface IDataBlockCollection {
	void BindShader( OglShaderPipelineBase s );
	void UnbindBuffers();
}
