namespace Engine.Rendering;
/*public class Context : DisposableIdentifiable {

	private readonly SingletonProvider<Identifiable> _singletonProvider;

	private bool _initialized;
	private readonly ContextTaskManager _work;
	private readonly ContextUpdateManager _updater;
	private readonly ConcurrentQueue<IDisposable> _disposables;
	public readonly Thread Thread;
	public readonly Window Window;
	public ShaderManager Shader => this._singletonProvider.Get<ShaderManager>();
	public TextureManager Textures => this._singletonProvider.Get<TextureManager>();
	public VertexBufferObjectManager VBOs => this._singletonProvider.Get<VertexBufferObjectManager>();
	public RenderDataObjectManager RDOs => this._singletonProvider.Get<RenderDataObjectManager>();
	public CompositeVertexArrayObjectDataLayoutManager VAODataLayouts => this._singletonProvider.Get<CompositeVertexArrayObjectDataLayoutManager>();
	public CompositeVertexLayoutBindingManager VAOLayoutBindings => this._singletonProvider.Get<CompositeVertexLayoutBindingManager>();
	public CompositeVertexArrayObjectManager CompositeVAOs => this._singletonProvider.Get<CompositeVertexArrayObjectManager>();
	public Mesh2Manager Mesh2 => this._singletonProvider.Get<Mesh2Manager>();
	public Mesh3Manager Mesh3 => this._singletonProvider.Get<Mesh3Manager>();
	public BufferedMeshManager BufferedMesh => this._singletonProvider.Get<BufferedMeshManager>();

	public bool InThread => Thread.CurrentThread == this.Thread;
	public float FrameTime => this._fpsTally.AverageDeltaTime * 1000;
	public float FramesPerSecond => this._fpsTally.AverageFramesPerSecond;
	public string FrameRateInformation => $"{  this._fpsTally.AverageDeltaTime * 1000:N4}ms/{this._fpsTally.AverageFramesPerSecond:N2}FPS::{this.TitleInfo ?? "No info"}";

	public string? TitleInfo { get; set; }

	public Context( Window window ) {
		this.Thread = Thread.CurrentThread;
		Glfw.MakeContextCurrent( window.Pointer );
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
		this._fpsTally = new FramerateTallyer( 60 );
		this._debugCallback = GLDebugHandler;
		Gl.DebugMessageCallback( this._debugCallback, IntPtr.Zero );
		this.Window = window;
		this._work = new ContextTaskManager();
		this._updater = new ContextUpdateManager();
		this._disposables = new ConcurrentQueue<IDisposable>();
		this._singletonProvider = new SingletonProvider<Identifiable>();
	}

	private void GLDebugHandler( DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam ) {
		switch ( severity ) {
			case DebugSeverity.DontCare:
				this.LogLine( OpenGLUtilities.GLDebugMessageDecipher( source, type, id, severity, length, message, userParam ), Log.Level.VERBOSE );
				break;
			case DebugSeverity.DebugSeverityNotification:
				switch ( source ) {
					case DebugSource.DebugSourceOther:
						this.LogLine( OpenGLUtilities.GLDebugMessageDecipher( source, type, id, severity, length, message, userParam ), Log.Level.VERBOSE, ConsoleColor.Cyan );
						break;
					case DebugSource.DebugSourceApi:
						this.LogLine( OpenGLUtilities.GLDebugMessageDecipher( source, type, id, severity, length, message, userParam ), Log.Level.VERBOSE, ConsoleColor.Cyan );
						break;
					default:
						this.LogLine( OpenGLUtilities.GLDebugMessageDecipher( source, type, id, severity, length, message, userParam ), Log.Level.LOW, ConsoleColor.Cyan );
						break;
				}
				break;
			case DebugSeverity.DebugSeverityLow:
				this.LogLine( OpenGLUtilities.GLDebugMessageDecipher( source, type, id, severity, length, message, userParam ), Log.Level.LOW, ConsoleColor.Magenta );
				break;
			case DebugSeverity.DebugSeverityMedium:
				this.LogLine( OpenGLUtilities.GLDebugMessageDecipher( source, type, id, severity, length, message, userParam ), Log.Level.NORMAL, ConsoleColor.Magenta );
				break;
			case DebugSeverity.DebugSeverityHigh:
				this.LogWarning( OpenGLUtilities.GLDebugMessageDecipher( source, type, id, severity, length, message, userParam ) );
				break;
		}
	}

	internal void Initialize() {
		if ( this._initialized )
			return;
		this._initialized = true;

		this.Textures.Initialize();
		this.VBOs.Initialize();
		this.RDOs.Initialize();
		this.Mesh2.Initialize();
		this.Mesh3.Initialize();
		this.Shader.Initialize();
	}

	public void Add( IContextUpdateable updateable ) => this._updater.Add( updateable );
	public void Enqueue( Action work ) => this._work.Enqueue( work );
	public void DisposeObject( IDisposable disposable ) => this._disposables.Enqueue( disposable );

	public T Get<T>() where T : Identifiable => this._singletonProvider.Get<T>();

	public void Update( float time, float deltaTime ) {
		this._fpsTally.Tally();
		this.Window.SetTitle( this.FrameRateInformation );
		this._work.ExecuteQueue();
		this._updater.Update();
		this._singletonProvider.Update( time, deltaTime );
		while ( this._disposables.TryDequeue( out IDisposable? disposable ) )
			disposable.Dispose();
	}

	protected override bool OnDispose() {
		this._singletonProvider.DisposeAndClear();
		this._work.Dispose();
		return true;
	}

}*/