using Engine.Utilities;
using Engine.Utilities.Data.Boxing;
using System.Collections.Concurrent;
using System.Threading;

namespace Engine.MemLib {
	public class StoreThreadpool {

		#region ID
		private static uint GetNextID() {
			lock( openID )
				return openID.Value++;
		}
		private static readonly MutableSinglet<uint> openID = new MutableSinglet<uint>( 0 );
		#endregion

		private readonly ConcurrentDictionary<uint, Thread> threads;
		private readonly ConcurrentQueue<Thread> unstarted;
		private readonly AutoResetEvent newThreadEvent;

		public StoreThreadpool() {
			threads = new ConcurrentDictionary<uint, Thread>();
			unstarted = new ConcurrentQueue<Thread>();
			newThreadEvent = new AutoResetEvent( false );
			Mem.Threads.StartNew( Maintain, "PoolMaintenance" );
		}

		private void Maintain() {
			while( Mem.Threads.Running ) {
				newThreadEvent.WaitOne();
				while( unstarted.TryDequeue( out Thread t ) )
					t.Start();
			}
		}

		public Thread this[ uint id ] {
			get {
				if( threads.TryGetValue( id, out Thread t ) )
					return t;
				return null;
			}
		}

		public uint Start( ThreadStart start ) {
			uint id = GetNextID();
			Thread t = new Thread( start ) {
				Name = $"Pool[#{id}]",
				IsBackground = true
			};
			t.Start();
			threads.TryAdd( id, t );
			return id;
		}

		public uint StartAsync( ThreadStart start ) {
			uint id = GetNextID();
			Thread t = new Thread( start ) {
				Name = $"Pool[#{id}]",
				IsBackground = true
			};
			threads.TryAdd( id, t );
			unstarted.Enqueue( t );
			newThreadEvent.Set();
			return id;
		}
	}
}
