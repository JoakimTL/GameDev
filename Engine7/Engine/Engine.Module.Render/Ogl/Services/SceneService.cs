using Engine.Module.Render.Ogl.Scenes;

namespace Engine.Module.Render.Ogl.Services;

public sealed class SceneService( BufferService bufferService ) {
	private readonly Dictionary<string, Scene> _scenesByName = [];
	private readonly BufferService _bufferService = bufferService;

	public Scene GetScene( string sceneName ) {
		if (!_scenesByName.TryGetValue( sceneName, out Scene? scene ))
			_scenesByName.Add( sceneName, scene = new( sceneName, _bufferService ) );
		return scene;
	}
}
