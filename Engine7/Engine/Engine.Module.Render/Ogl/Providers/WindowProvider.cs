using Engine.Module.Render.Ogl.OOP;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Module.Render.Ogl.Providers;

public sealed class WindowProvider( WindowService windowService ) : IServiceProvider {
	public OglWindow Window => windowService.Window;
}