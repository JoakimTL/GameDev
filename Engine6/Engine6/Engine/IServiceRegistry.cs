namespace Engine;

public interface IServiceRegistry {
	Type? GetImplementation( Type interfaceType );
	Type? GetImplementation<T>();
	void Register<Interface, Class>() where Class : Interface;
}


