namespace Engine.Logging;

public static class Log {

	public static Level LoggingLevel { get => GetLog().LoggingLevel; set => GetLog().LoggingLevel = value; }
	private static LogLogic? _log;
	public static LogStatistics Statistics { get; } = new();

	public static void Critical( Exception e, bool logToConsole = true )
		=> GetLog().LogInternal( $"{e}{Environment.NewLine}", InternalLevel.CRITICAL, -1, logToConsole, ConsoleColor.Red );
	public static void Warning( string text, bool logToConsole = true, bool stacktrace = false )
		=> GetLog().LogInternal( $"{text}{Environment.NewLine}", InternalLevel.WARNING, stacktrace ? 1 : -1, logToConsole, ConsoleColor.Yellow );
	public static void Line( string text, Level logLevel = Level.NORMAL, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true )
		=> Text( $"{text}{Environment.NewLine}", logLevel, color, stackLevel, logToConsole );
	public static void Text( string text, Level logLevel = Level.NORMAL, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true )
		=> GetLog().LogInternal( text, (InternalLevel) logLevel, stackLevel, logToConsole, color );

	public static T WarningThenReturn<T>( string text, T @return, bool logToConsole = true, bool stacktrace = false ) {
		Warning( text, logToConsole, stacktrace );
		return @return;
	}

	public static T? LineThenReturn<T>( string text, T? @return, Level logLevel = Level.NORMAL, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) {
		Line( text, logLevel, color, stackLevel, logToConsole );
		return @return;
	}

	public static T? TextThenReturnDefault<T>( string text, Level logLevel = Level.NORMAL, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) {
		Text( text, logLevel, color, stackLevel, logToConsole );
		return default;
	}

	public static T? WarningThenReturnDefault<T>( string text, bool logToConsole = true, bool stacktrace = false ) {
		Warning( text, logToConsole, stacktrace );
		return default;
	}

	public static T? LineThenReturnDefault<T>( string text, Level logLevel = Level.NORMAL, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) {
		Line( text, logLevel, color, stackLevel, logToConsole );
		return default;
	}

	public static T? TextThenReturn<T>( string text, T? @return, Level logLevel = Level.NORMAL, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) {
		Text( text, logLevel, color, stackLevel, logToConsole );
		return @return;
	}

	public static void Stop() {
		_log?.Stop();
		_log?.Dispose();
		_log = null;
	}

	private static LogLogic GetLog() => _log ??= new();

	public enum Level {
		HIGH = InternalLevel.HIGH,
		NORMAL = InternalLevel.NORMAL,
		VERBOSE = InternalLevel.VERBOSE,
	}
}
