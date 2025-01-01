using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Module.Render.Ogl.Providers;

public sealed class SceneInstanceProvider( SceneService sceneService, CompositeVertexArrayProvider compositeVertexArrayProvider, ShaderBundleProvider shaderBundleProvider ) : IRenderServiceProvider {
	private readonly SceneService _sceneService = sceneService;
	private readonly CompositeVertexArrayProvider _compositeVertexArrayProvider = compositeVertexArrayProvider;
	private readonly ShaderBundleProvider _shaderBundleProvider = shaderBundleProvider;

	public T RequestSceneInstance<T>( string sceneName, uint layer ) where T : SceneInstanceBase, new()
		=> this._sceneService.GetScene( sceneName ).CreateInstance<T>( layer, true );

	public SceneInstanceCollection<TVertexData, TInstanceData> RequestSceneInstanceCollection<TVertexData, TInstanceData, TShaderBundle>( string sceneName, uint layer )
		where TVertexData : unmanaged
		where TInstanceData : unmanaged
		where TShaderBundle : ShaderBundleBase {
		OglVertexArrayObjectBase vao = _compositeVertexArrayProvider.GetVertexArray<TVertexData, TInstanceData>() ?? throw new InvalidOperationException( "CompositeVertexArrayObject not found." );
		TShaderBundle shaderBundle = _shaderBundleProvider.GetShaderBundle<TShaderBundle>() ?? throw new InvalidOperationException( "ShaderBundle not found." );
		return this._sceneService.GetScene( sceneName ).CreateInstanceCollection<TVertexData, TInstanceData>( layer, vao, shaderBundle );
	}
}
