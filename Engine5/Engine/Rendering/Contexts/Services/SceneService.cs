using Engine.GlobalServices;
using Engine.Rendering.Contexts.Objects.Scenes;
using System.Runtime.InteropServices;

namespace Engine.Rendering.Contexts.Services;

public sealed class SceneService : Identifiable, IContextService, IDisposable
{

	private Dictionary<string, Scene> _scenes;
	private readonly IdentityTypeService _identityTypeService;

	public IReadOnlyCollection<Scene> Scenes => _scenes.Values;

	public SceneService(IdentityTypeService identityTypeService)
	{
		_scenes = new();
		_identityTypeService = identityTypeService;
	}

	public Scene? Get(string sceneIdentity)
	{
		if (_scenes.TryGetValue(sceneIdentity, out Scene? scene))
			return scene;
		var sceneType = _identityTypeService.GetFromIdentity(sceneIdentity);
		if (sceneType is null)
			return null;
		scene = sceneType.GetInjectedInstance() as Scene;
		if (scene is null)
			return null;
		_scenes.Add(sceneIdentity, scene);
		return scene;
	}

	public void Dispose()
	{
		foreach (var scene in _scenes.Values)
			scene.Dispose();
	}
}
