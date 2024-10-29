using System.Reflection;

namespace Engine;

internal sealed class ServiceProvider( ServiceCatalog serviceCatalog ) : IServiceProvider, IDisposable {

	private readonly ServiceCatalog _serviceCatalog = serviceCatalog;
	private readonly List<IDisposable> _disposables = [];
	private readonly Dictionary<Type, object> _instances = [];
	public event Action<object>? OnInstanceAdded;

	public object Get( Type t ) {
		Type implementationType = this._serviceCatalog.TryResolve( t, out Type? type ) ? type : throw new InvalidOperationException( $"No implementation found for {t.Name}" );

		if (this._instances.TryGetValue( implementationType, out object? instance ))
			return instance;

		ConstructorInfo[] constructors = implementationType.GetConstructors();
		if (constructors.Length != 1)
			throw new InvalidOperationException( $"Expecting only one constructor for a service. Found {constructors.Length} constructors!" );
		ConstructorInfo constructor = constructors[ 0 ];

		ParameterInfo[] parameters = constructor.GetParameters();
		object[] resolvedParameters = new object[ parameters.Length ];
		for (int i = 0; i < parameters.Length; i++)
			resolvedParameters[ i ] = Get( parameters[ i ].ParameterType );

		instance = constructor.Invoke( resolvedParameters );
		this._instances.Add( implementationType, instance );
		OnInstanceAdded?.Invoke( instance );
		if (instance is IDisposable disposable)
			this._disposables.Add( disposable );
		return instance;
	}

	public void Dispose() {
		foreach (IDisposable disposable in this._disposables)
			disposable.Dispose();
		this._disposables.Clear();
	}
}
