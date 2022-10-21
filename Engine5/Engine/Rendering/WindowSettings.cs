using Engine.Datatypes;
using Engine.Rendering.Objects;
using GLFW;

namespace Engine.Rendering;

public sealed class WindowSettings : Identifiable {
	public string windowName = "untitled window";
	public Vector2i resolution = (800, 600);
	public int vsyncLevel = 1;
	public int sampleCount = 60;
	public Mode windowMode = Mode.WINDOW;
	public MonitorPtr monitor = MonitorPtr.None;
	public Window? share = null;

	public enum Mode {
		WINDOW,
		FULLSCREEN,
		WINDOWEDFULLSCREEN
	}
}
