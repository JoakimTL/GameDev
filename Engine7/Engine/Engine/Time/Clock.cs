using System.Numerics;

namespace Engine.Time;

/// <typeparam name="TPrecision">The precision floating point type to use for this clock. Would usually either be <see cref="float"/> or <see cref="double"/></typeparam>
/// <typeparam name="TTickSupplier">Supplies the elapsed ticks on a systemwide basis. Default would be <see cref="StopwatchTickSupplier"/></typeparam>
public sealed class Clock<TPrecision, TTickSupplier>
	where TPrecision :
		unmanaged, IFloatingPointIeee754<TPrecision>
	where TTickSupplier :
		ITickSupplier {

	public static Clock<double, StopwatchTickSupplier> ReferenceClock { get; } = new( 1, true, true );

	private static readonly TPrecision _invFreq = TPrecision.One / TPrecision.CreateSaturating( TTickSupplier.Frequency );

	public Clock<TPrecisionNew, TTickSupplierNew> CreateNewFrom<TPrecisionNew, TTickSupplierNew>( TPrecisionNew timeDilation, bool startRunning = true, bool locked = false )
		where TPrecisionNew :
			unmanaged, IFloatingPointIeee754<TPrecisionNew>
		where TTickSupplierNew :
			ITickSupplier => new( this._createdTick, timeDilation, startRunning, @locked );

	private readonly long _createdTick;
	private long _sessionStartTick;
	private TPrecision _talliedTime;
	private TPrecision _dilation;
	private bool _running;
	private readonly bool _locked;

	private Clock( long startTick, TPrecision timeDilation, bool startRunning = true, bool locked = false ) {
		this._createdTick = startTick;
		this._sessionStartTick = this._createdTick;
		this._talliedTime = TPrecision.Zero;
		this._dilation = timeDilation;
		this._running = startRunning;
		this._locked = locked;
	}

	/// <param name="timeDilation">How many seconds go by for this clock during a realtime second.</param>
	public Clock( TPrecision timeDilation, bool startRunning = true, bool locked = false ) : this( TTickSupplier.Ticks, timeDilation, startRunning, locked ) { }

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
	public TPrecision Dilation { get => this._dilation; set => SetSpeed( value ); }
	/// <summary>
	/// Whether the clock is tallying or not. Useful for stopping a system reliant on timekeeping (say a game world).
	/// </summary>
	public bool Paused { get => !this._running; set => SetRunningState( !value ); }

	/// <summary>
	/// Outputs both the total elapsed time and the session time using one <see cref="TTickSupplier.Ticks"/> call.
	/// </summary>
	/// <param name="totalTime"></param>
	/// <param name="sessionTime"></param>
	public void Get( out TPrecision totalTime, out TPrecision sessionTime ) {
		long sessionTicks = GetSessionTicks();
		sessionTime = FromTicksToSeconds( sessionTicks );
		totalTime = !this._running ? this._talliedTime : this._talliedTime + sessionTime;
	}

	private TPrecision GetTotalTime()
		=> !this._running ? this._talliedTime : this._talliedTime + FromTicksToSeconds( GetSessionTicks() );

	private TPrecision FromTicksToSeconds( long ticks )
		=> TPrecision.CreateSaturating( ticks ) * _invFreq * this._dilation;

	private long GetSessionTicks()
		=> TTickSupplier.Ticks - this._sessionStartTick;

	/// <summary>
	/// Changes the speed of the clock.
	/// </summary>
	/// <param name="value">How many seconds the clock tallies per real-time seconds.</param>
	private void SetSpeed( TPrecision value ) {
		if (this._locked)
			return;
		NewSession( this._running );
		this._dilation = value;
	}

	private void NewSession( bool tallyTime ) {
		long currentTick = TTickSupplier.Ticks;
		if (tallyTime)
			this._talliedTime += FromTicksToSeconds( currentTick - this._sessionStartTick );
		this._sessionStartTick = currentTick;
	}

	/// <summary>
	/// Changes the running state of the clock.
	/// </summary>
	/// <param name="value">True means the clock continues running, false means the clock will be paused.</param>
	private void SetRunningState( bool value ) {
		if (this._locked)
			return;
		if (value == this._running)
			return;
		NewSession( !value ); //Tally time when pausing, but not when unpausing. We don't want to tally the time spent paused.
		this._running = value;
	}
}
