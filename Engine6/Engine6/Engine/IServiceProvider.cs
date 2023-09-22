namespace Engine;

public interface IServiceProvider {
	object GetService( Type t );
	T GetService<T>() where T : class;
}
