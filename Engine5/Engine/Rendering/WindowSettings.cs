using Engine.Datatypes.Vectors;

namespace Engine.Rendering;

public sealed class WindowSettings : Identifiable
{
    public string windowName = "untitled window";
    public Vector2i resolution = (800, 600);
    public int vsyncLevel = 1;
    public int sampleCount = 60;
    public Mode windowMode = Mode.WINDOW;
    public nint monitor = nint.Zero;
    public Window? share = null;

    public enum Mode
    {
        WINDOW,
        FULLSCREEN,
        WINDOWEDFULLSCREEN
    }
}
