using System.Numerics;

namespace Engine.Time;

/// <typeparam name="TPrecision">The precision floating point type to use for this clock. Would usually either be <see cref="float"/> or <see cref="double"/></typeparam>
/// <typeparam name="TTickSupplier">Supplies the elapsed ticks on a systemwide basis. Default would be <see cref="StopwatchTickSupplier"/></typeparam>
public sealed class Clock<TPrecision, TTickSupplier>
	where TPrecision :
		unmanaged, IFloatingPointIeee754<TPrecision>
	where TTickSupplier :
		ITickSupplier {

	private static readonly TPrecision _invFreq = TPrecision.One / TPrecision.CreateSaturating( TTickSupplier.Frequency );

	private readonly long _createdTick;
	private long _sessionStartTick;
	private TPrecision _talliedTime;
	private TPrecision _dilation;
	private bool _running;

	/// <param name="timeDilation">How many seconds go by for this clock during a realtime second.</param>
	public Clock( TPrecision timeDilation, bool startRunning = true ) {
		_createdTick = TTickSupplier.Ticks;
		_sessionStartTick = _createdTick;
		_talliedTime = TPrecision.Zero;
		_dilation = timeDilation;
		_running = startRunning;
	}

	/// <summary>
	/// The dialted time this clock has spent running since creation. If dilation deviate from 1, this will not be the same as the real time.
	/// </summary>
	public TPrecision Time => GetTotalTime();
	/// <summary>
	/// The time spent in the current session. Session changes happen when changing the speed of the clock or unpausing.<br/>
	/// Sessions still count when the clock is paused. For correct time measurement while utilizing all of the clock's features, use the <see cref="Time"/> property.
	/// </summary>
	public TPrecision Session => FromTicksToSeconds( GetSessionTicks() );
	/// <summary>
	/// The current dilation of the clock. This is how many seconds the clock tallies for each real second.
	/// </summary>
	public TPrecision Dilation { get => _dilation; set => SetSpeed( value ); }
	/// <summary>
	/// Whether the clock is tallying or not. Useful for stopping a system reliant on timekeeping (say a game world).
	/// </summary>
	public bool Paused { get => !_running; set => SetRunningState( !value ); }

	/// <summary>
	/// Outputs both the total elapsed time and the session time using one <see cref="TTickSupplier.Ticks"/> call.
	/// </summary>
	/// <param name="totalTime"></param>
	/// <param name="sessionTime"></param>
	public void Get( out TPrecision totalTime, out TPrecision sessionTime ) {
		long sessionTicks = GetSessionTicks();
		sessionTime = FromTicksToSeconds( sessionTicks );
		totalTime = !_running ? _talliedTime : _talliedTime + sessionTime;
	}

	private TPrecision GetTotalTime()
		=> !_running ? _talliedTime : _talliedTime + FromTicksToSeconds( GetSessionTicks() );

	private TPrecision FromTicksToSeconds( long ticks )
		=> TPrecision.CreateSaturating( ticks ) * _invFreq * _dilation;

	private long GetSessionTicks()
		=> TTickSupplier.Ticks - _sessionStartTick;

	/// <summary>
	/// Changes the speed of the clock.
	/// </summary>
	/// <param name="value">How many seconds the clock tallies per real-time seconds.</param>
	private void SetSpeed( TPrecision value ) {
		NewSession( _running );
		_dilation = value;
	}

	private void NewSession( bool tallyTime ) {
		long currentTick = TTickSupplier.Ticks;
		if (tallyTime)
			_talliedTime += FromTicksToSeconds( currentTick - _sessionStartTick );
		_sessionStartTick = currentTick;
	}

	/// <summary>
	/// Changes the running state of the clock.
	/// </summary>
	/// <param name="value">True means the clock continues running, false means the clock will be paused.</param>
	private void SetRunningState( bool value ) {
		if (value == _running)
			return;
		NewSession( !value ); //Tally time when pausing, but not when unpausing. We don't want to tally the time spent paused.
		_running = value;
	}
}

//internal sealed class TimePeriod : IDisposable {
//	private const string WINMM = "winmm.dll";

//	private static TIMECAPS timeCapabilities;

//	private static int inTimePeriod;

//	private readonly int period;

//	private int disposed;

//	[DllImport( WINMM, ExactSpelling = true )]
//	private static extern int timeGetDevCaps( ref TIMECAPS ptc, int cbtc );

//	[DllImport( WINMM, ExactSpelling = true )]
//	private static extern int timeBeginPeriod( int uPeriod );

//	[DllImport( WINMM, ExactSpelling = true )]
//	private static extern int timeEndPeriod( int uPeriod );

//	static TimePeriod() {
//		int result = timeGetDevCaps( ref timeCapabilities, Marshal.SizeOf( typeof( TIMECAPS ) ) );
//		if (result != 0) {
//			throw new InvalidOperationException( "The request to get time capabilities was not completed because an unexpected error with code " + result + " occured." );
//		}
//	}

//	internal TimePeriod( int period ) {
//		if (Interlocked.Increment( ref inTimePeriod ) != 1) {
//			Interlocked.Decrement( ref inTimePeriod );
//			throw new NotSupportedException( "The process is already within a time period. Nested time periods are not supported." );
//		}

//		if (period < timeCapabilities.wPeriodMin || period > timeCapabilities.wPeriodMax) {
//			throw new ArgumentOutOfRangeException( "period", "The request to begin a time period was not completed because the resolution specified is out of range." );
//		}

//		int result = timeBeginPeriod( period );
//		if (result != 0) {
//			throw new InvalidOperationException( "The request to begin a time period was not completed because an unexpected error with code " + result + " occured." );
//		}

//		this.period = period;
//	}

//	internal static int MinimumPeriod {
//		get {
//			return timeCapabilities.wPeriodMin;
//		}
//	}

//	internal static int MaximumPeriod {
//		get {
//			return timeCapabilities.wPeriodMax;
//		}
//	}

//	internal int Period {
//		get {
//			if (this.disposed > 0) {
//				throw new ObjectDisposedException( "The time period instance has been disposed." );
//			}

//			return this.period;
//		}
//	}

//	public void Dispose() {
//		if (Interlocked.Increment( ref this.disposed ) == 1) {
//			timeEndPeriod( this.period );
//			Interlocked.Decrement( ref inTimePeriod );
//		} else {
//			Interlocked.Decrement( ref this.disposed );
//		}
//	}

//	[StructLayout( LayoutKind.Sequential )]
//	private struct TIMECAPS {
//		internal int wPeriodMin;

//		internal int wPeriodMax;
//	}
//}