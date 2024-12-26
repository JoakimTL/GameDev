using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.MemLib {
	public abstract class Cache {

		protected VaultReferences ReferenceVault { get; private set; }

		public Cache( VaultReferences refVault ) {
			ReferenceVault = refVault;
			refVault.ObjectRemoved += ReferenceDestroyed;
		}

		public abstract void ReferenceDestroyed( uint refId );

	}

	public abstract class Cache<ACCESSORTYPE, OBJECTTYPE> : Cache where OBJECTTYPE : Cacheable {

		public bool HandlesNewAccessKeys { get; private set; }
		protected Dictionary<uint, ACCESSORTYPE> cacheAccessorDictionary;
		protected Dictionary<ACCESSORTYPE, uint> cacheReferenceDictionary;

		public Cache( VaultReferences refVault, bool handlesNew ) : base( refVault ) {
			HandlesNewAccessKeys = handlesNew;
			cacheAccessorDictionary = new Dictionary<uint, ACCESSORTYPE>();
			cacheReferenceDictionary = new Dictionary<ACCESSORTYPE, uint>();
		}

		public OBJECTTYPE Get( ACCESSORTYPE key ) {
			if( cacheReferenceDictionary.TryGetValue( key, out uint refId ) )
				return ReferenceVault.TryGet<OBJECTTYPE>( refId );
			if( HandlesNewAccessKeys )
				return HandleNewObject( key );
			return UnhandledNewObject( );
		}

		protected OBJECTTYPE Add( ACCESSORTYPE key, OBJECTTYPE o ) {
			if( cacheReferenceDictionary.ContainsKey( key ) )
				return o;
			lock( cacheReferenceDictionary ) {
				cacheReferenceDictionary.Add( key, o.RefID );
				cacheAccessorDictionary.Add( o.RefID, key );
			}
			return o;
		}

		public override sealed void ReferenceDestroyed( uint refId ) {
			if( cacheAccessorDictionary.TryGetValue( refId, out ACCESSORTYPE key ) ) {
				lock( cacheReferenceDictionary ) {
					cacheReferenceDictionary.Remove( key );
					cacheAccessorDictionary.Remove( refId );
				}
			}
		}

		protected OBJECTTYPE UnhandledNewObject() {
			return default;
		}

		protected abstract OBJECTTYPE HandleNewObject( ACCESSORTYPE key );
	}
}
