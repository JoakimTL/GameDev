using Engine.Logging;
using Engine.Module.Render.Domain;
using Engine.Module.Render.Ogl.OOP;
using Engine.Module.Render.Ogl.Services;
using Engine.Module.Render.Ogl.Utilities;
using OpenGL;

namespace Engine.Module.Render.Ogl;
public sealed class Context : DisposableIdentifiable, IUpdateable, IInitializable {

	public readonly IInstanceProvider InstanceProvider;
	private readonly InstanceUpdaterExtension _serviceProviderUpdater;
	private readonly InstanceInitializerExtension _serviceProviderInitializer;
	private Thread? _contextThread;

	//TODO: allow values to be edited in the windowsettings. We can listen to changes and update the window accordingly.
	internal WindowSettings WindowSettings { get; }

	public Context( WindowSettings settings ) {
		InstanceProvider = InstanceManagement.CreateProvider();
		_serviceProviderUpdater = InstanceProvider.CreateUpdater();
		_serviceProviderInitializer = InstanceProvider.CreateInitializer();
		InstanceProvider.Include( this, false );
		WindowSettings = settings;
	}

	public void Initialize() {
		InstanceProvider.Get<WindowService>().CreateWindow();
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
		InstanceProvider.Catalog.Host<UserInputService>();
	}

	public void Update( double time, double deltaTime ) {
		if (Disposed) {
			this.LogWarning( "Context has been disposed!" );
			return;
		}

		OglWindow window = InstanceProvider.Get<WindowService>().Window;

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
		ContextUtilities.MakeContextCurrent( InstanceProvider.Get<WindowService>().Window.Handle );
		_contextThread = Thread.CurrentThread;
	}

	protected override bool InternalDispose() {
		InstanceProvider.Dispose();
		return true;
	}
}

