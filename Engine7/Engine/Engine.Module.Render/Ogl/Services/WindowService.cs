using Engine.Module.Render.Ogl.OOP;
using Engine.Module.Render.Ogl.Utilities;

namespace Engine.Module.Render.Ogl.Services;

public sealed class WindowService : Identifiable {
	private readonly Context _context;
	private readonly ViewportStateService _viewportStateService;
	private readonly FramebufferStateService _framebufferStateService;
	private OglWindow? _window;

	public WindowService( Context context, ViewportStateService viewportStateService, FramebufferStateService framebufferStateService ) {
		this._context = context;
		this._viewportStateService = viewportStateService;
		this._framebufferStateService = framebufferStateService;
	}

	public OglWindow Window { get => GetWindow(); }

	internal void CreateWindow() => _window ??= new OglWindow( _framebufferStateService, _viewportStateService, WindowCreationUtility.Create( _context.WindowSettings ) );

	private OglWindow GetWindow() => _window ??= new OglWindow( _framebufferStateService, _viewportStateService, WindowCreationUtility.Create( _context.WindowSettings ) );
}

public sealed class UserInputService : DisposableIdentifiable {
	protected override bool InternalDispose() {
		return true;
	}
}