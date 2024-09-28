namespace Engine.Modules.ECS;

public sealed class SystemEntityContainer( EntityManager entityManager, SystemBase system ) : EntityContainerBase( entityManager ) {
	private readonly SystemBase _system = system;
	private readonly HashSet<Entity> _eligibleEntities = [];

	public IReadOnlyCollection<Entity> EligibleEntities => _eligibleEntities;

	public override void Dispose() { }

	protected internal override void AddAll( IEnumerable<Entity> allEntities ) {
		foreach (Entity e in allEntities)
			if (e.HasAllComponents( _system.RequiredTypes ))
				_eligibleEntities.Add( e );
	}

	protected override void ComponentAdded( Entity e, ComponentBase component ) {
		if (!_system.RequiredTypes.Contains( component.GetType() ))
			return;
		if (e.HasAllComponents( _system.RequiredTypes ))
			_eligibleEntities.Add( e );
	}

	protected override void ComponentRemoved( Entity e, ComponentBase component ) {
		if (!_system.RequiredTypes.Contains( component.GetType() ))
			return;
		_eligibleEntities.Remove( e );
	}
}
