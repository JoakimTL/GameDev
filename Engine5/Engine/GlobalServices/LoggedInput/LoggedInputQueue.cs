using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Engine.GlobalServices.LoggedInput;

public sealed class LoggedInputQueue : IDisposable {

    private readonly ConcurrentQueue<TimedEvent> _timedEventStates;
    private readonly LoggedInputService _loggedInputService;

    public LoggedInputQueue( LoggedInputService loggedInputService ) {
        this._loggedInputService = loggedInputService;
		_timedEventStates = new();
        _loggedInputService.TimedEvent += OnTimedEvent;
    }

    private void OnTimedEvent( TimedEvent timedEvent ) 
        => _timedEventStates.Enqueue( timedEvent );
    public bool TryDequeueTimedEvent( [NotNullWhen( true )] out TimedEvent? timedEventState ) 
        => _timedEventStates.TryDequeue( out timedEventState );

    public void Dispose() {
        _loggedInputService.TimedEvent -= OnTimedEvent;
    }
}
