using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime.CompilerServices;
using Engine.GlobalServices;

namespace Engine;
public static class Log {

	public static Level LoggingLevel { get; set; } = Level.VERBOSE;
	public static string Prefix => $"{Thread.CurrentThread.Name}:{Environment.CurrentManagedThreadId}/{Thread.CurrentThread.CurrentCulture.Name}|{DateTime.Now:yyMMdd/HH:mm:ss.fff}";

	private static ulong[] _logCounts = new ulong[ (int) InternalLevel.ERROR + 1 ];

	private static bool _initialized;
	private static bool _stopped;
	private static NamedPipeServerStream? _pipeServer;
	private static readonly BlockingCollection<string> _logData = new();

	public static ulong GetLogCount( Level level ) => _logCounts[ (int) level ];
	public static ulong GetWarningLogCount() => _logCounts[ (int) InternalLevel.WARNING ];
	public static ulong GetErrorLogCount() => _logCounts[ (int) InternalLevel.ERROR ];

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

	public static T WarningThenReturn<T>( string text, T @return, bool logToConsole = true, bool stacktrace = false ) {
		Warning( text, logToConsole, stacktrace );
		return @return;
	}

	public static T? LineThenReturn<T>( string text, Level logLevel, T? @return, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) {
		Line( text, logLevel, color, stackLevel, logToConsole );
		return @return;
	}

	public static T? TextThenReturnDefault<T>( string text, Level logLevel, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) {
		Text( text, logLevel, color, stackLevel, logToConsole );
		return default;
	}
	public static T? WarningThenReturnDefault<T>( string text, bool logToConsole = true, bool stacktrace = false ) {
		Warning( text, logToConsole, stacktrace );
		return default;
	}

	public static T? LineThenReturnDefault<T>( string text, Level logLevel, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) {
		Line( text, logLevel, color, stackLevel, logToConsole );
		return default;
	}

	public static T? TextThenReturn<T>( string text, Level logLevel, T? @return, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) {
		Text( text, logLevel, color, stackLevel, logToConsole );
		return @return;
	}

	private static void LogInternal( string text, InternalLevel internalLogLevel, int stackLevel, bool logToConsole, ConsoleColor color ) {
		_logCounts[ (int) internalLogLevel ]++;
		string logString = $"{internalLogLevel.ToString()[ ..3 ]}|{Prefix}: {text}";
		if ( stackLevel >= 0 )
			logString += $"{Environment.NewLine}{GenerateStackTrace( stackLevel )}";
		if ( !_initialized ) {
			_initialized = true;
			Global.Get<ThreadService>().Start( FileLogging, "Engine Logging" );
		}

		_logData.Add( logString );
		ConsoleColor prevColor = Console.ForegroundColor;
		Console.ForegroundColor = color;
		Console.Write( logString );
		Console.ForegroundColor = prevColor;
	}

	private static void FileLogging() {
		string pipeName = Guid.NewGuid().ToString();
		_pipeServer = new NamedPipeServerStream( pipeName, PipeDirection.Out );
		byte[] buffer = new byte[ ushort.MaxValue + 1 ];
		Process p = Process.Start( "EngineLogging.exe", pipeName );

		try {
			_pipeServer.WaitForConnection();
			while ( !_stopped ) {
				while ( _logData.TryTake( out string? logString, 50 ) ) {
					unsafe {
						uint len = (uint) Math.Min( logString.Length * sizeof( char ), buffer.Length );
						fixed ( byte* dstPtr = buffer ) {
							fixed ( char* srcPtr = logString ) {
								Unsafe.CopyBlock( dstPtr, srcPtr, len );
							}
						}
						try {
							_pipeServer.Write( buffer, 0, logString.Length * sizeof( char ) );
						} catch ( Exception e ) {
							Console.WriteLine( e );
						}
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

    internal static void Stop() => _stopped = true;

    private static string GenerateStackTrace( int level ) {
		StackTrace stack = new( true );
		int startLevel = level + 1;
		int maxPaddingLength = 0;
		for ( int i = startLevel; i < stack.FrameCount; i++ ) {
			StackFrame? sf = stack.GetFrame( i );
			if ( sf is null )
				continue;
			MethodBase? mth = sf.GetMethod();
			if ( mth is null )
				continue;
			string fullName = mth.DeclaringType?.ReflectedType?.FullName ?? "";
			if ( fullName.Length > maxPaddingLength )
				maxPaddingLength = fullName.Length;
		}

		string output = "";
		for ( int i = startLevel; i < stack.FrameCount; i++ ) {
			StackFrame? sf = stack.GetFrame( i );
			MethodBase? mth = sf?.GetMethod();
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

	public static T LogWarningThenReturn<T>( this Identifiable idf, string text, T @return, bool logToConsole = true, bool stacktrace = false ) {
		idf.LogWarning( text, logToConsole, stacktrace );
		return @return;
	}

	public static T? LogLineThenReturn<T>( this Identifiable idf, string text, Level logLevel, T? @return, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) {
		idf.LogLine( text, logLevel, color, stackLevel, logToConsole );
		return @return;
	}

	public static T? LogTextThenReturnDefault<T>( this Identifiable idf, string text, Level logLevel, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) {
		idf.LogText( text, logLevel, color, stackLevel, logToConsole );
		return default;
	}
	public static T? LogWarningThenReturnDefault<T>( this Identifiable idf, string text, bool logToConsole = true, bool stacktrace = false ) {
		idf.LogWarning( text, logToConsole, stacktrace );
		return default;
	}

	public static T? LogLineThenReturnDefault<T>( this Identifiable idf, string text, Level logLevel, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) {
		idf.LogLine( text, logLevel, color, stackLevel, logToConsole );
		return default;
	}

	public static T? LogTextThenReturn<T>( this Identifiable idf, string text, Level logLevel, T? @return, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) {
		idf.LogText( text, logLevel, color, stackLevel, logToConsole );
		return @return;
	}

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
