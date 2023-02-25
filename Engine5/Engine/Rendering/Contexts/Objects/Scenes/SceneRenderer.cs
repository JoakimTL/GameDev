using OpenGL;

namespace Engine.Rendering.Contexts.Objects.Scenes;

public class SceneRenderer : Identifiable, ISceneRender {

	public ISceneRender Scene { get; }
	public IDataBlockCollection? DataBlock { get; }
	public Action<bool>? BlendActivationFunction { get; }
	private readonly DataBlockCollectionMerge _merger;

	public SceneRenderer( ISceneRender scene, IDataBlockCollection? dataBlock = null, Action<bool>? blendActivationFunction = null ) {
		this.Scene = scene;
		this.DataBlock = dataBlock;
		this.BlendActivationFunction = blendActivationFunction;
		this._merger = new DataBlockCollectionMerge();
	}

	public void Render( IDataBlockCollection? dataBlock = null, Action<bool>? blendActivationFunction = null, uint shaderUse = 0, PrimitiveType prim = PrimitiveType.Triangles ) {
		this._merger.Clear();
		if ( this.DataBlock is not null )
			this._merger.AddMerger( this.DataBlock );
		if ( dataBlock is not null )
			this._merger.AddMerger( dataBlock );
		this.Scene.Render( this._merger, blendActivationFunction ?? this.BlendActivationFunction, shaderUse, prim );
	}
}
