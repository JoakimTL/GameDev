using OpenGL;

namespace Engine.Rendering.Contexts.Objects.Scenes;

public class SceneRenderer : Identifiable, ISceneRender {

	public ISceneRender Scene { get; }
	public IDataBlockCollection? DataBlock { get; }
	public Action<bool>? BlendActivationFunction { get; }
	private readonly DataBlockCollectionMerge _merger;

	public SceneRenderer( ISceneRender scene, IDataBlockCollection? dataBlock = null, Action<bool>? blendActivationFunction = null ) {
		Scene = scene;
		DataBlock = dataBlock;
		BlendActivationFunction = blendActivationFunction;
		_merger = new DataBlockCollectionMerge();
	}

	public void Render( string shaderIndex, IDataBlockCollection? dataBlock = null, Action<bool>? blendActivationFunction = null, PrimitiveType prim = PrimitiveType.Triangles ) {
		_merger.Clear();
		if ( DataBlock is not null )
			_merger.AddMerger( DataBlock );
		if ( dataBlock is not null )
			_merger.AddMerger( dataBlock );
		Scene.Render( shaderIndex, _merger, blendActivationFunction ?? BlendActivationFunction, prim );
	}
}
