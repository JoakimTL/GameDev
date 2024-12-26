using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utilities.Data.Boxing {
	public class MutableMemorySinglet<T> : MutableSinglet<T> {

		public T OldValue { get; private set; }

		public MutableMemorySinglet( T val, SingleValueConditional<T> conditional, bool locked = false ) : base( val, conditional, locked ) {
			OldValue = default;
			Changed += ValueChanged;
		}

		public MutableMemorySinglet( T val ) : this( val, delegate ( T v ) { return true; } ) { }

		private void ValueChanged( T oldValue ) {
			OldValue = oldValue;
		}

		public void ClearOldValue() {
			OldValue = default;
		}

	}
}
