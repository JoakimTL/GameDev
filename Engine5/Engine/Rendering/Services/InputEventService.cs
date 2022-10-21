using Engine.Rendering.Input;
using Engine.Rendering.Objects;

namespace Engine.Rendering.Services;

public sealed class InputEventService : IContextService {

	public MouseInputEventManager Mouse { get; }
	public KeyboardInputEventManager Keyboard { get; }
	public WindowInputEventManager Window { get; }

	public InputEventService( Window window) {
        Mouse = new( window );
		Keyboard = new( window );
		Window = new( window );
	}
}
