using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Pipes;

namespace Engine.Logging;

internal sealed class LogLogic : IDisposable {

	public Log.Level LoggingLevel { get; set; } = Log.Level.NORMAL;

	private readonly CancellationTokenSource _cancellationTokenSource = new();

	private readonly Thread _consoleThread;
	private readonly Thread _pipeThread;

	private NamedPipeServerStream? _pipeServer;

	private readonly BlockingCollection<string> _pipeLogData = [];
	private readonly BlockingCollection<(InternalLevel level, ConsoleColor color, string message)> _consoleLogData = [];

	private readonly UnhandledExceptionHandler _unhandledExceptionHandler = new();

	public LogLogic() {
		this._consoleThread = new Thread( ConsoleLoggingFunction ) {
			Name = "Console Logging"
		};
		this._consoleThread.Start();
		this._pipeThread = new Thread( PipeLoggingFunction ) {
			Name = "Pipe Logging"
		};
		this._pipeThread.Start();
	}

	private static ConsoleColor DetermineColor( ConsoleColor desiredColor, InternalLevel logLevel ) {
		if (logLevel == InternalLevel.CRITICAL)
			return ConsoleColor.Red;
		if (logLevel == InternalLevel.WARNING)
			return ConsoleColor.Yellow;
		return desiredColor;
	}

	internal void LogInternal( string text, InternalLevel internalLogLevel, int stackLevel, bool logToConsole, ConsoleColor color ) {
		if ((int) internalLogLevel > (int) this.LoggingLevel)
			return;
		color = DetermineColor( color, internalLogLevel );
		string logString = $"{internalLogLevel.ToString()[ ..3 ]}|{Helper.GetLogPrefix()}: {text}";
		if (stackLevel >= 0)
			logString += $"{Environment.NewLine}{Helper.GenerateStackTrace( stackLevel )}";
		this._pipeLogData.Add( logString );
		this._consoleLogData.Add( (internalLogLevel, color, logString) );
	}

	internal void Stop() => this._cancellationTokenSource.Cancel();

	private void ConsoleLoggingFunction() {
		while (!this._cancellationTokenSource.IsCancellationRequested)
			try {
				while (this._consoleLogData.TryTake( out (InternalLevel level, ConsoleColor color, string message) data, Timeout.Infinite, this._cancellationTokenSource.Token )) {
					Log.Statistics.Increment( data.level );
					ConsoleColor prevColor = Console.ForegroundColor;
					Console.ForegroundColor = data.color;
					Console.Write( data.message );
					Console.ForegroundColor = prevColor;
				}
			} catch (OperationCanceledException) { }
	}

	private void PipeLoggingFunction() {
		string pipeName = Guid.NewGuid().ToString();
		this._pipeServer = new NamedPipeServerStream( pipeName, PipeDirection.Out );
		Process p = Process.Start( "Engine.Logging.exe", pipeName );

		try {
			this._pipeServer.WaitForConnection();
			while (!this._cancellationTokenSource.IsCancellationRequested)
				try {
					while (this._pipeLogData.TryTake( out string? message, Timeout.Infinite, this._cancellationTokenSource.Token ))
						Helper.SendMessageOverPipe( this._pipeServer, message );
				} catch (OperationCanceledException) { }
		} catch (Exception e) {
			Log.Critical( e );
		}

		this._pipeServer.Close();
		this._pipeServer.Dispose();
	}

	public void Dispose() {
		this._unhandledExceptionHandler.Dispose();
	}
}

internal sealed class UnhandledExceptionHandler : IDisposable {

	public UnhandledExceptionHandler() {
		AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
	}

	private void OnUnhandledException( object sender, UnhandledExceptionEventArgs e ) {
		if (e.ExceptionObject is Exception ex)
			Log.Critical( ex );
	}

	public void Dispose() {
		AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
	}
}