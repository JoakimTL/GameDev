using System.Numerics;
using Engine.Data.Datatypes;
using Engine.Rendering.Utilities;
using GLFW;
using OpenGL;

namespace Engine.Rendering;
public class Window : DisposableIdentifiable {

	private readonly FramerateTallyer _fpsTally;
	private readonly Gl.DebugProc _debugCallback;

	public float FrameTime => this._fpsTally.AverageDeltaTime * 1000;
	public float FramesPerSecond => this._fpsTally.AverageFramesPerSecond;
	public string FrameRateInformation => $"{  this._fpsTally.AverageDeltaTime * 1000:N4}ms/{this._fpsTally.AverageFramesPerSecond:N2}FPS::{this.TitleInfo ?? "No info"}";
	public string? TitleInfo { get; set; }

	public Vector2i Size { get; private set; }
	public Vector2 AspectRatioVector { get; private set; }
	public float AspectRatio { get; private set; }
	public bool Focused { get; private set; }
	public WindowPtr Pointer { get; }
	public InputHandling.WindowInputEventManager WindowEvents { get; }
	public InputHandling.KeyboardInputEventManager KeyboardEvents { get; }
	public InputHandling.MouseInputEventManager MouseEvents { get; }

	public Window( WindowSettings settings ) {
		this.Pointer = settings.windowMode switch {
			WindowSettings.Mode.WINDOW => OpenGLUtilities.CreateWindow( settings.windowName, settings.resolution.X, settings.resolution.Y, settings?.share ),
			WindowSettings.Mode.FULLSCREEN => OpenGLUtilities.CreateFullscreen( settings.windowName, settings.monitor, settings?.share ),
			WindowSettings.Mode.WINDOWEDFULLSCREEN => OpenGLUtilities.CreateWindowedFullscreen( settings.windowName, settings.monitor, settings?.share ),
			_ => OpenGLUtilities.CreateWindow( "Untitled", 800, 600, settings.share )
		};
		this.LogLine( $"Window created!", Log.Level.NORMAL, ConsoleColor.Green );
		Glfw.GetWindowSize( this.Pointer, out int w, out int h );
		this.Size = (w, h);
		UpdateAspectRatio();
		this.WindowEvents = new InputHandling.WindowInputEventManager( this );
		this.KeyboardEvents = new InputHandling.KeyboardInputEventManager( this );
		this.MouseEvents = new InputHandling.MouseInputEventManager( this );
		this.WindowEvents.Resized += OnResized;
		this.WindowEvents.Focused += OnFocused;
		Glfw.MakeContextCurrent( this.Pointer );
		Gl.BindAPI();
		OpenGLUtilities.SetVSync( settings?.vsyncLevel ?? 0 );
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

	private void OnFocused( bool value ) => this.Focused = value;

	private void OnResized( int w, int h ) {
		this.Size = (w, h);
		UpdateAspectRatio();
	}

	private void UpdateAspectRatio() {
		this.AspectRatio = (float) this.Size.X / this.Size.Y;
		float aspectRatioX = this.Size.X > this.Size.Y ? ( (float) this.Size.X / this.Size.Y ) : 1;
		float aspectRatioY = this.Size.Y > this.Size.X ? ( (float) this.Size.Y / this.Size.X ) : 1;
		this.AspectRatioVector = new Vector2( aspectRatioX, aspectRatioY );
	}

	public void SetSize( Vector2i newSize ) {
		if ( Vector2i.NegativeOrZero( newSize ) ) {
			this.LogError( "Tried to set window size to zero or a negative number." );
			return;
		}
		if ( this.Size == newSize )
			return;
		Glfw.SetWindowSize( this.Pointer, this.Size.X, this.Size.Y );
	}

	public void SetTitle( string newTitle ) {
		if ( string.IsNullOrEmpty( newTitle ) ) {
			this.LogError( "Tried to set window title to an invalid value." );
			return;
		}
		Glfw.SetWindowTitle( this.Pointer, newTitle );
	}

	public Vector2i GetPixelCoord( Vector2 ndc ) => Vector2i.Floor( ( ndc + Vector2.One ) * 0.5f * this.Size );

	public void Bind() {
		Framebuffer.Unbind( FramebufferTarget.DrawFramebuffer );
		Viewport.Set( 0, this.Size );
	}

	protected override bool OnDispose() {
		try {
			Glfw.DestroyWindow( this ); //TODO: FIX
		} catch {
			this.LogError( "Unable to dispose window properly." );
			throw new InvalidOperationException( "Unable to dipose window properly! There are probably undiposed ogl elements remaining." );
		}
		return true;
	}

	public static implicit operator WindowPtr( Window window ) => window.Pointer;

}
