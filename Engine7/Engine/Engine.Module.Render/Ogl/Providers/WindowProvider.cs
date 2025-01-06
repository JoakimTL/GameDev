using Engine.Module.Render.Ogl.OOP;
using Engine.Module.Render.Ogl.Services;

namespace Engine.Module.Render.Ogl.Providers;

public sealed class WindowProvider( WindowService windowService ) : IRenderServiceProvider {
	public OglWindow Window => windowService.Window;
}