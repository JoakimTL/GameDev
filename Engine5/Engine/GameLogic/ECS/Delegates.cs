namespace Engine.GameLogic.ECS;

public delegate void EntityParentIdEvent( Entity child, Guid? parent );
public delegate void EntityParentEvent( Entity child, Entity? parent );
public delegate void EntityComponentEvent( ComponentBase component );
