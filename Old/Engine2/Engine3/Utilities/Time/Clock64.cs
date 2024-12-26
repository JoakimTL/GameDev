using Engine.Utilities.Data.Boxing;
using System.Diagnostics;

namespace Engine.Utilities.Time {
	public class Clock64 {

		private static readonly Stopwatch internalWatch = Stopwatch.StartNew();
		private static readonly double inv_freq = 1d / Stopwatch.Frequency;

		private readonly bool canChangeSpeed;
		private readonly bool canBePaused;

		private bool paused;
		private double lastPause, talliedTime;

		public bool Paused {
			get {
				return paused;
			}
			set {
				SetPaused( value );
			}
		}
		public MutableSinglet<double> Speed {
			get; private set;
		}
		public double Time {
			get {
				return talliedTime + SessionTime * ( Paused ? 0 : 1 );
			}
		}
		private double SessionTime {
			get {
				return ( internalWatch.ElapsedTicks * inv_freq - lastPause ) * Speed.Value;
			}
		}

		public Clock64( double speed, bool canChangeSpeed = true, bool canBePaused = true ) {
			this.canBePaused = canBePaused;
			this.canChangeSpeed = canChangeSpeed;
			this.Speed = new MutableSinglet<double>( speed, SpeedChangeCondition );

			lastPause = 0;
			talliedTime = 0;

			Paused = false;
		}

		public Clock64( bool canChangeSpeed = true, bool canBePaused = true ) : this( 1, canChangeSpeed, canBePaused ) { }

		private void SetPaused( bool val ) {
			if( !canBePaused )
				return;

			if( val ) {
				talliedTime += SessionTime;
			} else {
				lastPause = internalWatch.ElapsedTicks * inv_freq;
			}
			paused = val;
		}

		private bool SpeedChangeCondition( double newSpeed ) {
			return newSpeed >= 0 && canChangeSpeed;
		}

		public void SetTime( double t ) {
			talliedTime = t;
		}

		public readonly static Clock64 Standard = new Clock64( false, false );

	}
}
