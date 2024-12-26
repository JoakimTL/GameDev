using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Utilities.Data.Boxing {
	public class ImmutableDuo<T1, T2> {

		public T1 ValueA { get; private set; }
		public T2 ValueB { get; private set; }

		public ImmutableDuo( T1 a, T2 b ) {
			ValueA = a;
			ValueB = b;
		}

	}
}
