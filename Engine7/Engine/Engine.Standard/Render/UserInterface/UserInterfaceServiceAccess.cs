using Engine.Module.Render;
using Engine.Module.Render.Entities;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Providers;
using Engine.Module.Render.Ogl.Scenes;

namespace Engine.Standard.Render.UserInterface;

public sealed class UserInterfaceServiceAccess( RenderServiceAccess renderServiceAccess, string userInterfaceSceneName ) {
	private readonly string _userInterfaceSceneName = userInterfaceSceneName;
	private readonly SceneInstanceProvider _sceneInstanceProvider = renderServiceAccess.Get<SceneInstanceProvider>();

	public CompositeVertexArrayProvider CompositeVertexArrayProvider { get; } = renderServiceAccess.Get<CompositeVertexArrayProvider>();
	public ShaderBundleProvider ShaderBundleProvider { get; } = renderServiceAccess.Get<ShaderBundleProvider>();
	public MeshProvider MeshProvider { get; } = renderServiceAccess.Get<MeshProvider>();
	public T Get<T>() where T : IRenderServiceProvider => renderServiceAccess.Get<T>();

	public T RequestSceneInstance<T>( uint layer ) where T : SceneInstanceBase, new() 
		=> _sceneInstanceProvider.RequestSceneInstance<T>( _userInterfaceSceneName, layer );

	public SceneInstanceCollection<TVertexData, TInstanceData> RequestSceneInstanceCollection<TVertexData, TInstanceData, TShaderBundle>( uint layer )
		where TVertexData : unmanaged
		where TInstanceData : unmanaged
		where TShaderBundle : ShaderBundleBase 
		=> _sceneInstanceProvider.RequestSceneInstanceCollection<TVertexData, TInstanceData, TShaderBundle>( _userInterfaceSceneName, layer );
}
