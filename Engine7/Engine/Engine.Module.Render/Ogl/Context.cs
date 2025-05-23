﻿using Engine.Logging;
using Engine.Module.Render.Domain;
using Engine.Module.Render.Glfw;
using Engine.Module.Render.Ogl.OOP;
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

	public event Action<Context>? OnInitialized;

	public Context( WindowSettings settings ) {
		this.WindowSettings = settings;
		this.InstanceProvider = InstanceManagement.CreateProvider(true);
		this._serviceProviderUpdater = this.InstanceProvider.CreateUpdater();
		this._serviceProviderInitializer = this.InstanceProvider.CreateInitializer();
		this._pipelineExecuterExtension = new( this.InstanceProvider );
		this.InstanceProvider.Inject( this, false );
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
		Gl.GetInteger( GetPName.MaxColorTextureSamples, out uint maxSamples );
		Log.Line( $"Max color texture samples: {maxSamples}", Log.Level.NORMAL, ConsoleColor.Blue );
		this.InstanceProvider.Get<GlDebugMessageService>().BindErrorCallback();
		this.InstanceProvider.Catalog.Host<UserInputService>();
		GLFW.SwapInterval( (int) WindowSettings.VSyncLevel );
		OnInitialized?.Invoke( this );
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

		this.InstanceProvider.Get<ViewportStateService>().Set( 0, window.Size );

		Gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
		this._pipelineExecuterExtension.DrawToScreen();

		window.SwapBuffer();
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