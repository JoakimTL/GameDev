using Engine.MemLib;
using System.Threading;

namespace Engine.QuickstartKit {
	public abstract class Quickstart : IRunnable {

		protected static Thread entryThread;

		public abstract bool Running { get; protected set; }

		public abstract void Run();

		/// <summary>
		/// Adds custom packets from the project this is started from
		/// </summary>
		public abstract void Packets();

		/// <summary>
		/// Starting point for the application
		/// </summary>
		public abstract void Entry();

		public static void Start( Quickstart start ) {
			MemLib.Mem.Initialize();
			entryThread = MemLib.Mem.Threads.New( start, "EntryThread", false, true );
			entryThread.Start();
			Thread.Sleep( 1000 );
		}
	}
}
