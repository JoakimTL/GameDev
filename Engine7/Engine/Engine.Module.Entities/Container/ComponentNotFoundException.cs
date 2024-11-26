namespace Engine.Module.Entities.Container;

public sealed class ComponentNotFoundException( Type componentType ) : Exception( $"Couldn't find component of type {componentType.Name}." );