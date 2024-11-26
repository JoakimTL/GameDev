namespace Engine.Module.Entities.Container;

public abstract class ArchetypeBase {
	private Entity? _entity;
	public Entity Entity => this._entity ?? throw new InvalidOperationException( "Archetype is not attached to an entity." );
	internal void SetEntity( Entity e ) {
		this._entity = e;
	}
}