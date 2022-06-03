using System.Collections;
using System.Reflection;

namespace Engine.Structure;

public class SingletonProvider<BASE> : DisposableIdentifiable {

	private readonly Dictionary<Type, object> _singletons;
	private readonly BidirectionalTreeStructureProvider _disposableSingletonTypeTree;
	public event Action<object>? SingletonAdded;
	public event Action<Type>? SingletonRemoved;

	public SingletonProvider() {
		this._singletons = new();
		this._disposableSingletonTypeTree = new( typeof( IDisposable ) );
		SingletonAdded += CheckDisposable;
		SingletonAdded += CheckRemovable;
		SingletonRemoved += OnSingletonRemoved;
	}

	private void OnSingletonRemoved( Type type ) {
		lock ( this._singletons ) {
			if ( this._singletons.TryGetValue( type, out object? singleton ) ) {
				this._singletons.Remove( type );
				this._disposableSingletonTypeTree.Remove( type );
				if ( singleton is IRemovable removable )
					removable.Removed -= OnSingletonRemovalTriggered;
			}
		}
	}

	private void OnSingletonRemovalTriggered( IRemovable obj ) => SingletonRemoved?.Invoke( obj.GetType() );

	private void CheckRemovable( object obj ) {
		if ( obj is IRemovable removable )
			removable.Removed += OnSingletonRemovalTriggered;
	}

	private void CheckDisposable( object obj ) {
		if ( obj is IDisposable )
			this._disposableSingletonTypeTree.Add( obj.GetType() );
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
		}

		this._singletons.Add( t, singleton );
		SingletonAdded?.Invoke( singleton );

		return singleton;
	}

	private object GetInternal( Type type ) {
		lock ( this._singletons ) {
			if ( !this._singletons.TryGetValue( type, out object? value ) )
				value = Add( type );
			return value;
		}
	}

	protected void Add<T>() => GetInternal( typeof( T ) );

	public object? GetOrDefault( Type type ) {
		lock ( this._singletons ) {
			if ( this._singletons.TryGetValue( type, out object? value ) )
				return value;
		}
		return default;
	}

	public BASE Get( Type t ) => t.IsAssignableTo( typeof( BASE ) ) ? (BASE) GetInternal( t ) : throw new InvalidOperationException( $"Cannot get singleton not derived from {typeof( BASE )}!" );

	public T Get<T>() where T : BASE => (T) GetInternal( typeof( T ) );

	public IEnumerable<T> GetMultiple<T>() where T : BASE => ( (IEnumerable) GetInternal( typeof( IEnumerable<T> ) ) ).OfType<T>();

	public IReadOnlyList<object> GetAll() {
		lock ( this._singletons )
			return this._singletons.Values.ToList();
	}

	protected override bool OnDispose() {
		lock ( this._singletons ) {
			foreach ( Type t in this._disposableSingletonTypeTree.WalkTreeForward() )
				if ( this._singletons[ t ] is IDisposable disposable )
					disposable.Dispose();
			this._disposableSingletonTypeTree.Clear();
			this._singletons.Clear();
		}
		return true;
	}
}
