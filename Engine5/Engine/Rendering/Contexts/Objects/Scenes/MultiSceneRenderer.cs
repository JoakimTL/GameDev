using OpenGL;

namespace Engine.Rendering.Contexts.Objects.Scenes;

public class MultiSceneRenderer : Identifiable, ISceneRender {

	private readonly List<ISceneRender> _sceneRenderers;

	public MultiSceneRenderer() {
		_sceneRenderers = new();
	}

	public void Add( ISceneRender sceneRenderer ) => _sceneRenderers.Add( sceneRenderer );

	public void Remove( ISceneRender sceneRenderer ) => _sceneRenderers.Remove( sceneRenderer );

	public void Render( string shaderIndex, IDataBlockCollection? dataBlock = null, Action<bool>? blendActivationFunction = null, PrimitiveType prim = PrimitiveType.Triangles ) {
		for ( int i = 0; i < _sceneRenderers.Count; i++ )
			_sceneRenderers[ i ].Render( shaderIndex, dataBlock, blendActivationFunction, prim );
	}

}
