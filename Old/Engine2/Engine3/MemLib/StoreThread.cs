using Engine.Utilities;
using Engine.Utilities.Data.Boxing;
using Engine.Utilities.Time;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Engine.MemLib {
	public class StoreThread {

		#region ID
		private static uint GetNextID() {
			lock( openID )
				return openID.Value++;
		}
		private static readonly MutableSinglet<uint> openID = new MutableSinglet<uint>( 0 );
		#endregion

		private volatile bool oldRunvar;
		private volatile bool running;
		public bool Running { get => running; private set => running = value; }
		public bool Identity { get; set; } = true;
		private Timer32 monitoringTimer;
		private readonly ConcurrentDictionary<string, Thread> importantThreads;
		private readonly ConcurrentDictionary<string, Thread> threads;
		private readonly ConcurrentDictionary<string, ThreadStatistics> statistics;

		public event MutableSinglet<ThreadStatistics>.SingleValueChange NewThreadStatistics;
		public event Action AllThreadsElapsed;

		public StoreThread( bool cacheCurrentThread ) {
			Running = true;
			importantThreads = new ConcurrentDictionary<string, Thread>();
			threads = new ConcurrentDictionary<string, Thread>();
			statistics = new ConcurrentDictionary<string, ThreadStatistics>();

			if( cacheCurrentThread )
				AddExisting( Thread.CurrentThread, true );
		}

		private void MonitoringEvent() {
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Running = importantThreads.Count > 0;
			if( oldRunvar != Running )
				if( !Running ) {
					Mem.Logs.Routine.WriteLine( "All important threads are closed, if this message appears at the start this is an error!", ConsoleColor.Yellow );
					monitoringTimer.Stop();
					AllThreadsElapsed?.Invoke();
				} else {
					Mem.Logs.Routine.WriteLine( "Monitoring thread has detected an important thread!", ConsoleColor.Blue );
				}
			oldRunvar = Running;

			List<string> removed = new List<string>();
			foreach( var t in threads )
				if( !t.Value.IsAlive && ( t.Value.ThreadState & ThreadState.Unstarted ) == 0 ) {
					removed.Add( t.Key );
				}

			for( int i = 0; i < removed.Count; i++ ) {
				{
					importantThreads.TryRemove( removed[ i ], out _);
				}
				{
					threads.TryRemove( removed[ i ], out _ );
				}
				statistics[ removed[ i ] ].Killed();
			}
		}

		public HashSet<ThreadStatistics> GetThreadStatistics() {
			return new HashSet<ThreadStatistics>( statistics.Values );
		}

		public Thread this[ string name ] {
			get {
					if( threads.TryGetValue( name, out Thread t ) )
						return t;
				return null;
			}
		}

		private void StartTimer() {
			if( monitoringTimer is null ) {
				monitoringTimer = new Timer32( 100 );
				monitoringTimer.Elapsed += MonitoringEvent;
			}
			if( !monitoringTimer.Enabled )
				monitoringTimer.Start();
		}

		public Thread AddExisting( Thread t, bool important = false ) {
			if( threads.ContainsKey( t.Name ) ) {
				if( Identity ) {
					t.Name += $"[{GetNextID()}]";
				} else {
					Mem.Logs.Error.WriteLine( $"Couldn't create new Thread with name [{t.Name}] because a thread with that name already exists!" );
					return null;
				}
			}

			if( important ) {
				importantThreads.TryAdd( t.Name, t );
				StartTimer();
			}
			threads.TryAdd( t.Name, t );
			ThreadStatistics ts;
			if( statistics.TryAdd( t.Name, ts = new ThreadStatistics( t, Thread.CurrentThread.Name ) ) )
				NewThreadStatistics?.Invoke( ts );

			return t;
		}

		public Thread StartNew( ThreadStart start, string name, bool background = true, bool important = false ) {
			if( threads.ContainsKey( name ) ) {
				if( Identity ) {
					name += $"[{GetNextID()}]";
				} else {
					Mem.Logs.Error.WriteLine( $"Couldn't create new Thread with name [{name}] because a thread with that name already exists!" );
					return null;
				}
			}

			Thread t = new Thread( start ) {
				Name = name,
				IsBackground = background
			};
			t.Start();
			if( important ) {
				importantThreads.TryAdd( t.Name, t );
				StartTimer();
			}
			threads.TryAdd( t.Name, t );
			ThreadStatistics ts;
			if( statistics.TryAdd( t.Name, ts = new ThreadStatistics( t, Thread.CurrentThread.Name ) ) )
				NewThreadStatistics?.Invoke( ts );
			return t;
		}

		public Thread StartNew( IRunnable runnable, string name, bool background = true, bool important = false ) => StartNew( runnable.Run, name, background, important );

		public Thread StartNew( ParameterizedThreadStart start, object obj, string name, bool background = true, bool important = false ) {
			if( threads.ContainsKey( name ) ) {
				if( Identity ) {
					name += $"[{GetNextID()}]";
				} else {
					Mem.Logs.Error.WriteLine( $"Couldn't create new Thread with name [{name}] because a thread with that name already exists!" );
					return null;
				}
			}

			Thread t = new Thread( start ) {
				Name = name,
				IsBackground = background
			};
			t.Start( obj );
			if( important ) {
				importantThreads.TryAdd( t.Name, t );
				StartTimer();
			}
			threads.TryAdd( t.Name, t );
			ThreadStatistics ts;
			if( statistics.TryAdd( t.Name, ts = new ThreadStatistics( t, Thread.CurrentThread.Name ) ) )
				NewThreadStatistics?.Invoke( ts );
			return t;
		}

		public Thread New( ThreadStart start, string name, bool background = true, bool important = false ) {
			if( threads.ContainsKey( name ) ) {
				if( Identity ) {
					name += $"[{GetNextID()}]";
				} else {
					Mem.Logs.Error.WriteLine( $"Couldn't create new Thread with name [{name}] because a thread with that name already exists!" );
					return null;
				}
			}

			Thread t = new Thread( start ) {
				Name = name,
				IsBackground = background
			};
			if( important ) {
				importantThreads.TryAdd( t.Name, t );
				StartTimer();
			}
			threads.TryAdd( t.Name, t );
			ThreadStatistics ts;
			if( statistics.TryAdd( t.Name, ts = new ThreadStatistics( t, Thread.CurrentThread.Name ) ) )
				NewThreadStatistics?.Invoke( ts );
			return t;
		}

		public Thread New( ParameterizedThreadStart start, string name, bool background = true, bool important = false ) {
			if( threads.ContainsKey( name ) ) {
				if( Identity ) {
					name += $"[{GetNextID()}]";
				} else {
					Mem.Logs.Error.WriteLine( $"Couldn't create new Thread with name [{name}] because a thread with that name already exists!" );
					return null;
				}
			}

			Thread t = new Thread( start ) {
				Name = name,
				IsBackground = background
			};
			if( important ) {
				importantThreads.TryAdd( t.Name, t );
				StartTimer();
			}
			threads.TryAdd( t.Name, t );
			ThreadStatistics ts;
			if( statistics.TryAdd( t.Name, ts = new ThreadStatistics( t, Thread.CurrentThread.Name ) ) )
				NewThreadStatistics?.Invoke( ts );
			return t;
		}

		public Thread New( IRunnable runnable, string name, bool background = true, bool important = false ) => New( runnable.Run, name, background, important );

	}

	public interface IRunnable {

		void Run();

		bool Running { get; }

	}

	public class ThreadStatistics {

		private bool isDead;
		private float lifetime;
		public float Cached { get; private set; }
		public string Name { get; private set; }
		public string ParentName { get; private set; }

		public float LifeTime {
			get {
				if( !isDead )
					lifetime = Clock32.Standard.Time - Cached;
				return lifetime;
			}
		}

		public ThreadStatistics( Thread t, string parent ) {
			Name = t.Name;
			ParentName = parent;
			Cached = Clock32.Standard.Time;
			isDead = !t.IsAlive && !( ( t.ThreadState & ThreadState.Unstarted ) > 0 );
			lifetime = 0;
		}

		public void Killed() {
			lifetime = Clock32.Standard.Time - Cached;
			isDead = true;
		}

	}
}
