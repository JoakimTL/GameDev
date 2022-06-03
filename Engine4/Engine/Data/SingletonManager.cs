namespace Engine.Data;

/// <typeparam name="BASE"></typeparam>
/*public abstract class SingletonManager<BASE> : DisposableIdentifiable {

	protected readonly ConcurrentDictionary<Type, BASE> _singletons;

	public SingletonManager() {
		this._singletons = new ConcurrentDictionary<Type, BASE>();
	}

	/// <returns>Returns the default value if the manager is diposed.</returns>
	public T Get<T>() where T : BASE {
		if ( this.Disposed )
			throw new Exception( "Accessed object has been disposed!" );
		lock ( this._singletons ) {
			{
				if ( this._singletons.TryGetValue( typeof( T ), out BASE? baseValue ) )
					if ( baseValue is T value )
						return value;
			}
			{
				try {
					T value = Activator.CreateInstance<T>();
					this._singletons[ typeof( T ) ] = value;
					return value;
				} catch ( Exception e ) {
					this.LogError( e.Message );
				}
			}
		}
		throw new Exception( $"Couldn't find singleton {typeof( T ).Name}, nor a constructor with the chosen parameters!" );
	}

	public bool TryGet<T>( [NotNullWhen( returnValue: true )] out T? value ) where T : BASE {
		value = default;
		if ( this.Disposed ) {
			this.LogWarning( "Accessed object has been disposed!" );
			return false;
		}
		lock ( this._singletons ) {
			{
				if ( this._singletons.TryGetValue( typeof( T ), out BASE? baseValue ) )
					if ( baseValue is T tValue ) {
						value = tValue;
						return true;
					}
			}
			{
				try {
					value = Activator.CreateInstance<T>();
					this._singletons[ typeof( T ) ] = value;
					return true;
				} catch ( Exception e ) {
					this.LogError( e.Message );
				}
			}
		}
		this.LogWarning( $"Couldn't find singleton {typeof( T ).Name}, nor a constructor with the chosen parameters!" );
		return false;
	}

	public BASE Get( Type t ) {
		if ( this.Disposed )
			throw new Exception( "Accessed object has been disposed!" );
		if ( !t.IsAssignableTo( typeof( BASE ) ) )
			throw new Exception( $"{t.Name} does not derive from {typeof( BASE ).Name}!" );
		lock ( this._singletons ) {
			if ( this._singletons.TryGetValue( t, out BASE? value ) )
				if ( value is not null )
					return value;
			{
				try {
					if ( Activator.CreateInstance( t ) is BASE tValue ) {
						this._singletons[ t ] = tValue;
						return tValue;
					}
				} catch ( Exception e ) {
					this.LogError( e.Message );
				}
			}
		}
		throw new Exception( $"Couldn't find singleton {t.Name}, nor a constructor with the chosen parameters!" );
	}

	public bool TryGet( Type t, [NotNullWhen( returnValue: true )] out BASE? value ) {
		value = default;
		if ( this.Disposed ) {
			this.LogWarning( "Accessed object has been disposed!" );
			return false;
		}
		if ( !t.IsAssignableTo( typeof( BASE ) ) ) {
			this.LogWarning( $"{t.Name} does not derive from {typeof( BASE ).Name}!" );
			return false;
		}
		lock ( this._singletons ) {
			if ( this._singletons.TryGetValue( t, out value ) )
				if ( value is not null )
					return true;
			{
				try {
					if ( Activator.CreateInstance( t ) is BASE tValue ) {
						this._singletons[ t ] = tValue;
						value = tValue;
						return true;
					}
				} catch ( Exception e ) {
					this.LogWarning( e.Message );
				}
			}
		}
		this.LogWarning( $"Couldn't find singleton {t.Name}, nor a constructor with the chosen parameters!" );
		return false;
	}
}
*/