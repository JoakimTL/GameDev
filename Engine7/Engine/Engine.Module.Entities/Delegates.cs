namespace Engine.Module.Entities;

public delegate void EntityListChangedHandler( Entity entity );
public delegate void ComponentChangeHandler( ComponentBase component );
public delegate void ComponentListChangedHandler( ComponentBase component );
public delegate void EntityRelationChangedHandler( Entity entity, Entity? oldParent, Entity? newParent );
public delegate void ParentIdChanged( Entity entity );