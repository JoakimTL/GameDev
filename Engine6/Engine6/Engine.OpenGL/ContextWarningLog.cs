namespace Engine.OpenGL;

public sealed class ContextWarningLog {

	private static Queue<string> _warnings;

	public ContextWarningLog() {
		_warnings = new();
	}

	internal void LogWarning( string message ) => _warnings.Enqueue( message );

	public string ConsumeLogEntry() => _warnings.Dequeue();

}
