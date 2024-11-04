namespace Engine;

public sealed class LogStatistics {

	private readonly ulong[] _logCounts = new ulong[ (int) InternalLevel.VERBOSE + 1 ];

	public ulong LogCalls( Log.Level level ) => _logCounts[ (int) level ];
	public ulong Warnings() => _logCounts[ (int) InternalLevel.WARNING ];
	public ulong Criticals() => _logCounts[ (int) InternalLevel.CRITICAL ];

	internal void Increment( InternalLevel level ) {
		_logCounts[ (int) level ]++;
	}
}


