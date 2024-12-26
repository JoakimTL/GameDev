using Engine.Utilities.Data.Boxing;

namespace Engine.Utilities.Time {
	public class Watch32 {

		public MutableSinglet<Clock32> Clock {
			get; private set;
		}

		public float TimePrevious { get; private set; }
		public float TimeCurrent { get; private set; }

		/// <summary>
		/// Returns the time between now and the last Zero()
		/// </summary>
		public float ElapsedTime {
			get {
				return Clock.Value.Time - TimeCurrent;
			}
		}

		/// <summary>
		/// Returns the time between the last two Zero()s
		/// </summary>
		public float DeltaTime {
			get {
				return TimeCurrent - TimePrevious;
			}
		}

		public Watch32( Clock32 c ) {
			this.Clock = new MutableSinglet<Clock32>( c, ClockChangeCondition );
			this.Clock.Changed += ClockChangeHandler;

			TimePrevious = c.Time;
			TimeCurrent = TimePrevious;
		}

		public Watch32( MutableSinglet<Clock32> c ) {
			this.Clock = c;
			this.Clock.Set( ClockChangeCondition );
			this.Clock.Changed += ClockChangeHandler;

			TimePrevious = c.Value.Time;
			TimeCurrent = TimePrevious;
		}

		public void Zero() {
			TimePrevious = TimeCurrent;
			TimeCurrent = Clock.Value.Time;
		}

		public void Reset() {
			TimePrevious = Clock.Value.Time;
			TimeCurrent = TimePrevious;
		}

		public bool ClockChangeCondition( Clock32 n ) {
			return n != null;
		}

		public void ClockChangeHandler( Clock32 old ) {
			Reset();
		}

	}
}
