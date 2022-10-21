using Engine.Structure.Interfaces;
using GLFW;
using OpenGL;

namespace Engine.Rendering.Services;

public sealed class GlDebugMessageService : IRenderService, IInitializable {
	private Gl.DebugProc _debugCallback;

	public GlDebugMessageService() {
		this._debugCallback = GLDebugHandler;
	}

	public void Initialize() {
		Gl.DebugMessageCallback( this._debugCallback, IntPtr.Zero );
	}

	public void BindErrorCallback() => Glfw.SetErrorCallback( ErrorCallback );


	private void ErrorCallback( GLFW.ErrorCode code, IntPtr desc ) {
		Log.Error( "GLFW Error: " + code + ": " + desc );
		Console.ReadLine();
	}

	private void GLDebugHandler( DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam ) {
		switch ( severity ) {
			case DebugSeverity.DontCare:
				Log.Line( GLDebugMessageDecipher( source, type, id, severity, length, message, userParam ), Log.Level.VERBOSE );
				break;
			case DebugSeverity.DebugSeverityNotification:
				switch ( source ) {
					case DebugSource.DebugSourceOther:
						Log.Line( GLDebugMessageDecipher( source, type, id, severity, length, message, userParam ), Log.Level.VERBOSE, ConsoleColor.Cyan );
						break;
					case DebugSource.DebugSourceApi:
						Log.Line( GLDebugMessageDecipher( source, type, id, severity, length, message, userParam ), Log.Level.VERBOSE, ConsoleColor.Cyan );
						break;
					default:
						Log.Line( GLDebugMessageDecipher( source, type, id, severity, length, message, userParam ), Log.Level.LOW, ConsoleColor.Cyan );
						break;
				}
				break;
			case DebugSeverity.DebugSeverityLow:
				Log.Line( GLDebugMessageDecipher( source, type, id, severity, length, message, userParam ), Log.Level.LOW, ConsoleColor.Magenta );
				break;
			case DebugSeverity.DebugSeverityMedium:
				Log.Line( GLDebugMessageDecipher( source, type, id, severity, length, message, userParam ), Log.Level.NORMAL, ConsoleColor.Magenta );
				break;
			case DebugSeverity.DebugSeverityHigh:
				Log.Warning( GLDebugMessageDecipher( source, type, id, severity, length, message, userParam ) );
				break;
		}
	}

	private string GLDebugMessageDecipher( DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam ) {
		unsafe {
			string errorDescription = "";
			for ( int i = 0; i < length; i++ )
				errorDescription += (char) ( (byte*) message )[ i ];

			string sourceText = source switch {
				DebugSource.DontCare => "None",
				DebugSource.DebugSourceWindowSystem => "Window System",
				DebugSource.DebugSourceThirdParty => "3rd Party",
				DebugSource.DebugSourceShaderCompiler => "Shader Compiler",
				DebugSource.DebugSourceOther => "Other",
				DebugSource.DebugSourceApplication => "Application",
				DebugSource.DebugSourceApi => "API",
				_ => "Unknown",
			};
			string typeText = type switch {
				DebugType.DontCare => "None",
				DebugType.DebugTypeUndefinedBehavior => "Undefined",
				DebugType.DebugTypePushGroup => "Push",
				DebugType.DebugTypePortability => "Portability",
				DebugType.DebugTypePopGroup => "Pop",
				DebugType.DebugTypePerformance => "Performance",
				DebugType.DebugTypeOther => "Other",
				DebugType.DebugTypeMarker => "Marker",
				DebugType.DebugTypeError => "Error",
				DebugType.DebugTypeDeprecatedBehavior => "Deprecated",
				_ => "Unknown",
			};
			string severityText = severity switch {
				DebugSeverity.DontCare => "None",
				DebugSeverity.DebugSeverityNotification => "Note",
				DebugSeverity.DebugSeverityLow => "Low",
				DebugSeverity.DebugSeverityMedium => "Med",
				DebugSeverity.DebugSeverityHigh => "High",
				_ => "Unknown",
			};
			return $"[{id}/{severityText}] {sourceText} {typeText}: {errorDescription}";
		}
	}
}