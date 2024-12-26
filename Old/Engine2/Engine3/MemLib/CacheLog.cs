using Engine.Utilities.IO;
using System.Collections.Concurrent;

namespace Engine.MemLib {
	public class CacheLog : Cache<string, Log> {

		public Log Routine { get; private set; }
		public Log Warning { get; private set; }
		public Log SilentException { get; private set; }
		public Log MemoryLogger { get; private set; }
		public Log Error { get; private set; }

		public CacheLog( VaultReferences refVault ) : base( refVault, true ) {
			Add( "routine", Routine = new Log( "routine" ) { DefaultColor = System.ConsoleColor.White } );
			Add( "warning", Warning = new Log( "warning" ) { DefaultColor = System.ConsoleColor.Yellow } );
			Add( "memlog", MemoryLogger = new Log( "memlog" ) { DefaultFlags = LogFlags.QUIET } );
			Add( "error", Error = new Log( "error" ) { DefaultColor = System.ConsoleColor.Red, DefaultFlags = LogFlags.STACK } );
			Add( "exceptions", SilentException = new Log( "exceptions" ) { DefaultFlags = LogFlags.QUIET } );
		}

		protected override Log HandleNewObject( string key ) {
			return Add( key, new Log( key ) );
		}
	}
}
