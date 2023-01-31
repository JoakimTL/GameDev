using Engine.GlobalServices;
using Engine.Structure.Interfaces;

namespace Engine.Rendering.Services;

public class RenderableManagerService : IRenderService, IInitializable, IUpdateable, IDisposable {

	private readonly RenderableContainerService _renderableContainerService;



	public RenderableManagerService() {
		_renderableContainerService = Global.Get<RenderableContainerService>();
	}

	public void Initialize() {

	}

	public void Update( float time, float deltaTime ) {

	}

	public void Dispose() {

	}
}