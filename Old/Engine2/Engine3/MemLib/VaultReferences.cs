using Engine.Utilities.Data;
using Engine.Utilities.Data.Boxing;
using Engine.Utilities.IO;
using Engine.Utilities.Time;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Engine.MemLib {
	public class VaultReferences {

		#region ID
		private static uint GetNextID() {
			lock( openID )
				return openID.Value++;
		}
		private static readonly MutableSinglet<uint> openID = new MutableSinglet<uint>( 0 );
		#endregion

		private Log refIdLogger;

		private readonly ConcurrentDictionary<uint, WeakReference<Cacheable>> objects;
		private readonly Timer32 scanTimer;

		public delegate void CachedObjectRemovalHandler( uint id );
		public event CachedObjectRemovalHandler ObjectRemoved;

		private uint scanId;

		public VaultReferences() {
			objects = new ConcurrentDictionary<uint, WeakReference<Cacheable>>();
			scanTimer = new Timer32( 1000 );
			scanTimer.Elapsed += ScanFunc;
		}

		public void Initialize() {
			if( refIdLogger is null ) {
				refIdLogger = new Log( "refIds" ) { DefaultFlags = LogFlags.QUIET };
				refIdLogger.WriteLine( $"Log created, added [{refIdLogger}]" );
				scanTimer.Start();
			}
		}

		private void ScanFunc() {
			if( objects.Count > 0 ) {
				if( objects.TryGetValue( scanId, out WeakReference<Cacheable> oRefContainer ) ) {
					if( !oRefContainer.TryGetTarget( out _ ) ) {
						refIdLogger.WriteLine( $"[{scanId}] REMOVED" );
						ObjectRemoved?.Invoke( scanId );
						if( !objects.TryRemove( scanId, out _ ) )
							Logging.Error( $"[{scanId}] Unable to remove unreferenced object." );
					}
				}
				scanId++;
				scanId = (uint) ( scanId % objects.Count );
				scanTimer.Interval = (int) System.Math.Min( System.Math.Max( 2000 * System.Math.Pow( 2, -System.Math.Log( objects.Count, 2 ) ), 1 ), 2000 );
			}
		}

		public T TryGet<T>( uint id ) where T : Cacheable {
			if( objects.TryGetValue( id, out WeakReference<Cacheable> oRefContainer ) ) {
				if( oRefContainer.TryGetTarget( out Cacheable o ) ) {
					if( o is T ) {
						return (T) o;
					} else {
						Mem.Logs.Error.WriteLine( $"Tried to fetch cached object into type {typeof( T ).Name}, but object is of type {o.GetType().Name}." );
					}
				} else {
					Mem.Logs.Error.WriteLine( "Tried to fetch cached object, but said object was already disposed of." );
				}
			} else {
				Mem.Logs.Error.WriteLine( "Tried to fetch cached object, but none could be found." );
			}
			return default;
		}

		public uint Add( Cacheable o ) {
			uint availableId = GetNextID();
			WeakReference<Cacheable> _ref = new WeakReference<Cacheable>( o );
			if( !objects.TryAdd( availableId, _ref ) )
				Logging.Error( "Unable to add object to references." );
			if( !( refIdLogger is null ) )
				refIdLogger.WriteLine( $"Added [{o}] with RefID [{availableId}]" );

			return availableId;
		}

		public void Dispose() {
			foreach( var idRefPair in objects ) {
				if( idRefPair.Value.TryGetTarget( out Cacheable o ) ) {
					if( !( o is null ) ) {
						refIdLogger.WriteLine( $"Disposing [{idRefPair.Key}]!" );
						o.Dispose();
					}
				}
			}
		}

	}
}
