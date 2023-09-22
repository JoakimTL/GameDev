using Engine.GameLogic.ECPS;
using Engine.Structure.Attributes;
using StandardPackage.ECPS.Components;
using Engine.GlobalServices;
using Engine.GlobalServices.LoggedInput;

namespace StandardPackage.ECPS.Systems;

[Require<UnmappedInputStateComponent>]
public class UnmappedInputSystem : SystemBase {
    private readonly LoggedInputQueue _inputQueue;
    private readonly List<TimedEvent> _timedEvents;

    public UnmappedInputSystem( LoggedInputService loggedInputService ) {
        _inputQueue = loggedInputService.CreateInputQueue();
		_timedEvents = new();
    }

    public override void Update( IEnumerable<Entity> entities, float time, float deltaTime ) {
		_timedEvents.Clear();
		while (_inputQueue.TryDequeueTimedEvent(out TimedEvent? timedEvent))
			_timedEvents.Add(timedEvent);

        foreach ( Entity entity in entities )
            entity.GetOrThrow<UnmappedInputStateComponent>().SetStates( _timedEvents );
    }

	protected override void OnDispose() => _inputQueue.Dispose();
}

[Require<UnmappedInputStateComponent>]
[Require<InputCommandComponent>]
[ProcessAfter<UnmappedInputSystem, SystemBase>]
public class InputCommandSystem : SystemBase {

    public InputCommandSystem()
    {
        
    }

	public override void Update( IEnumerable<Entity> entities, float time, float deltaTime ) {
		foreach ( Entity entity in entities )
			entity.GetOrThrow<InputCommandComponent>().RegisterEvents( entity.GetOrThrow<UnmappedInputStateComponent>().TimedEvents );
	}
}