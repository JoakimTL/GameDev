using Engine.Module.Render.Ogl.Services;

namespace Engine.Module.Render.Ogl.Scenes;

public sealed class SceneService( BufferService bufferService ) {
	private readonly Dictionary<string, Scene> _scenesByName = [];
	private readonly BufferService _bufferService = bufferService;

	public Scene GetScene( string sceneName ) {
		if (!_scenesByName.TryGetValue( sceneName, out Scene? scene ))
			_scenesByName.Add( sceneName, scene = new( sceneName, _bufferService ) );
		return scene;
	}
}
