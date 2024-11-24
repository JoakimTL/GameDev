using System.Collections.Generic;
using System.Reflection;

namespace Engine;

internal sealed class InstanceProvider : DisposableIdentifiable, IInstanceProvider {

	private readonly InstanceCatalog _instanceCatalog;
	private readonly List<IDisposable> _disposables = [];
	private readonly Dictionary<Type, object> _instances = [];
	public event Action<object>? OnInstanceAdded;
	public IInstanceCatalog Catalog => this._instanceCatalog;

	public InstanceProvider( InstanceCatalog instanceCatalog ) {
		this._instanceCatalog = instanceCatalog;
		instanceCatalog.OnHostedTypeAdded += OnHostedTypeAdded;
		foreach (Type hostedType in instanceCatalog.HostedTypes)
			Get( hostedType );
	}

	private void OnHostedTypeAdded( Type type )
		=> Get( type );

	public object Get( Type t ) {
		Type implementationType = this._instanceCatalog.TryResolve( t, out Type? type ) ? type : throw new InvalidOperationException( $"No implementation found for {t.Name}" );

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

	public bool Include<T>( T instance, bool triggerEvents ) {
		ArgumentNullException.ThrowIfNull( instance );
		Type contractType = typeof( T );
		Type implementationType = this._instanceCatalog.TryResolve( contractType, out Type? type ) ? type : throw new InvalidOperationException( $"No implementation found for {contractType.Name}" );
		if (!this._instances.TryAdd( implementationType, instance ))
			return false;
		if (triggerEvents)
			OnInstanceAdded?.Invoke( instance );
		return true;
	}

	protected override bool InternalDispose() {
		foreach (IDisposable disposable in this._disposables)
			disposable.Dispose();
		this._disposables.Clear();
		return true;
	}
}
