namespace Engine.Module.Entities;

public interface ICleanupController {
	bool ShouldBeRemoved { get; }
}

public sealed class ComponentNotFoundException( Type componentType ) : Exception( $"Couldn't find component of type {componentType.Name}." );