namespace Engine;

internal sealed class ServiceLibrary : IServiceLibrary {

	private readonly Dictionary<Type, Type> _connections = [];

	internal IReadOnlyDictionary<Type, Type> Connections => this._connections;

	public bool Connect<TContract, TImplementation>() where TImplementation : TContract {
		Type implementationType = typeof( TImplementation );
		if (!implementationType.IsClass)
			throw new InvalidOperationException( "Can't assign a non-class type as an implementation!" );
		if (implementationType.IsAbstract)
			throw new InvalidOperationException( "Can't assign an abstract type as an implementation!" );
		return this._connections.TryAdd( typeof( TContract ), typeof( TImplementation ) );
	}
}