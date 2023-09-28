using System.Collections.Concurrent;

namespace Engine;

internal sealed class ServiceProvider : DependencyInjectorBase, IServiceProvider {

	private readonly ConcurrentDictionary<Type, object> _instances = new();

	public ServiceProvider( IServiceRegistry serviceRegistry ) : base( serviceRegistry ) {
		this._instances = new();
	}

	public object GetService( Type t ) => GetInternal( t );

	public T GetService<T>() where T : class => GetService( typeof( T ) ) as T ?? throw new ServiceProviderException( typeof( T ) );

	protected override object GetInternal( Type t ) {
		Type implementingType = GetImplementingType( t );
		if ( this._instances.TryGetValue( implementingType, out object? instance ) )
			return instance;
		instance = Create( implementingType, false );
		this._instances.TryAdd( t, instance );
		return instance;
	}
}


