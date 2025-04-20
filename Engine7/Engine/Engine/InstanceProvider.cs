using System.Reflection;

namespace Engine;

internal sealed class InstanceProvider : DisposableIdentifiable, IInstanceProvider {

	private readonly InstanceCatalog _instanceCatalog;
	private readonly InstanceDisposalExtension _disposalExtension;
	private readonly Dictionary<Type, object> _instances = [];
	public event Action<object>? OnInstanceAdded;
	public IInstanceCatalog Catalog => this._instanceCatalog;

	public InstanceProvider( InstanceCatalog instanceCatalog ) {
		this._instanceCatalog = instanceCatalog;
		instanceCatalog.OnHostedTypeAdded += OnHostedTypeAdded;
		foreach (Type hostedType in instanceCatalog.HostedTypes)
			Get( hostedType );
		_disposalExtension = new( this );
	}

	private void OnHostedTypeAdded( Type type )
		=> Get( type );

	public object Get( Type t ) {
		if (t == typeof( IInstanceProvider ) || t.IsAssignableTo( typeof( IInstanceProvider ) ))
			return this;
		Type implementationType = this._instanceCatalog.TryResolve( t, out Type? type ) ? type : throw new InvalidOperationException( $"No implementation found for {t.Name}" );

		if (this._instances.TryGetValue( implementationType, out object? instance ))
			return instance;

		instance = Construct( implementationType );
		this._instances.Add( implementationType, instance );
		OnInstanceAdded?.Invoke( instance );
		return instance;
	}

	public object CreateTransient( Type t ) {
		if (t == typeof( IInstanceProvider ) || t.IsAssignableTo( typeof( IInstanceProvider ) ))
			return this;
		Type implementationType = this._instanceCatalog.TryResolve( t, out Type? type ) ? type : throw new InvalidOperationException( $"No implementation found for {t.Name}" );

		return Construct( implementationType );
	}

	private object Construct( Type implementationType ) {
		ConstructorInfo[] constructors = implementationType.GetConstructors();
		if (constructors.Length != 1)
			throw new InvalidOperationException( $"Expecting only one constructor for a service. Found {constructors.Length} constructors!" );
		ConstructorInfo constructor = constructors[ 0 ];

		ParameterInfo[] parameters = constructor.GetParameters();
		object[] resolvedParameters = new object[ parameters.Length ];
		for (int i = 0; i < parameters.Length; i++)
			resolvedParameters[ i ] = Get( parameters[ i ].ParameterType );

		return constructor.Invoke( resolvedParameters );
	}

	public bool Inject<T>( T instance, bool triggerEvents ) {
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
		_disposalExtension.Dispose();
		return true;
	}
}
