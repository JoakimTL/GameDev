using System.Collections.Concurrent;

namespace Engine;

internal sealed class ServiceProvider( IServiceRegistry serviceRegistry ) : DependencyInjectorBase( serviceRegistry ), IServiceProvider {

	private readonly ConcurrentDictionary<Type, object> _instances = new();
	private readonly ConcurrentDictionary<Type, object> _constants = new();

	public event IServiceProvider.ServiceEventHandler? ServiceAdded;

	public void AddConstant<T>( T instance ) where T : class {
		ArgumentNullException.ThrowIfNull( instance );
		if (!this._constants.TryAdd( typeof( T ), instance ))
			throw new ServiceProviderException( typeof( T ), "Constant already added" );
	}

	public object GetService( Type t ) => GetInternal( t );

	public T GetService<T>() where T : class => GetService( typeof( T ) ) as T ?? throw new ServiceProviderException( typeof( T ) );

	protected override object GetInternal( Type t ) {
		Type implementingType = GetImplementingType( t );
		if (_constants.TryGetValue( implementingType, out object? constant ))
			return constant;
		if (this._instances.TryGetValue( implementingType, out object? instance ))
			return instance;
		instance = Create( implementingType, false );
		this._instances.TryAdd( t, instance );
		ServiceAdded?.Invoke( instance );
		return instance;
	}
}


