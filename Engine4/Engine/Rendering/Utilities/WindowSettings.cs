using Engine.Data.Datatypes;
using GLFW;

namespace Engine.Rendering.Utilities;
public class WindowSettings : Identifiable {
	public string windowName = "untitled window";
	public Vector2i resolution = (800, 600);
	public int vsyncLevel = 1;
	public int sampleCount = 60;
	public Mode windowMode = Mode.WINDOW;
	public MonitorPtr monitor = MonitorPtr.None;
	public Window? share = null;

	public WindowSettings() {

	}

	public enum Mode {
		WINDOW,
		FULLSCREEN,
		WINDOWEDFULLSCREEN
	}
}
