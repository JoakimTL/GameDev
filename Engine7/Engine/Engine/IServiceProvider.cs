namespace Engine;

public interface IServiceProvider {
	event Action<object>? OnInstanceAdded;
	object Get( Type t );
}
