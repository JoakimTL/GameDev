namespace Engine;
public class ThreadManager : Identifiable {

	private readonly List<Thread> _threads;
	public IReadOnlyList<Thread> Threads => this._threads;

	public ThreadManager() {
		Thread.CurrentThread.Name = "Entry";
		Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
		this._threads = new List<Thread> { Thread.CurrentThread };
	}

	public Thread Start( ThreadStart start, string name, bool background = true ) {
		Thread t = new( start ) {
			Name = name,
			IsBackground = background,
			CurrentCulture = System.Globalization.CultureInfo.InvariantCulture
		};

		lock ( this._threads )
			this._threads.Add( t );
		t.Start();
		this.LogLine( $"Started new thread [{t.Name}:{t.ManagedThreadId}]!", Log.Level.NORMAL );

		return t;
	}

	public Thread Start( Data.ResourceManagement.Threads.IRunnable runnable, string name, bool background = true ) {
		Thread t = new( runnable.Run ) {
			Name = name,
			IsBackground = background,
			CurrentCulture = System.Globalization.CultureInfo.InvariantCulture
		};

		lock ( this._threads )
			this._threads.Add( t );
		t.Start();
		this.LogLine( $"Started new thread [{t.Name}:{t.ManagedThreadId}]!", Log.Level.NORMAL );

		return t;
	}
}
