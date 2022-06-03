namespace Engine.Data;

public static class ReflectionMagic {

	private static readonly Dictionary<Type, Func<object>> _constructors = new();

	/// <summary>
	/// Attempts to get the no parameter constructor for a type.
	/// </summary>
	/// <typeparam name="T">The return type for ctorFunc delegate. This is only a declared type, can be cast to derived types.</typeparam>
	/// <param name="t">The type to search for the constructor in</param>
	/// <param name="ctorFunc">The constructor delegate.</param>
	/// <returns>True if a constructor was found, false if otherwise</returns>
	public static bool TryGetConstructor( Type t, out Func<object>? ctorFunc ) {
		ctorFunc = null;
		System.Reflection.ConstructorInfo? constructor = t.GetConstructor( Array.Empty<Type>() );
		if ( constructor is null )
			return false;
		System.Linq.Expressions.Expression<Func<object>> creatorExpression = System.Linq.Expressions.Expression.Lambda<Func<object>>( System.Linq.Expressions.Expression.New( constructor ) );
		ctorFunc = creatorExpression.Compile();
		return ctorFunc is not null;
	}

	/// <summary>
	/// Attempts to get the no parameter constructor for a type.
	/// </summary>
	/// <typeparam name="T">The return type for ctorFunc delegate. This is only a declared type, can be cast to derived types.</typeparam>
	/// <param name="t">The type to search for the constructor in</param>
	/// <param name="ctorFunc">The constructor delegate.</param>
	/// <returns>True if a constructor was found, false if otherwise</returns>
	public static bool TryGetConstructor<T>( Type t, out Func<T>? ctorFunc ) {
		ctorFunc = null;
		System.Reflection.ConstructorInfo? constructor = t.GetConstructor( Array.Empty<Type>() );
		if ( constructor is null )
			return false;
		System.Linq.Expressions.Expression<Func<T>> creatorExpression = System.Linq.Expressions.Expression.Lambda<Func<T>>( System.Linq.Expressions.Expression.New( constructor ) );
		ctorFunc = creatorExpression.Compile();
		return ctorFunc is not null;
	}

	/// <summary>
	/// Attempts to get the no parameter constructor for a type.
	/// </summary>
	/// <typeparam name="T">The type to search for the constructor in, also the return type for the constructor.</typeparam>
	/// <param name="ctorFunc">The constructor delegate.</param>
	/// <returns>True if a constructor was found, false if otherwise</returns>
	public static bool TryGetConstructor<T>( out Func<T>? ctorFunc ) {
		ctorFunc = null;
		System.Reflection.ConstructorInfo? constructor = typeof( T ).GetConstructor( Array.Empty<Type>() );
		if ( constructor is null )
			return false;
		System.Linq.Expressions.Expression<Func<T>> creatorExpression = System.Linq.Expressions.Expression.Lambda<Func<T>>( System.Linq.Expressions.Expression.New( constructor ) );
		ctorFunc = creatorExpression.Compile();
		return ctorFunc is not null;
	}

	public static bool TryConstruct<T>( out T? newInstance ) {
		Type t = typeof( T );
		newInstance = default;
		if ( !_constructors.TryGetValue( t, out Func<object>? ctor ) )
			if ( TryGetConstructor( t, out ctor ) && ctor is not null ) {
				_constructors.Add( t, ctor );
			} else {
				Log.Warning( $"Failed to fetch constructor for type {t.Name}. Class is most likely missing a parameterless constructor.", stacktrace: true );
				return false;
			}
		object? newInstanceObject = ctor.Invoke();
		if ( newInstanceObject is not T ) {
			Log.Warning( $"Attempted to construct instance of type {t.Name}, but got {newInstanceObject.GetType().Name}!", stacktrace: true );
			return false;
		}
		newInstance = (T) newInstanceObject;
		return true;
	}

	public static bool TryConstruct<T>( Type t, out T? newInstance ) {
		newInstance = default;
		if ( t.IsAssignableFrom( typeof( T ) ) ) {
			Log.Warning( $"{t.Name} is not assignable from {typeof( T ).Name}!", stacktrace: true );
			return false;
		}
		if ( !_constructors.TryGetValue( t, out Func<object>? ctor ) )
			if ( TryGetConstructor( t, out ctor ) && ctor is not null ) {
				_constructors.Add( t, ctor );
			} else {
				Log.Warning( $"Failed to fetch constructor for type {t.Name}. Class is most likely missing a parameterless constructor.", stacktrace: true );
				return false;
			}
		object? newInstanceObject = ctor.Invoke();
		if ( newInstanceObject is not T ) {
			Log.Warning( $"Attempted to construct instance of type {t.Name}, but got {newInstanceObject.GetType().Name}!", stacktrace: true );
			return false;
		}
		newInstance = (T) newInstanceObject;
		return true;
	}

	public static bool TryConstruct( Type t, out object? newInstance ) {
		newInstance = default;
		if ( !_constructors.TryGetValue( t, out Func<object>? ctor ) )
			if ( TryGetConstructor( t, out ctor ) && ctor is not null ) {
				_constructors.Add( t, ctor );
			} else {
				Log.Warning( $"Failed to fetch constructor for type {t.Name}. Class is most likely missing a parameterless constructor.", stacktrace: true );
				return false;
			}
		newInstance = ctor.Invoke();
		return true;
	}

	public static IEnumerable<Type> GetAllTypes<T>( bool allowAbstract = false, bool allowInterfaces = false ) {
		Type t = typeof( T );
		return AppDomain.CurrentDomain.GetAssemblies().SelectMany( p => p.GetTypes().Where( q => q.IsAssignableTo( t ) && ( allowInterfaces || !q.IsInterface ) && ( allowAbstract || !q.IsAbstract ) ) );
	}
}
