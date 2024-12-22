using Engine.Module.Entities.Render;
using Engine.Module.Render.Ogl.OOP;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Module.Render.Entities.Providers;

public sealed class WindowProvider( WindowService windowService ) : IRenderEntityServiceProvider {
	private readonly WindowService _windowService = windowService;
	public OglWindow Window => this._windowService.Window;
}