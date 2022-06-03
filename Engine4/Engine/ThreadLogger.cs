using System.Collections.Concurrent;
using Engine.Data.ResourceManagement.Threads;
using Engine.Time;

namespace Engine;

public class ThreadLogger : Identifiable {

	/// <summary>
	/// Timer utilized for tracking
	/// </summary>
	private readonly Timer32 _logTimer;

	private readonly ConcurrentDictionary<Thread, ThreadLog> _logs;

	internal ThreadLogger() {
		this._logs = new ConcurrentDictionary<Thread, ThreadLog>();
		this._logTimer = new Timer32( "Thread Logger", 100 );
		this._logTimer.Elapsed += InternalCallback;
		this._logTimer.Start();
	}

	public ThreadLog? Get( Thread t ) {
		if ( this._logs.TryGetValue( t, out ThreadLog? l ) )
			return l;
		return null;
	}

	private void InternalCallback() {
		//If the collection is changed during the loop, the newest threads will not be included at this time. Since the thread collection only increases in size, this is a safe operation.
		IReadOnlyList<Thread> threads = Resources.Get<ThreadManager>().Threads;
		int c = threads.Count;
		double time = Clock64.StartupTime;
		for ( int i = 0; i < c; i++ ) {
			Thread t = threads[ i ];

			//Checks if the thread is being logged, or adds a log for the thread if it needs one.
			if ( !this._logs.TryGetValue( t, out ThreadLog? log ) )
				this._logs.TryAdd( t, log = new ThreadLog() );

			//Checks if the state of the checked thread is still the same. If not, then append the new state to the log.
			if ( log.Latest.State != t.ThreadState )
				log.Append( new ThreadLogSection( time, t.ThreadState ) );
		}
	}
}
