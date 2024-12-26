using Engine.QuickstartKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CollisionDetDev3 {
	static class Program {
		/*/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.SetHighDpiMode( HighDpiMode.SystemAware );
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );
			Application.Run( new CollisionDetection() );
		}*/
		static void Main() {
			//Comment code, PLEASE
			Quickstart.Start( new Entry2() );
		}
	}
}
