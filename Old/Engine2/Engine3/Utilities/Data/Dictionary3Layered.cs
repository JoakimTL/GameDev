using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Utilities.Data {
	public class Dictionary3Layered<TKey, TVal> {

		private Dictionary<TKey, Dictionary<TKey, Dictionary<TKey, TVal>>> dictionary;

		public Dictionary3Layered() {
			dictionary = new Dictionary<TKey, Dictionary<TKey, Dictionary<TKey, TVal>>>();
		}

		public void Remove( TKey keyA, TKey keyB, TKey keyC ) {
			lock( dictionary ) {
				if( !dictionary.TryGetValue( keyA, out Dictionary<TKey, Dictionary<TKey, TVal>> dictLayer1 ) )
					return;
				if( !dictLayer1.TryGetValue( keyB, out Dictionary<TKey, TVal> dictLayer2 ) )
					return;
				dictLayer2.Remove( keyC );
			}
		}

		public void Add( TKey keyA, TKey keyB, TKey keyC, TVal value ) {
			lock( dictionary ) {
				if( !dictionary.TryGetValue( keyA, out Dictionary<TKey, Dictionary<TKey, TVal>> dictLayer1 ) )
					dictionary.Add( keyA, dictLayer1 = new Dictionary<TKey, Dictionary<TKey, TVal>>() );
				if( !dictLayer1.TryGetValue( keyB, out Dictionary<TKey, TVal> dictLayer2 ) )
					dictLayer1.Add( keyB, dictLayer2 = new Dictionary<TKey, TVal>() );
				dictLayer2.Add( keyC, value );
			}
		}

		public void Set( TKey keyA, TKey keyB, TKey keyC, TVal value ) {
			lock( dictionary ) {
				if( !dictionary.TryGetValue( keyA, out Dictionary<TKey, Dictionary<TKey, TVal>> dictLayer1 ) )
					dictionary.Add( keyA, dictLayer1 = new Dictionary<TKey, Dictionary<TKey, TVal>>() );
				if( !dictLayer1.TryGetValue( keyB, out Dictionary<TKey, TVal> dictLayer2 ) )
					dictLayer1.Add( keyB, dictLayer2 = new Dictionary<TKey, TVal>() );
				dictLayer2[ keyC ] = value;
			}
		}

		public TVal Get( TKey keyA, TKey keyB, TKey keyC ) {
			lock( dictionary ) {
				return dictionary[ keyA ][ keyB ][ keyC ];
			}
		}

		public bool TryGet( TKey keyA, TKey keyB, TKey keyC, out TVal val ) {
			lock( dictionary ) {
				if( dictionary.TryGetValue( keyA, out Dictionary<TKey, Dictionary<TKey, TVal>> dictLayer1 ) )
					if( dictLayer1.TryGetValue( keyB, out Dictionary<TKey, TVal> dictLayer2 ) )
						return dictLayer2.TryGetValue( keyC, out val );
			}
			val = default;
			return false;
		}

	}
}
