using Neural.UserInterface;

namespace Neural;

internal static class Program {
	[STAThread]
	static void Main() {
		ApplicationConfiguration.Initialize();   // sets visual styles, text rendering, DPI, etc.
		Application.Run( new MainWindow() );            // show your first Form
	}
}
