namespace Engine;

/*
public class SingletonProvider<BT> : Identifiable where BT : Identifiable {

	private readonly Dictionary<Type, BT> _singletonsByType;
	private readonly Dictionary<Guid, BT> _singletonsByGuid;
	private readonly List<IUpdateable> _updateableSingletons;
	private readonly List<IDisposable> _disposableSingletons;

	internal SingletonProvider() {
		this._singletonsByType = new();
		this._singletonsByGuid = new();
		this._updateableSingletons = new();
		this._disposableSingletons = new();
	}

	public T Get<T>() where T : BT => Get( typeof( T ) ) is T t ? t : throw new Exception( "Wrong type stored in singleton provider." );

	private BT Get( Type t ) {
		if ( !this._singletonsByType.TryGetValue( t, out BT? o ) )
			o = CreateSingleton( t );
		return o;
	}

	public BT Get( Guid g ) {
		if ( !this._singletonsByGuid.TryGetValue( g, out BT? o ) )
			o = CreateSingleton( g );
		return o;
	}

	private BT CreateSingleton( Guid guid ) {
		Type? singletonType = IdentificationRegistry.Get( guid );
		if ( singletonType is null )
			throw new ArgumentException( "Guid must match a known singleton!", nameof( guid ) );
		return CreateSingleton( singletonType );
	}

	private BT CreateSingleton( Type t ) {
		ConstructorInfo[]? ctors = t.GetConstructors();
		if ( ctors.Length == 0 )
			throw new Exception( $"No constructor for type {t.FullName}!" );
		ConstructorInfo? ctor = ctors[ 0 ];
		if ( ctors.Length > 1 )
			this.LogLine( $"Found multiple constructors for type {t.FullName}. Using {t.Name}({string.Join( ", ", ctor.GetParameters().Select( p => $"{p.ParameterType.Name} {p.Name}" ) )})!", Log.Level.CRITICAL );
		if ( ctor.GetParameters().Any( p => p.ParameterType == t ) )
			throw new Exception( $"Singleton {t.FullName} can't depend on itself!" );
		object?[] dependencies = ctor.GetParameters().Select( p => Get( p.ParameterType ) ).ToArray();
		object? singletonObject = ctor.Invoke( dependencies );
		if ( singletonObject is not BT singleton )
			throw new Exception( $"Singleton was not of correct type." );
		SingletonCreated( singleton );
		lock ( this._singletonsByType )
			this._singletonsByType[ t ] = singleton;
		IdentificationAttribute? guidAttrib = t.GetCustomAttribute<IdentificationAttribute>();
		if ( guidAttrib is not null )
			lock ( this._singletonsByGuid )
				this._singletonsByGuid[ guidAttrib.Guid ] = singleton;
		if ( singleton is IInitializableSingleton initializableSingleton )
			initializableSingleton.Initialize();
		if ( singleton is IUpdateable updateableSingleton )
			lock ( this._updateableSingletons ) {
				this._updateableSingletons.Add( updateableSingleton );
			}
		if ( singleton is IDisposable disposableSingleton )
			lock ( this._disposableSingletons ) {
				this._disposableSingletons.Add( disposableSingleton );
			}
		return singleton;
	}

	protected virtual void SingletonCreated( BT singleton ) { }

	public void Update( float time, float deltaTime ) {
		lock ( this._updateableSingletons ) {
			for ( int i = 0; i < this._updateableSingletons.Count; i++ ) {
				IUpdateable updateable = this._updateableSingletons[ i ];
				if ( updateable.Active )
					updateable.Update( time, deltaTime );
			}
		}
	}

	public void DisposeAndClear() {
		lock ( this._disposableSingletons )
			MapAndDispose( this._disposableSingletons );
		lock ( this._singletonsByType )
			this._singletonsByType.Clear();
		lock ( this._singletonsByGuid )
			this._singletonsByGuid.Clear();
		lock ( this._updateableSingletons )
			this._updateableSingletons.Clear();
		lock ( this._disposableSingletons )
			this._disposableSingletons.Clear();
	}

	private void MapAndDispose( List<IDisposable> singletons ) {
		HashSet<Type> unmapped = new( singletons.Select( p => p.GetType() ) );
		HashSet<Type> newlyMapped = new();
		List<Type> sorted = new();
		while ( unmapped.Count > 0 ) {
			newlyMapped.Clear();

			foreach (Type t in unmapped) {
				List<DisposeBeforeAttribute> attributeList = t.GetCustomAttributes<DisposeBeforeAttribute>().ToList();
				if (attributeList.Count == 0) {
					newlyMapped.Add( t );
					continue;
				}

				bool isValid = true;
				foreach ( DisposeBeforeAttribute attribute in attributeList ) {
					if ( unmapped.Contains( attribute.FollowingType ) )
						isValid = false;
				}

				if ( isValid )
					newlyMapped.Add( t );
			}

			foreach ( Type t in newlyMapped ) {
					unmapped.Remove( t );
					sorted.Add( t );
				}
		}

		sorted.Reverse();

		Dictionary<Type, IDisposable>? dict = singletons.ToDictionary( p => p.GetType() );
		for ( int i = 0; i < sorted.Count; i++ )
			dict[ sorted[ i ] ].Dispose();
	}

	public IEnumerable<T> AddAll<T>() {
		Type t = typeof( T );
		IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies().SelectMany( p => p.GetTypes().Where( q => q.IsAssignableTo( t ) && !q.IsInterface && !q.IsAbstract ) );
		return types.Select( Get ).OfType<T>().ToList();
	}

	public IEnumerable<T> GetAll<T>() {
		Type t = typeof( T );
		lock ( this._singletonsByType )
			return this._singletonsByType.Where( p => p.Key.IsAssignableTo( t ) ).Select( p => p.Value ).OfType<T>();
	}
}
*/