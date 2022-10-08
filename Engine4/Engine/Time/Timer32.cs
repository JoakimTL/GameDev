using System.Diagnostics;

namespace Engine.Time;

public class Timer32 : DisposableIdentifiable {

	public delegate void TimerElapseCallback( double time, double deltaTime );

	private double _previousElapseTime;

	private bool _newInterval;
	private int _interval;
	/// <summary>
	/// The time till the next interval
	/// </summary>
	private int _remainingInterval;
	/// <summary>
	/// The time this timer started ticking
	/// </summary>
	private long _startTime;
	/// <summary>
	/// The amount of elapsed intervals since the timer was started
	/// </summary>
	private int _tickCount;
	/// <summary>
	/// Used to block the timer while it's not active.
	/// </summary>
	private readonly AutoResetEvent _startEvent;
	/// <summary>
	/// The internal timer. Used to create all the timer ticks.
	/// </summary>
	private readonly ManualResetEvent _tickerEvent;

	public bool Enabled { get; private set; }
	public int Interval {
		get => this._interval;
		set => SetInterval( value );
	}

	/// <summary>
	/// This event is raised every time the interval elapses.
	/// </summary>
	public event TimerElapseCallback? Elapsed;

	/// <summary>
	/// Creates a new <see cref="Timer32"/>, with a custom interval.
	/// </summary>
	/// <param name="interval">The interval between each tick, in milliseconds.</param>
	public Timer32( string? name, int interval, bool background = true ) {
		this._interval = interval;
		this.Enabled = false;

		this._tickerEvent = new ManualResetEvent( false );
		this._startEvent = new AutoResetEvent( false );

		Resources.GlobalService<ThreadManager>().Start( InternalTimerCallback, name ?? this.FullName, background );
	}

	/// <summary>
	/// Creates a new <see cref="Timer32"/>, with an interval of 1 second (1000ms)
	/// </summary>
	public Timer32() : this( null, 1000 ) { }

	//The function called by the timer thread
	private void InternalTimerCallback() {
		while ( !this.Disposed ) {
			this._startEvent.WaitOne();
			this._tickerEvent.Reset();

			this._tickCount = 0;
			this._startTime = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond;
			this._remainingInterval = this._interval;

			while ( this.Enabled ) {
				bool shouldBreak = false;
				if ( this._tickerEvent.WaitOne( this._remainingInterval ) )
					//The ticker was set, which means the timer has been disabled!
					shouldBreak = true;

				//If a new interval has been selected while the timer is on, the timer stops and resets with the new interval.
				if ( this._newInterval ) {
					this._startEvent.Set();
					this._newInterval = false;
					shouldBreak = true;
				}

				if ( shouldBreak )
					break;

				double time = Clock64.StartupTime;
				double deltaTime = time - this._previousElapseTime;
				this._previousElapseTime = time;
				Elapsed?.Invoke( time, deltaTime );

				this._tickCount++;

				long actualTime = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond;
				long expectedTime = this._startTime + ( this._interval * this._tickCount );

				//Setting the remaining interval for the next tick such that the timer has the expected number of ticks. When the 50 ticks happen over 50 intervals is not as important as 50 ticks happening by the time 50 intervals have elapsed.
				this._remainingInterval = this._interval - (int) ( actualTime - expectedTime );

				if ( this._remainingInterval < 0 )
					this._remainingInterval = 0;
			}
		}
	}

	/// <summary>
	/// Starts the timer, with a set interval<br />To stop the timer use <see cref="Stop"/>!
	/// </summary>
	/// <param name="ms">The new interval the timer ticks on. Cannot be a negative integer.</param>
	public void Start( int ms ) {
		if ( this.Enabled )
			return;
		if ( ms < 0 )
			return;
		this._interval = ms;
		this.Enabled = true;
		this._startEvent.Set();
	}

	/// <summary>
	/// Sets the interfal of the timer
	/// </summary>
	/// <param name="ms">The new interval the timer ticks on. Cannot be a negative integer.</param>
	public void SetInterval( int ms ) {
		if ( ms < 0 )
			return;
		this._interval = ms;
		if ( this.Enabled ) {
			this._newInterval = true;
			this._tickerEvent.Set();
		}
	}

	/// <summary>
	/// Starts the timer on the current interval.<br />To stop the timer use <see cref="Stop"/>!
	/// </summary>
	public void Start() {
		this.Enabled = true;
		this._startEvent.Set();
	}

	/// <summary>
	/// Stops the timer, to start the timer use <see cref="Start"/> or <see cref="Start(int)"/>!
	/// </summary>
	public void Stop() {
		this.Enabled = false;
		this._tickerEvent.Set();
	}

	protected override bool OnDispose() {
		Stop();
		return true;
	}
}