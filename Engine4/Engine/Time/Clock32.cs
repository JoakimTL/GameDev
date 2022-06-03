using System.Diagnostics;

namespace Engine.Time;

public class Clock32 : Identifiable {

	private static readonly float _invFreq = 1f / Stopwatch.Frequency;

	private float _talliedTime, _lastPause;
	private float _speed;
	private bool _paused;

	public Clock32( float speed = 1 ) {
		this._speed = speed;
		this._lastPause = 0;
		this._talliedTime = 0;

		this.Paused = false;
	}

	/// <summary>
	/// A simple wrapper that returns the System.Diagnostics.Stopwatch.GetTimestamp() in seconds. Considering this number might be large and inaccurate, it might be wise to use <see cref="StartupTime"/>.
	/// </summary>
	public static float SystemTime => Stopwatch.GetTimestamp() * _invFreq;

	/// <summary>
	/// The time since startup. This clock always starts at 0.
	/// </summary>
	public static float StartupTime => Clock64.Watch.ElapsedTicks * _invFreq;

	/// <summary>
	/// The total time this clock has tallied. This accounts for changes in clock speed and might not be the actual real time this clock has spent active.
	/// </summary>
	public float Time => GetTotalTime();
	/// <summary>
	/// The time spent in the current session. Session changes happen when changing the speed of the clock or unpausing.
	/// </summary>
	public float Session => GetSessionTime();
	/// <summary>
	/// The speed of the clock. This is how many seconds the clock tallies for each real second.
	/// </summary>
	public float Speed { get => this._speed; set => SetSpeed( value ); }
	/// <summary>
	/// Whether the clock is tallying or not. Useful for stopping a system reliant on timekeeping (say a game-world).
	/// </summary>
	public bool Paused { get => this._paused; set => SetState( value ); }

	private float GetTotalTime() {
		if ( !this._paused )
			return this._talliedTime + GetSessionTime();
		return this._talliedTime;
	}

	private float GetSessionTime() => ( StartupTime - this._lastPause ) * this._speed;

	/// <summary>
	/// Changes the speed of the clock.
	/// </summary>
	/// <param name="value">How many seconds the clock tallies per real-time seconds.</param>
	private void SetSpeed( float value ) {
		this._talliedTime += GetSessionTime();
		this._speed = value;
		if ( !this._paused )
			//Unpaused!
			this._lastPause = StartupTime;
	}

	/// <summary>
	/// Changes the running state of the clock.
	/// </summary>
	/// <param name="value">True means the clock continues running, false means the clock will be paused.</param>
	private void SetState( bool value ) {
		if ( value ) {
			//Paused!
			this._talliedTime += GetSessionTime();
		} else {
			//Unpaused!
			this._lastPause = StartupTime;
		}
		this._paused = value;
	}

}
