using Engine.Logging;
using Engine.Module.Render.Domain;
using Engine.Module.Render.Ogl.OOP;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.Services;
using Engine.Module.Render.Ogl.Utilities;
using OpenGL;

namespace Engine.Module.Render.Ogl;
public sealed class Context : DisposableIdentifiable, IUpdateable, IInitializable {

	public readonly IInstanceProvider InstanceProvider;
	private readonly InstanceUpdaterExtension _serviceProviderUpdater;
	private readonly InstanceInitializerExtension _serviceProviderInitializer;
	private readonly RenderPipelineExecuterInstanceExtension _pipelineExecuterExtension;
	private Thread? _contextThread;

	//TODO: allow values to be edited in the windowsettings. We can listen to changes and update the window accordingly.
	internal WindowSettings WindowSettings { get; }

	public Context( WindowSettings settings ) {
		this.InstanceProvider = InstanceManagement.CreateProvider();
		this._serviceProviderUpdater = this.InstanceProvider.CreateUpdater();
		this._serviceProviderInitializer = this.InstanceProvider.CreateInitializer();
		this._pipelineExecuterExtension = new( this.InstanceProvider );
		this.InstanceProvider.Inject( this, false );
		this.WindowSettings = settings;
	}

	public void Initialize() {
		this.InstanceProvider.Get<WindowService>().CreateWindow();
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
		this.InstanceProvider.Catalog.Host<UserInputService>();
	}

	public void Update( double time, double deltaTime ) {
		if (this.Disposed) {
			this.LogWarning( "Context has been disposed!" );
			return;
		}

		OglWindow window = this.InstanceProvider.Get<WindowService>().Window;

		if (window.ShouldClose) {
			window.Title = "Closing!";
			Dispose();
			return;
		}

		Bind();
		window.CheckResize();

		this._serviceProviderInitializer.Update( time, deltaTime );
		this._serviceProviderUpdater.Update( time, deltaTime );
		this._pipelineExecuterExtension.PrepareRendering( time, deltaTime );

		InstanceProvider.Get<ViewportStateService>().Set( 0, window.Size );

		this._pipelineExecuterExtension.DrawToScreen();

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
		ContextUtilities.MakeContextCurrent( this.InstanceProvider.Get<WindowService>().Window.Handle );
		this._contextThread = Thread.CurrentThread;
	}

	protected override bool InternalDispose() {
		this.InstanceProvider.Dispose();
		return true;
	}
}