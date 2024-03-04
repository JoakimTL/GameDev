namespace Engine;

public interface IServiceProvider {
	public delegate void ServiceEventHandler( object service );

	event ServiceEventHandler? ServiceAdded;

	/// <summary>
	/// Adds a constant value to the service provider. This value may be values not usually instantiated by the service provider, but might be used by other services.
	/// </summary>
	/// <typeparam name="T">The type to add this instance as.</typeparam>
	/// <param name="instance">The instance to add as constant.</param>
	void AddConstant<T>( T instance ) where T : class;
	object GetService( Type t );
	T GetService<T>() where T : class;
}
