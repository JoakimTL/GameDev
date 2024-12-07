using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Module.Entities.Render;

public sealed class SceneInstanceProvider( SceneService sceneService ) {
	private readonly SceneService _sceneService = sceneService;
	internal T RequestSceneInstance<T>( string sceneName, uint layer ) where T : SceneInstanceBase, new() => this._sceneService.GetScene( sceneName ).CreateInstance<T>( layer, true );
}
