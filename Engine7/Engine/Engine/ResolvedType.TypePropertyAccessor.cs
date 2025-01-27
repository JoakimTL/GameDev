using System.Linq.Expressions;
using System.Reflection;

namespace Engine;

public sealed class TypePropertyAccessor {
	private readonly PropertyInfo _property;
	private readonly bool _expectsInstanceInput;
	private readonly Func<object?, object?>? _readAccessor;
	private readonly Action<object?, object?>? _writeAccessor;

	public TypePropertyAccessor( PropertyInfo propertyInfo ) {
		this._property = propertyInfo;
		this._expectsInstanceInput = !((propertyInfo.GetMethod?.IsStatic ?? false) || (propertyInfo.SetMethod?.IsStatic ?? false));
		this._readAccessor = CreateReadAccessor( _property );
		this._writeAccessor = CreateWriteAccessor( _property );
	}

	public T ReadProperty<T>( object? instance )
		=> ReadProperty( instance ) is T value ? value : throw new InvalidCastException();

	public object? ReadProperty( object? instance ) {
		if (_readAccessor is null)
			throw new InvalidOperationException( "Property is write-only." );
		if (_expectsInstanceInput && instance is null)
			throw new ArgumentNullException( nameof( instance ) );
		return _readAccessor( instance );
	}

	public void WriteProperty( object? instance, object? value ) {
		if (_writeAccessor is null)
			throw new InvalidOperationException( "Property is read-only." );
		if (_expectsInstanceInput && instance is null)
			throw new ArgumentNullException( nameof( instance ) );
		_writeAccessor( instance, value );
	}

	private static Func<object?, object?>? CreateReadAccessor( PropertyInfo property ) {
		if (!property.CanRead)
			return null;

		MethodInfo getMethod = property.GetMethod ?? throw new InvalidOperationException( "Property has no getter." );

		ParameterExpression instanceParameter = Expression.Parameter( typeof( object ), "instance" );
		Expression propertyAccess;

		if (getMethod.IsStatic) {
			propertyAccess = Expression.Property( null, property );
		} else {
			UnaryExpression instanceCast = Expression.Convert( instanceParameter, property.DeclaringType! );
			propertyAccess = Expression.Property( instanceCast, property );
		}

		UnaryExpression convertResult = Expression.Convert( propertyAccess, typeof( object ) );
		Expression<Func<object?, object?>> lambda = Expression.Lambda<Func<object?, object?>>( convertResult, instanceParameter );

		return lambda.Compile();
	}

	private static Action<object?, object?>? CreateWriteAccessor( PropertyInfo property ) {
		if (!property.CanWrite)
			return null;

		MethodInfo setMethod = property.SetMethod ?? throw new InvalidOperationException( "Property has no setter." );

		ParameterExpression instanceParameter = Expression.Parameter( typeof( object ), "instance" );
		ParameterExpression valueParameter = Expression.Parameter( typeof( object ), "value" );

		Expression propertyAccess;

		if (setMethod.IsStatic) {
			propertyAccess = Expression.Property( null, property );
		} else {
			UnaryExpression instanceCast = Expression.Convert( instanceParameter, property.DeclaringType! );
			propertyAccess = Expression.Property( instanceCast, property );
		}

		UnaryExpression valueCast = Expression.Convert( valueParameter, property.PropertyType );
		BinaryExpression assign = Expression.Assign( propertyAccess, valueCast );
		Expression<Action<object?, object?>> lambda = Expression.Lambda<Action<object?, object?>>( assign, instanceParameter, valueParameter );

		return lambda.Compile();
	}
}