using Engine.MemLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Utilities.Time {
	public class Timer32 {

		private static int timerNum = 0;
		public string Name { get; private set; }
		private int interval;
		public int Interval { get => interval; set => SetInterval( value ); }
		private int remainingInterval;
		public long startTime;
		public long cumilativeTime;
		public int step;
		private bool newInterval;

		public bool Enabled { get; private set; }

		private AutoResetEvent startEvent;
		private AutoResetEvent resetEvent;
		private Thread timerThread;

		public event Action Elapsed;

		public Timer32( string name, int interval ) {
			Name = name;
			resetEvent = new AutoResetEvent( false );
			startEvent = new AutoResetEvent( false );
			Enabled = false;
			this.interval = interval;
			timerThread = Mem.Threads.StartNew( InternalTimerCallback, name + " Thread" );
			Mem.Threads.AllThreadsElapsed += Stop;
		}

		public Timer32( string name ) : this( name, 1000 ) { }

		public Timer32( int interval ) : this( $"Timer[{timerNum++}]", interval ) { }

		public Timer32() : this( $"Timer[{timerNum++}]", 1000 ) { }

		private void InternalTimerCallback() {
			while( Mem.Threads.Running ) {
				startEvent.WaitOne();
				startEvent.Reset();
				cumilativeTime = 0;
				step = 0;
				startTime = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond;
				remainingInterval = interval;
				while( Enabled ) {
					resetEvent.WaitOne( remainingInterval );
					if( !Enabled ) {
						resetEvent.Reset();
						break;
					}
					if( newInterval ) {
						startEvent.Set();
						newInterval = false;
						break;
					}
					new Task( () => Elapsed?.Invoke() ).Start();
					cumilativeTime += remainingInterval;
					step++;
					long timeEnd = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond;
					remainingInterval = interval - (int) ( timeEnd - ( startTime + interval * step ) );
					if( remainingInterval < 0 )
						remainingInterval = 0;
				}
			}
		}

		public void Start( int ms ) {
			if( ms <= 0 )
				return;
			interval = ms;
			Enabled = true;
			startEvent.Set();
		}

		public void Start() {
			Enabled = true;
			startEvent.Set();
		}

		public void Stop() {
			Enabled = false;
			resetEvent.Set();
		}

		public void SetInterval( int ms ) {
			if( ms <= 0 )
				return;
			interval = ms;
			if( Enabled ) {
				newInterval = true;
				resetEvent.Set();
			}
		}

	}
}
