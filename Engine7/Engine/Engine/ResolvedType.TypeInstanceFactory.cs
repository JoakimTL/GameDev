using System.Collections.Frozen;
using System.Linq.Expressions;
using System.Reflection;

namespace Engine;

public sealed class TypeInstanceFactory {
	private readonly FrozenDictionary<Type[], Func<object[]?, object?>> _factories;

	public TypeInstanceFactory( Type t ) {
		this._factories = CreateFactories( t );
	}

	public object? CreateInstance( object[]? parameters ) {
		if (!this._factories.TryGetValue( parameters?.Select( p => p.GetType() ).ToArray() ?? Type.EmptyTypes, out Func<object[]?, object?>? factory ))
			return null;
		return factory.Invoke( parameters );
	}

	private static FrozenDictionary<Type[], Func<object[]?, object?>> CreateFactories( Type t ) {
		Dictionary<Type[], Func<object[]?, object?>> factories = [];
		foreach (ConstructorInfo constructor in t.GetConstructors()) {
			ParameterInfo[] parameters = constructor.GetParameters();
			Type[] parameterTypes = parameters.Select( p => p.ParameterType ).ToArray();

			// Create the lambda expression to invoke this constructor
			ParameterExpression parameterArray = Expression.Parameter( typeof( object[] ), "ctorArgs" );

			// Create expressions to cast array elements to constructor parameter types
			UnaryExpression[] constructorArgs = parameters
				.Select( ( param, index ) => Expression.Convert( Expression.ArrayIndex( parameterArray, Expression.Constant( index ) ), param.ParameterType ) )
				.ToArray();

			// Create a "new T(...)" expression
			NewExpression newExpression = Expression.New( constructor, constructorArgs );

			// Compile into a lambda: (object[] args) => new T(...)
			Func<object[]?, object?> lambda = Expression.Lambda<Func<object[]?, object?>>( Expression.Convert( newExpression, typeof( object ) ), parameterArray ).Compile();

			// Add the factory to the dictionary
			factories.Add( parameterTypes, lambda );
		}
		return FrozenDictionary.ToFrozenDictionary( factories, new TypeArrayEqualityComparer() );
	}


	public class TypeArrayEqualityComparer : IEqualityComparer<Type[]> {
		public bool Equals( Type[]? x, Type[]? y ) {
			if (x is null || y is null)
				return false;
			if (x.Length != y.Length)
				return false;
			for (int i = 0; i < x.Length; i++)
				if (x[ i ] != y[ i ])
					return false;
			return true;
		}

		public int GetHashCode( Type[] obj ) => HashCode.Combine( obj );
	}
}