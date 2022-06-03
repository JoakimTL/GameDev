using Engine.Modularity.ECS.Components;

namespace Engine.Modularity.ECS.Organization;
public abstract class EntityTrackerBase<R> : Identifiable where R : Component {

	protected readonly EntityManager _manager;
	private readonly ComponentOrganizer<R> _renderComponentOrganizer;

	public EntityTrackerBase( EntityManager manager ) {
		this._manager = manager;
		this._renderComponentOrganizer = new ComponentOrganizer<R>( manager );
		this._renderComponentOrganizer.OnComponentAdded += ComponentAdded;
		this._renderComponentOrganizer.OnComponentRemoved += ComponentRemoved;
		this._renderComponentOrganizer.OnComponentChanged += ComponentChanged;
	}

	protected abstract void ComponentAdded( Entity e, R c );
	protected abstract void ComponentRemoved( Entity e, R c );
	protected abstract void ComponentChanged( Entity e, R c );
}

public class EntityTransform3Tracker : EntityTrackerBase<Transform3Component> {
	public EntityTransform3Tracker( EntityManager manager ) : base( manager ) {

	}

	protected override void ComponentAdded( Entity e, Transform3Component c ) => throw new NotImplementedException();
	protected override void ComponentChanged( Entity e, Transform3Component c ) => throw new NotImplementedException();
	protected override void ComponentRemoved( Entity e, Transform3Component c ) => throw new NotImplementedException();
}
