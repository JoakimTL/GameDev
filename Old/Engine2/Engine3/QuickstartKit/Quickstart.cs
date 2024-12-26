using Engine.MemLib;
using System.Threading;

namespace Engine.QuickstartKit {
	public abstract class Quickstart : IRunnable {

		protected static Thread entryThread;

		public abstract bool Running { get; protected set; }

		public abstract void Run();

		/// <summary>
		/// Starting point for the application
		/// </summary>
		public abstract void Entry();

		public static void Start( Quickstart start ) {
			Mem.Initialize();
			entryThread = Mem.Threads.New( start, "EntryThread", false, true );
			entryThread.Start();
			Thread.Sleep( 1000 );
		}
	}
}
