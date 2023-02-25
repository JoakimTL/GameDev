using Engine.Rendering.Contexts.Input;

namespace Engine.Rendering.Contexts.Services;

public sealed class InputEventService : IContextService
{

    public MouseInputEventManager Mouse { get; }
    public KeyboardInputEventManager Keyboard { get; }
    public WindowInputEventManager Window { get; }

    public InputEventService(Window window)
    {
        Mouse = new(window);
        Keyboard = new(window);
        Window = new(window);
    }
}
