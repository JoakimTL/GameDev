using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utilities.Data.Boxing {
	public class MutableSinglet<T> {

		private T wrappedValue;

		public T Value {
			get => wrappedValue;
			set => Set( value );
		}

		public bool Locked { get; private set; }

		public event SingleValueChange<T> Changed;
		public SingleValueConditional<T> Condition { get; private set; }

		public MutableSinglet( T val, SingleValueConditional<T> conditional, bool locked = false ) {
			this.Condition = conditional;
			wrappedValue = val;
			Locked = locked;
		}

		public MutableSinglet( T val ) : this( val, delegate ( T v ) { return true; } ) { }

		public bool Set( T val ) {
			if( ( val == null || !val.Equals( wrappedValue ) ) && Condition( val ) ) {
				var oldVlaue = wrappedValue;
				wrappedValue = val;
				Changed?.Invoke( oldVlaue );
				return true;
			}
			return false;
		}

		public void Set( SingleValueConditional<T> con ) {
			if( con != null && !Locked )
				Condition = con;
		}

		public void Lock() {
			Locked = true;
		}

	}
}
