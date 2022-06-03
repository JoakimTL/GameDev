using OpenGL;

namespace Engine.Rendering.Standard.Scenes;

public class MultiSceneRenderer : Identifiable {

	private readonly List<SceneRenderer> _sceneRenderers;

	public MultiSceneRenderer() {
		this._sceneRenderers = new List<SceneRenderer>();
	}

	public void Add( SceneRenderer sceneRenderer ) => this._sceneRenderers.Add( sceneRenderer );

	public void Remove( SceneRenderer sceneRenderer ) => this._sceneRenderers.Remove( sceneRenderer );

	public void Render( DataBlockCollection? dataBlock = null, Action<bool>? blendActivationFunction = null, uint shaderUse = 0, PrimitiveType prim = PrimitiveType.Triangles ) {
		for ( int i = 0; i < this._sceneRenderers.Count; i++ )
			this._sceneRenderers[ i ].Render( dataBlock, blendActivationFunction, shaderUse, prim );
	}

}
