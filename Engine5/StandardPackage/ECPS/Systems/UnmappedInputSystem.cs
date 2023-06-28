using Engine.GameLogic.ECPS;
using Engine.Structure.Attributes;
using StandardPackage.ECPS.Components;
using Engine.GlobalServices;
using Engine.GlobalServices.LoggedInput;

namespace StandardPackage.ECPS.Systems;

[Require<UnmappedInputStateComponent>]
public class UnmappedInputSystem : SystemBase {
	private readonly LoggedInputQueue _inputQueue;
    private readonly List<TimedKeyEventState> _timedKeyEventStates;
    private readonly List<TimedButtonEventState> _timedButtonEventStates;
    private readonly List<TimedMousePointerState> _timedMousePointerStates;
    private readonly List<TimedMouseWheelState> _timedMouseWheelStates;
    private readonly List<TimedMouseEnterEventState> _timedMouseEnterEventStates;
    private readonly List<TimedMouseLockEventState> _timedMouseLockEventStates;

    public UnmappedInputSystem( LoggedInputService loggedInputService ) {
		_inputQueue = loggedInputService.CreateInputQueue();
		_timedKeyEventStates = new();
		_timedButtonEventStates = new();
		_timedMousePointerStates = new();
        _timedMouseWheelStates = new();
        _timedMouseEnterEventStates = new();
        _timedMouseLockEventStates = new();
    }

	public override void Update( IEnumerable<Entity> entities, float time, float deltaTime ) {
		while (_inputQueue.TryDequeueKeyEvent(out TimedKeyEventState eventState)) 
			_timedKeyEventStates.Add(eventState);
		while (_inputQueue.TryDequeueButtonEvent(out TimedButtonEventState eventState))
            _timedButtonEventStates.Add(eventState);
		while (_inputQueue.TryDequeueMousePointerEvent(out TimedMousePointerState eventState))
			_timedMousePointerStates.Add(eventState);
		while (_inputQueue.TryDequeueMouseWheelEvent(out TimedMouseWheelState eventState))
            _timedMouseWheelStates.Add(eventState);
		while (_inputQueue.TryDequeueMouseEnterEvent(out TimedMouseEnterEventState eventState))
            _timedMouseEnterEventStates.Add(eventState);
		while(_inputQueue.TryDequeueMouseLockEvent(out TimedMouseLockEventState eventState ) ) 
			_timedMouseLockEventStates.Add(eventState);

		foreach ( Entity entity in entities )
			entity.GetOrThrow<UnmappedInputStateComponent>().SetStates( _timedKeyEventStates, _timedButtonEventStates, _timedMousePointerStates, _timedMouseWheelStates, _timedMouseEnterEventStates, _timedMouseLockEventStates );
	}

	protected override void OnDispose() {
		_inputQueue.Dispose();
	}
}