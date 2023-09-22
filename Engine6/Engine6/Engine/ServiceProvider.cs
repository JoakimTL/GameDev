using System.Collections.Concurrent;

namespace Engine;

internal sealed class ServiceProvider : DependencyInjectorBase, IServiceProvider {

	private readonly IServiceRegistry _serviceRegistry;

	private readonly ConcurrentDictionary<Type, object> _instances = new();

	public ServiceProvider( IServiceRegistry serviceRegistry ) {
		this._serviceRegistry = serviceRegistry ?? throw new ArgumentNullException( nameof( serviceRegistry ) );
		this._instances = new();
	}

	public object GetService( Type t ) {
		Type originalType = t;
		if ( t.IsValueType )
			throw new ServiceProviderException( t, "Value types can't be services.");
		if (t.IsAbstract )
			t = this._serviceRegistry.GetImplementation( t ) ?? throw new ServiceProviderException( originalType, "No registered type for abstract class found." );
		if ( t.IsInterface )
			t = this._serviceRegistry.GetImplementation( t ) ?? throw new ServiceProviderException( originalType, "No registered type for interface found." );

		if ( this._instances.TryGetValue( t, out object? instance ) )
			return instance;

		instance = Activator.CreateInstance( t ) ?? throw new ServiceProviderException( originalType, "Unable to create instance." );
		this._instances.TryAdd( t, instance );
		//What about multiple interfaces registering the same class?
		return instance;
	}

	public T GetService<T>() where T : class => GetService( typeof( T ) ) as T ?? throw new ServiceProviderException( typeof( T ) );
}


