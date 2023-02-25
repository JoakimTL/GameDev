using Engine.GlobalServices;

namespace Engine.GameLogic.ECPS;

public sealed class EntityProcessorContainerService : Identifiable, IGameLogicService {

	private readonly Dictionary<Type, List<ProcessorBase>> _processorsByComponentType;
	private readonly EntityContainerService _entityContainerService;

	public EntityProcessorContainerService( EntityContainerService entityContainerService ) {
		_entityContainerService = entityContainerService;

		_processorsByComponentType = new();
		SetupProcessors();
		_entityContainerService._container.ComponentAdded += OnComponentChange;
		_entityContainerService._container.ComponentRemoved += OnComponentChange;
	}

	private void SetupProcessors() {
		var processorTypes = Global.Get<TypeService>().ImplementationTypes.Where( p => p.IsAssignableTo( typeof( ProcessorBase ) ) );
		foreach ( var processorType in processorTypes )
			if ( processorType.GetInjectedInstance() is ProcessorBase processor )
				foreach ( var componentType in processor.RequiredComponents.ComponentTypes ) {
					if ( !_processorsByComponentType.TryGetValue( componentType, out var list ) )
						_processorsByComponentType.Add( componentType, list = new() );
					list.Add( processor );
				}
	}

	private void OnComponentChange( ComponentBase component ) {
		if ( component.Owner is null )
			return;
		var type = component.GetType();
		if ( _processorsByComponentType.TryGetValue( type, out var list ) )
			for ( int i = 0; i < list.Count; i++ )
				list[ i ].ProcessEntity( component.Owner );
	}
}