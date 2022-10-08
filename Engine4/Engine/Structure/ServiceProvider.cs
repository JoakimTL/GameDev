using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;

namespace Engine.Structure;

public class ServiceProvider<BASE> : DisposableIdentifiable, IUpdateable where BASE : class {

	private readonly ConcurrentDictionary<Type, object> _services;
	private readonly List<ServiceProviderExtension<BASE>> _extensions;
	public event Action<object>? ServiceAdded;
	public event Action<object>? ServiceRemoved;
	public bool Active { get; protected set; }

	public ServiceProvider() {
		this._services = new();
		this._extensions = new();
		this.Active = true;
		ServiceAdded += CheckRemovable;
		ServiceRemoved += OnSingletonRemoved;
	}

	private void OnSingletonRemovalTriggered( IRemoveable obj ) => ServiceRemoved?.Invoke( obj );

	private void OnSingletonRemoved( object obj ) {
		if ( this._services.TryRemove( obj.GetType(), out _ ) )
			if ( obj is IRemoveable removable )
				removable.Removed -= OnSingletonRemovalTriggered;
	}

	private void CheckRemovable( object obj ) {
		if ( obj is IRemoveable removable )
			removable.Removed += OnSingletonRemovalTriggered;
	}

	private object Add( Type t ) {
		object singleton;
		if ( t.IsAssignableTo( typeof( IEnumerable ) ) ) {
			Type[]? generics = t.GetGenericArguments();
			if ( generics.Length != 1 )
				throw new InvalidOperationException( "IEnumerable must have one generic argument." );
			Type? generic = generics[ 0 ];

			IReadOnlyList<Type>? implementations = DependencyTypeProvider.GetImplementations( generic );

			singleton = implementations.Select( GetInternal ).ToList();
		} else {
			Type implementingType = t;
			if ( t.IsInterface || t.IsAbstract )
				implementingType = DependencyTypeProvider.GetImplementation( t );

			ConstructorInfo[]? ctors = implementingType.GetConstructors();
			if ( ctors.Length == 0 )
				throw new InvalidOperationException( "Type must have a valid constructor" );

			ConstructorInfo? ctor = ctors[ 0 ];
			if ( ctors.Length > 1 )
				this.LogLine( $"Found multiple constructors for type {t.FullName}. Using {t.Name}({string.Join( ", ", ctor.GetParameters().Select( p => $"{p.ParameterType.Name} {p.Name}" ) )})!", Log.Level.CRITICAL );

			Type[] parameterTypes = ctor.GetParameters().Select( p => p.ParameterType ).ToArray();

			if ( parameterTypes.Any( p => p == t ) )
				throw new Exception( $"Singleton {t.FullName} can't depend on itself!" );

			Type selfType = GetType();
			object[]? parameters = parameterTypes.Select( p => p == selfType ? this : GetInternal( p ) ).ToArray();

			singleton = ctor.Invoke( parameters );

			foreach ( Type requiredServiceType in implementingType.GetCustomAttributes<RequireServiceAttribute>().Select( p => p.Type ) )
				GetInternal( requiredServiceType );
		}

		this._services.TryAdd( t, singleton );
		ServiceAdded?.Invoke( singleton );

		return singleton;
	}

	private object GetInternal( Type type ) {
		if ( !this._services.TryGetValue( type, out object? value ) )
			value = Add( type );
		return value;
	}

	protected void Add<T>() => GetInternal( typeof( T ) );

	public object? Peek( Type type ) {
		if ( this._services.TryGetValue( type, out object? value ) )
			return value;
		return default;
	}
	public T? Peek<T>() where T : BASE => Peek( typeof( T ) ) is T t ? t : default;

	public BASE GetOrAdd( Type t ) => t.IsAssignableTo( typeof( BASE ) )
		? (BASE) GetInternal( t )
		: throw new InvalidOperationException( $"Cannot get service not derived from {typeof( BASE )}!" );

	public T GetOrAdd<T>() where T : BASE => (T) GetInternal( typeof( T ) );

	public IReadOnlyList<T> GetOrAddAll<T>() where T : BASE => ( (IEnumerable) GetInternal( typeof( IEnumerable<T> ) ) ).OfType<T>().ToList();

	public IReadOnlyList<T> PeekAll<T>() where T : BASE => this._services.Values.OfType<T>().ToList();

	public IReadOnlyList<object?> PeekAll( IEnumerable<Type> types ) => types.Select( Peek ).ToList();

	public void Update( float time, float deltaTime ) {
		if ( !this.Active )
			return;

		for ( int i = 0; i < this._extensions.Count; i++ )
			if ( this._extensions[ i ] is IUpdateable updateable )
				updateable.Update( time, deltaTime );
	}

	protected override bool OnDispose() {
		for ( int i = 0; i < this._extensions.Count; i++ )
			this._extensions[ i ].Dispose();
		this._services.Clear();
		return true;
	}

	internal void ExecuteOnServices( IEnumerable<Type> types, Action<object> func ) {
		foreach ( Type t in types ) {
			object? service = Peek( t );
			if ( service is not null )
				func.Invoke( service );
		}
	}
}
