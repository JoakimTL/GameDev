namespace Engine.Logging;

public static class LoggingExtensions {

	public static void Log( this Exception e, bool logToConsole = true ) => Logging.Log.Critical( e, logToConsole );
	public static void LogWarning( this object o, string errorText, bool logToConsole = true, bool stacktrace = false ) => Logging.Log.Warning( $"{o}: {errorText}", logToConsole, stacktrace );
	public static void LogLine( this object o, string text, Log.Level logLevel = Logging.Log.Level.NORMAL, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) => Logging.Log.Line( $"{o}: {text}", logLevel, color, stackLevel, logToConsole );
	public static void LogText( this object o, string text, Log.Level logLevel = Logging.Log.Level.NORMAL, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) => Logging.Log.Text( $"{o}: {text}", logLevel, color, stackLevel, logToConsole );

	public static T LogWarningThenReturn<T>( this object o, string text, T @return, bool logToConsole = true, bool stacktrace = false ) {
		o.LogWarning( text, logToConsole, stacktrace );
		return @return;
	}

	public static T? LogLineThenReturn<T>( this object o, string text, Log.Level logLevel, T? @return, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) {
		o.LogLine( text, logLevel, color, stackLevel, logToConsole );
		return @return;
	}

	public static T? LogTextThenReturnDefault<T>( this object o, string text, Log.Level logLevel, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) {
		o.LogText( text, logLevel, color, stackLevel, logToConsole );
		return default;
	}
	public static T? LogWarningThenReturnDefault<T>( this object o, string text, bool logToConsole = true, bool stacktrace = false ) {
		o.LogWarning( text, logToConsole, stacktrace );
		return default;
	}

	public static T? LogLineThenReturnDefault<T>( this object o, string text, Log.Level logLevel, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) {
		o.LogLine( text, logLevel, color, stackLevel, logToConsole );
		return default;
	}

	public static T? LogTextThenReturn<T>( this object o, string text, Log.Level logLevel, T? @return, ConsoleColor color = ConsoleColor.Gray, int stackLevel = -1, bool logToConsole = true ) {
		o.LogText( text, logLevel, color, stackLevel, logToConsole );
		return @return;
	}
}


