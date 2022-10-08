using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.CompilerServices;

namespace Engine;
public static class Log {

	public static Level LoggingLevel { get; set; } = Level.NORMAL;
	public static string Prefix => $"[{DateTime.Now:yyyy/MM/dd/HH:mm:ss.fff}][{Thread.CurrentThread.Name}:{Environment.CurrentManagedThreadId}/{Thread.CurrentThread.CurrentCulture.Name}]";

	private static bool _initialized;
	private static NamedPipeServerStream? _pipeServer;
	private static readonly BlockingCollection<(bool, ConsoleColor, string)> _logData = new();

	public static void Error( Exception e, bool logToConsole = true ) => LogInternal( $"{e}{Environment.NewLine}", InternalLevel.ERROR, -1, logToConsole, ConsoleColor.Red );
	public static void Error( string text, bool logToConsole = true ) => LogInternal( $"{text}{Environment.NewLine}", InternalLevel.ERROR, 1, logToConsole, ConsoleColor.Red );
	public static void Warning( string text, bool logToConsole = true, bool stacktrace = false ) => LogInternal( $"{text}{Environment.NewLine}", InternalLevel.WARNING, stacktrace ? 1 : -1, logToConsole, ConsoleColor.Yellow );
	public static void Line( string text, Level logLevel, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) => Text( $"{text}{Environment.NewLine}", logLevel, color, stackLevel, logToConsole );
	public static void Text( string text, Level logLevel, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) {
		if ( logLevel > LoggingLevel )
			return;
		if ( color == ConsoleColor.Red || color == ConsoleColor.Yellow )
			color = ConsoleColor.Gray;
		LogInternal( text, (InternalLevel) logLevel, stackLevel, logToConsole, color );
	}

	private static void LogInternal( string text, InternalLevel internalLogLevel, int stackLevel, bool logToConsole, ConsoleColor color ) {
		string logString = $"{Prefix}[{internalLogLevel}]: {text}";
		if ( stackLevel >= 0 )
			logString += $"{Environment.NewLine}{GenerateStackTrace( stackLevel )}";
		if ( !_initialized ) {
			_initialized = true;
			Resources.GlobalService<ThreadManager>().Start( FileLogging, "Engine Logging" );
		}

		_logData.Add( (logToConsole, color, logString) );
	}

	private static void FileLogging() {
		string pipeName = Guid.NewGuid().ToString();
		_pipeServer = new NamedPipeServerStream( pipeName, PipeDirection.Out );
		byte[] buffer = new byte[ ushort.MaxValue + 1 ];
		Process p = Process.Start( "EngineLogging.exe", pipeName );

		try {
			_pipeServer.WaitForConnection();
			while ( true ) {
				while ( _logData.TryTake( out (bool logToConsole, ConsoleColor color, string logString) data ) ) {
					unsafe {
						uint len = (uint) Math.Min( data.logString.Length * sizeof( char ), buffer.Length );
						fixed ( byte* dstPtr = buffer ) {
							fixed ( char* srcPtr = data.logString ) {
								Unsafe.CopyBlock( dstPtr, srcPtr, len );
							}
						}
						try {
							_pipeServer.Write( buffer, 0, data.logString.Length * sizeof( char ) );
						} catch ( Exception e ) {
							Console.WriteLine( e );
						}
					}
					if ( data.logToConsole ) {
						ConsoleColor prevColor = Console.ForegroundColor;
						Console.ForegroundColor = data.color;
						Console.Write( data.logString );
						Console.ForegroundColor = prevColor;
					}
				}
			}
		} catch ( Exception e ) {
			Error( e );
		}

		_pipeServer.Close();
		_pipeServer.Dispose();
		_initialized = false;
	}

	private static string GenerateStackTrace( int level ) {
		System.Diagnostics.StackTrace stack = new( true );
		int startLevel = level + 1;
		int maxPaddingLength = 0;
		for ( int i = startLevel; i < stack.FrameCount; i++ ) {
			System.Diagnostics.StackFrame? sf = stack.GetFrame( i );
			if ( sf is null )
				continue;
			System.Reflection.MethodBase? mth = sf.GetMethod();
			if ( mth is null )
				continue;
			string fullName = mth.DeclaringType?.ReflectedType?.FullName ?? "";
			if ( fullName.Length > maxPaddingLength )
				maxPaddingLength = fullName.Length;
		}

		string output = "";
		for ( int i = startLevel; i < stack.FrameCount; i++ ) {
			System.Diagnostics.StackFrame? sf = stack.GetFrame( i );
			System.Reflection.MethodBase? mth = sf?.GetMethod();
			string fullName = mth?.DeclaringType?.ReflectedType?.FullName ?? "";
			string methodName = mth?.Name ?? "External code...";
			string lineInformation = sf is not null ? $"[{sf.GetFileLineNumber()}:{sf.GetFileColumnNumber()}]" : "";
			string spacing = new( ' ', maxPaddingLength - fullName.Length );
			output += $" @ {fullName}{spacing}.{methodName}{lineInformation}";
			if ( i < stack.FrameCount - 1 )
				output += Environment.NewLine;
		}

		return output;
	}

	public static void LogError( this Identifiable idf, Exception e, bool logToConsole = true ) => Error( $"{idf.FullName}: {e}{Environment.NewLine}", logToConsole );
	public static void LogError( this Identifiable idf, string errorText, bool logToConsole = true ) => Error( $"{idf.FullName}: {errorText}{Environment.NewLine}", logToConsole );
	public static void LogWarning( this Identifiable idf, string errorText, bool logToConsole = true, bool stacktrace = false ) => Warning( $"{idf.FullName}: {errorText}", logToConsole, stacktrace );
	public static void LogLine( this Identifiable idf, string text, Level logLevel, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) => Line( $"{idf.FullName}: {text}", logLevel, color, stackLevel, logToConsole );
	public static void LogText( this Identifiable idf, string text, Level logLevel, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) => Text( $"{idf.FullName}: {text}", logLevel, color, stackLevel, logToConsole );


	private enum InternalLevel {
		CRITICAL,
		HIGH,
		NORMAL,
		LOW,
		VERBOSE,
		WARNING,
		ERROR,
	}

	public enum Level {
		CRITICAL,
		HIGH,
		NORMAL,
		LOW,
		VERBOSE,
	}

}
