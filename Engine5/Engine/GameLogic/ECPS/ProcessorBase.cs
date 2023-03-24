namespace Engine.GameLogic.ECPS;

public abstract class ProcessorBase {

	public readonly ComponentTypeCollection RequiredComponents;
	private readonly Type _compositeComponent;

	protected ProcessorBase( Type compositeResult, params Type[] requiredComponentTypes ) {
		RequiredComponents = new( requiredComponentTypes );
		_compositeComponent = compositeResult;
	}

	public void ProcessEntity( Entity e ) {
		if ( !e.HasAllComponents( RequiredComponents ) ) {
			e.Remove( _compositeComponent );
			return;
		}
		e.AddOrGet( _compositeComponent );
	}
}

public abstract class ProcessorBase<T> : ProcessorBase where T : ComponentBase {
	protected ProcessorBase( params Type[] requiredComponentTypes ) : base( typeof( T ), requiredComponentTypes ) { }
}
