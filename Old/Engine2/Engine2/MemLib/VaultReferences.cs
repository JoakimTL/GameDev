using Engine.Utilities.Data;
using Engine.Utilities.IO;
using Engine.Utilities.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Engine.MemLib {
	public class VaultReferences {

		public const uint SIZE_PER_BITSET_CHUNK = 4096;

		private Log refIdLogger;

		private readonly BitSet bitset;
		private readonly Dictionary<uint, WeakReference<Cacheable>> objects;
		private readonly Timer32 scanTimer;

		public delegate void CachedObjectRemovalHandler( uint id );
		public event CachedObjectRemovalHandler ObjectRemoved;

		private uint scanId;

		public VaultReferences() {
			bitset = new BitSet( (int) SIZE_PER_BITSET_CHUNK );
			objects = new Dictionary<uint, WeakReference<Cacheable>>();
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
						lock( objects ) {
							objects.Remove( scanId );
							bitset.Clear( (int) scanId );
						}
					}
				}
				if( bitset.Max > 0 )
					scanId = ( scanId + 1 ) % ( (uint) bitset.Max );
				scanTimer.Interval = (int) System.Math.Min( System.Math.Max( 2000 * System.Math.Pow( 2, -System.Math.Log( objects.Count, 2 ) ), 1 ), 2000 );
			}
		}

		public T TryGet<T>( uint id ) where T : Cacheable {
			if( objects.TryGetValue( id, out WeakReference<Cacheable> oRefContainer ) ) {
				if( oRefContainer.TryGetTarget( out Cacheable o ) ) {
					//oRefContainer.SetLastAccess( Clock32.Standard.Time );
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
			lock( objects ) {
				if( bitset.Min == -1 )
					bitset.Resize( bitset.Size + (int) SIZE_PER_BITSET_CHUNK );
				uint availableId = (uint) ( bitset.Min );
				bitset.Set( (int) availableId );
				objects.Add( availableId, new WeakReference<Cacheable>( o ) );
				if( !( refIdLogger is null ) )
					refIdLogger.WriteLine( $"Added [{o}] with RefID [{availableId}]" );

				return availableId;
			}
		}

		public void Dispose() {
			lock( objects ) {
				foreach( var idRefPair in objects ) {
					if( idRefPair.Value.TryGetTarget( out Cacheable o ) ) {
						IDisposable disp = o as IDisposable;
						if( !( disp is null ) ) {
							refIdLogger.WriteLine( $"Disposing [{idRefPair.Key}]!" );
							disp.Dispose();
						}
					}
				}
			}
		}

	}
}
