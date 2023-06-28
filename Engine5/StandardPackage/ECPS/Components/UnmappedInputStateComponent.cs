using Engine;
using Engine.GameLogic.ECPS;
using Engine.GlobalServices.LoggedInput;

namespace StandardPackage.ECPS.Components;

public sealed class UnmappedInputStateComponent : ComponentBase {

    private readonly List<TimedKeyEventState> _keyEvents;
	private readonly List<TimedButtonEventState> _buttonEvents;
	private readonly List<TimedMousePointerState> _mousePointerStates;
	private readonly List<TimedMouseWheelState> _mouseWheelStates;
	private readonly List<TimedMouseEnterEventState> _mouseEnterEventStates;
	private readonly List<TimedMouseLockEventState> _mouseLockEventStates;

    public UnmappedInputStateComponent() {
		_keyEvents = new();
		_buttonEvents = new();
		_mousePointerStates = new();
		_mouseWheelStates = new();
		_mouseEnterEventStates = new();
		_mouseLockEventStates = new();
	}

    internal void SetStates( IEnumerable<TimedKeyEventState> keyEvents, IEnumerable<TimedButtonEventState> buttonEvents, IEnumerable<TimedMousePointerState> mousePointerStates, IEnumerable<TimedMouseWheelState> mouseWheelStates, IEnumerable<TimedMouseEnterEventState> mouseEnterEventStates, IEnumerable<TimedMouseLockEventState> mouseLockEventStates ) {
		_keyEvents.ClearThenAddRange( keyEvents );
		_buttonEvents.ClearThenAddRange( buttonEvents );
		_mousePointerStates.ClearThenAddRange( mousePointerStates );
		_mouseWheelStates.ClearThenAddRange( mouseWheelStates );
		_mouseEnterEventStates.ClearThenAddRange( mouseEnterEventStates );
		_mouseLockEventStates.ClearThenAddRange( mouseLockEventStates );
    }

	public IReadOnlyList<TimedKeyEventState> KeyEvents => _keyEvents;
	public IReadOnlyList<TimedButtonEventState> ButtonEvents => _buttonEvents;
	public IReadOnlyList<TimedMousePointerState> MousePointerStates => _mousePointerStates;
	public IReadOnlyList<TimedMouseWheelState> MouseWheelStates => _mouseWheelStates;
	public IReadOnlyList<TimedMouseEnterEventState> MouseEnterEventStates => _mouseEnterEventStates;
	public IReadOnlyList<TimedMouseLockEventState> MouseLockEventStates => _mouseLockEventStates;

}