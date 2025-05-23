﻿namespace Engine.Module.Entities.Container;

public delegate void EntityListChangedHandler( Entity entity );
public delegate void ComponentChangeHandler( ComponentBase component );
public delegate void ComponentListChangedHandler( Entity entity, ComponentBase component );
public delegate void EntityRelationChangedHandler( Entity entity, Entity? oldParent, Entity? newParent );
public delegate void ParentIdChanged( Entity entity );
public delegate void EntityArchetypeChangeHandler( ArchetypeBase archetype );