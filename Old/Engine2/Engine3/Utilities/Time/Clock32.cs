using Engine.Utilities.Data.Boxing;
using System.Diagnostics;

namespace Engine.Utilities.Time {
	public class Clock32 {

		private static readonly Stopwatch internalWatch = Stopwatch.StartNew();
		private static readonly float inv_freq = 1f / Stopwatch.Frequency;

		private readonly bool canChangeSpeed;
		private readonly bool canBePaused;

		private bool paused;
		private float lastPause, talliedTime;

		public bool Paused {
			get {
				return paused;
			}
			set {
				SetPaused( value );
			}
		}
		public MutableSinglet<float> Speed {
			get; private set;
		}
		public float Time {
			get {
				return talliedTime + SessionTime * ( Paused ? 0 : 1 );
			}
		}
		private float SessionTime {
			get {
				return ( internalWatch.ElapsedTicks * inv_freq - lastPause ) * Speed.Value;
			}
		}

		public Clock32( float speed, bool canChangeSpeed = true, bool canBePaused = true ) {
			this.canBePaused = canBePaused;
			this.canChangeSpeed = canChangeSpeed;
			this.Speed = new MutableSinglet<float>( speed, SpeedChangeCondition );

			lastPause = 0;
			talliedTime = 0;

			Paused = false;
		}

		public Clock32( bool canChangeSpeed = true, bool canBePaused = true ) : this( 1, canChangeSpeed, canBePaused ) { }

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

		private bool SpeedChangeCondition( float newSpeed ) {
			return newSpeed >= 0 && canChangeSpeed;
		}

		public void SetTime( float t ) {
			talliedTime = t;
		}

		public readonly static Clock32 Standard = new Clock32( false, false );

	}
}
