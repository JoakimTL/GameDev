using System;
using System.Collections.Immutable;

namespace Engine;

internal sealed class ServiceRegistry : IServiceRegistry {

	private ImmutableDictionary<Type, Type> _registry;

	public ServiceRegistry() {
		this._registry = ImmutableDictionary<Type, Type>.Empty;
	}

	public Type? GetImplementation( Type interfaceType ) => this._registry.GetValueOrDefault( interfaceType );
	public Type? GetImplementation<T>() => GetImplementation( typeof( T ) );

	public void Register<Interface, Class>() where Class : Interface => this._registry = this._registry.Add( typeof( Interface ), typeof( Class ) );
}
