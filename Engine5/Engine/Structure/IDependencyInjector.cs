namespace Engine.Structure;

public interface IDependencyInjector {
	object? Get( Type type );
}
