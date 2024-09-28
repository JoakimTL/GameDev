using Engine.Modules.Rendering.Ogl.OOP;
using Engine.Modules.Rendering.Ogl.Services;
using Engine.Modules.Rendering.Ogl.Utilities;
using OpenGL;

namespace Engine.Modules.Rendering;

public sealed class Context : DisposableIdentifiable, IUpdateable, IInitializable {

	private readonly IServiceProvider _serviceProvider;
	private readonly ServiceProviderDisposalExtension _serviceProviderDisposer;
	private readonly ServiceProviderUpdateExtension _serviceProviderUpdater;
	private readonly ServiceProviderInitializationExtension _serviceProviderInitializer;
	private Thread? _contextThread;

	//TODO: allow values to be edited in the windowsettings. We can listen to changes and update the window accordingly.
	internal WindowSettings WindowSettings { get; }

	public Context( WindowSettings settings ) {
		_serviceProvider = Services.CreateServiceProvider( ContextServiceRegistryProvider.ServiceRegistry );
		_serviceProviderUpdater = new( _serviceProvider );
		_serviceProviderDisposer = new( _serviceProvider );
		_serviceProviderInitializer = new( _serviceProvider );
		_serviceProvider.AddConstant( this );
		WindowSettings = settings;
	}

	public void Initialize() {
		_serviceProvider.GetService<WindowService>().CreateWindow();
		Bind();
		Gl.BindAPI();
		Log.Line( "Using OpenGL version: " + Gl.GetString( StringName.Version ), Log.Level.NORMAL, ConsoleColor.Blue );
		Gl.GetInteger( GetPName.MaxVertexAttribBindings, out uint maxAttribBindings );
		Gl.GetInteger( GetPName.MaxVertexAttribs, out uint maxAttribs );
		Gl.GetInteger( GetPName.MaxVertexAttribRelativeOffset, out uint maxAttribOffset );
		Log.Line( $"Max vertex spec: Bindings: {maxAttribBindings}, Attributes: {maxAttribs}, Offset: {maxAttribOffset}B", Log.Level.NORMAL, ConsoleColor.Blue );
		Gl.GetInteger( GetPName.MaxUniformBlockSize, out uint maxBlockSize );
		Gl.GetInteger( GetPName.MaxUniformBufferBindings, out uint maxBindings );
		Gl.GetInteger( GetPName.MaxUniformLocations, out uint maxLocations );
		Log.Line( $"Max uniform spec: Bindings: {maxBindings}, Locations: {maxLocations}, Block size: {maxBlockSize}B", Log.Level.NORMAL, ConsoleColor.Blue );
		Gl.GetInteger( GetPName.MaxShaderStorageBufferBindings, out maxBindings );
		Log.Line( $"Max shader storage spec: Bindings: {maxBindings}", Log.Level.NORMAL, ConsoleColor.Blue );
		_serviceProvider.GetService<UserInputService>();
	}

	public void Update( in double time, in double deltaTime ) {
		if (Disposed) {
			this.LogWarning( "Context has been disposed!" );
			return;
		}

		OglWindow window = _serviceProvider.GetService<WindowService>().Window;

		if (window.ShouldClose) {
			window.Title = "Closing!";
			Dispose();
			return;
		}

		Bind();
		window.CheckResize();

		_serviceProviderInitializer.Update( time, deltaTime );
		_serviceProviderUpdater.Update( time, deltaTime );

		window.SwapBuffer();

		//if (WindowUtilities.ShouldWindowClose( Pointer )) {
		//	WindowUtilities.SetTitle( Pointer, "Closing!" );
		//	Dispose();
		//	Closed = true;
		//	Closing?.Invoke( this );
		//	return;
		//}

		//_context.Bind();
		//Glfw.GetWindowSize( Pointer, out int w, out int h );
		//if (Size.X != w || Size.Y != h) {
		//	Size = (w, h);
		//	UpdateAspectRatio();
		//	Resized?.Invoke( this );
		//}

		//_context.Update( time, deltaTime );
		//SetTitle( $"{(1f / deltaTime):N2} FPS " );
		//WindowUtilities.SetTitle( Pointer, _title );
		//WindowUtilities.SwapBuffer( Pointer );
	}

	private void Bind() {
		ContextUtilities.MakeContextCurrent( _serviceProvider.GetService<WindowService>().Window.Handle );
		_contextThread = Thread.CurrentThread;
	}

	protected override bool InternalDispose() {
		_serviceProviderDisposer.Dispose();
		return true;
	}
}
