using Engine.MemLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine {
	public static class Logging {

		public static void Routine( object o ) {
			if( !Mem.Initialized )
				return;
			Mem.Logs.Routine.WriteLine( o );
		}

		public static void Warning( object o ) {
			if( !Mem.Initialized )
				return;
			Mem.Logs.Warning.WriteLine( o );
		}

		public static void Error( object o ) {
			if( !Mem.Initialized )
				return;
			Mem.Logs.Error.WriteLine( o );
		}

		/// <summary>
		/// When allocating data. This log only appears on the log itself, not in the console!
		/// </summary>
		/// <param name="o"></param>
		public static void Memory( object o ) {
			if( !Mem.Initialized )
				return;
			Mem.Logs.MemoryLogger.WriteLine( o );
		}

	}
}
