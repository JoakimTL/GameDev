using System.Diagnostics;
using Engine.GlobalServices;

namespace Engine.Time;

public class TickingTimer : Identifiable
{

    public delegate void TimerElapseCallback(double time, double deltaTime);

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
    public int Interval
    {
        get => _interval;
        set => SetInterval(value);
    }

    /// <summary>
    /// This event is raised every time the interval elapses.
    /// </summary>
    public event TimerElapseCallback? Elapsed;

    /// <summary>
    /// Creates a new <see cref="TickingTimer"/>, with a custom interval.
    /// </summary>
    /// <param name="interval">The interval between each tick, in milliseconds.</param>
    public TickingTimer(string? name, int interval, bool background = true)
    {
        _interval = interval;
        Enabled = false;

        _tickerEvent = new ManualResetEvent(false);
        _startEvent = new AutoResetEvent(false);

        Global.Get<ThreadService>().Start(InternalTimerCallback, name ?? FullName, background);
    }

    /// <summary>
    /// Creates a new <see cref="TickingTimer"/>, with an interval of 1 second (1000ms)
    /// </summary>
    public TickingTimer() : this(null, 1000) { }

    //The function called by the timer thread
    private void InternalTimerCallback()
    {
        while (Enabled)
        {
            _startEvent.WaitOne();
            _tickerEvent.Reset();

            _tickCount = 0;
            _startTime = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond;
            _remainingInterval = _interval;

            while (Enabled)
            {
                bool shouldBreak = false;
                if (_tickerEvent.WaitOne(_remainingInterval))
                    //The ticker was set, which means the timer has been disabled!
                    shouldBreak = true;

                //If a new interval has been selected while the timer is on, the timer stops and resets with the new interval.
                if (_newInterval)
                {
                    _startEvent.Set();
                    _newInterval = false;
                    shouldBreak = true;
                }

                if (shouldBreak)
                    break;

                double time = Clock64.StartupTime;
                double deltaTime = time - _previousElapseTime;
                _previousElapseTime = time;
                Elapsed?.Invoke(time, deltaTime);

                _tickCount++;

                long actualTime = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond;
                long expectedTime = _startTime + (_interval * _tickCount);

                //Setting the remaining interval for the next tick such that the timer has the expected number of ticks. When the 50 ticks happen over 50 intervals is not as important as 50 ticks happening by the time 50 intervals have elapsed.
                _remainingInterval = _interval - (int)(actualTime - expectedTime);

                if (_remainingInterval < 0)
                    _remainingInterval = 0;
            }
        }
    }

    /// <summary>
    /// Starts the timer, with a set interval<br />To stop the timer use <see cref="Stop"/>!
    /// </summary>
    /// <param name="ms">The new interval the timer ticks on. Cannot be a negative integer.</param>
    public void Start(int ms)
    {
        if (Enabled)
            return;
        if (ms < 0)
            return;
        _interval = ms;
        Enabled = true;
        _startEvent.Set();
    }

    /// <summary>
    /// Sets the interfal of the timer
    /// </summary>
    /// <param name="ms">The new interval the timer ticks on. Cannot be a negative integer.</param>
    public void SetInterval(int ms)
    {
        if (ms < 0)
            return;
        _interval = ms;
        if (Enabled)
        {
            _newInterval = true;
            _tickerEvent.Set();
        }
    }

    /// <summary>
    /// Starts the timer on the current interval.<br />To stop the timer use <see cref="Stop"/>!
    /// </summary>
    public void Start()
    {
        Enabled = true;
        _startEvent.Set();
    }

    /// <summary>
    /// Stops the timer, to start the timer use <see cref="Start"/> or <see cref="Start(int)"/>!
    /// </summary>
    public void Stop()
    {
        Enabled = false;
        _tickerEvent.Set();
    }
}