using Engine.Module.Render.Ogl.Scenes;

namespace Engine.Module.Render.Ogl.Services;

public sealed class SceneService( BufferService bufferService ) {
	private readonly Dictionary<string, Scene> _scenesByName = [];
	private readonly BufferService _bufferService = bufferService;

	public Scene GetScene( string sceneName ) {
		if (!this._scenesByName.TryGetValue( sceneName, out Scene? scene ))
			this._scenesByName.Add( sceneName, scene = new( sceneName, this._bufferService ) );
		return scene;
	}
}
