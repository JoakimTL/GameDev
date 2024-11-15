namespace Engine;

public sealed class LogStatistics {

	private readonly ulong[] _logCounts = new ulong[ (int) InternalLevel.VERBOSE + 1 ];

	public ulong LogCalls( Log.Level level ) => this._logCounts[ (int) level ];
	public ulong Warnings() => this._logCounts[ (int) InternalLevel.WARNING ];
	public ulong Criticals() => this._logCounts[ (int) InternalLevel.CRITICAL ];

	internal void Increment( InternalLevel level ) {
		this._logCounts[ (int) level ]++;
	}
}


