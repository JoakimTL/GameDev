using System.Collections.Concurrent;

namespace Engine.Structure.ServiceProvider;

public class ServiceProvider : DependencyInjectorBase {

	//Service provider should not dispose, update or initialize services. Those should be external managers, using the "ServiceAdded" and "ServiceRemoved" event

	//Service Providers should load new types, but not allow "adding". Peeking should be allowed.

	private readonly ConcurrentDictionary<Type, WeakReference> _constants;
	private readonly ConcurrentDictionary<Type, object> _services;

	public delegate void ServiceEventHandler( object service );

	public event ServiceEventHandler? ServiceAdded;

	public ServiceProvider() {
		_constants = new();
		_services = new();
	}

	protected virtual bool CanLoad( Type t ) => true;

	public void AddConstant( object o ) => _constants.TryAdd( o.GetType(), new( o ) );

	private object? Load( Type t ) {
		if ( !CanLoad( t ) ) {
			this.LogWarning( $"Cannot load {t.FullName}!" );
			return null;
		}

		var service = Create( t, true ) ?? throw new NullReferenceException( "Service shouldn't be null!" );

		if ( _services.TryAdd( t, service ) ) {
			ServiceAdded?.Invoke( service );
			return service;
		}

		if ( !_services.TryGetValue( t, out service ) )
			throw new Exception( "Service does not exist, but should." );
		return service;
	}

	protected override object? GetInternal( Type t ) {
		if ( _constants.TryGetValue( t, out var o ) )
			return o.Target;
		if ( _services.TryGetValue( t, out var service ) )
			return service;
		return Load( t );
	}

	public object? Get( Type t ) => GetInternal( t );


	public T Get<T>() => GetInternal( typeof( T ) ) is T t ? t : throw new InvalidOperationException( "Unable to properly load service." );

	public object? Peek( Type t ) {
		if ( _services.TryGetValue( t, out var service ) )
			return service;
		return null;
	}

	public T? Peek<T>() => Peek( typeof( T ) ) is T t ? t : default;

}
