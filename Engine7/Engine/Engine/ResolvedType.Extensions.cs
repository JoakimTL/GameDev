using System.Reflection;

namespace Engine;

public static class ResolvedTypeExtensions {
	/// <summary>
	/// Throws an <see cref="InvalidOperationException"/> if the type has more than one of the specified attribute.
	/// </summary>
	/// <throws cref="InvalidOperationException"></throws>
	public static bool TryGetAttribute<T>( this ResolvedType type, out T? attribute ) => (attribute = type.GetAttributes<T>().SingleOrDefault()) is not null;
	/// <summary>
	/// Throws an <see cref="InvalidOperationException"/> if the type does not have exactly one of the specified attribute.
	/// </summary>
	/// <throws cref="InvalidOperationException"></throws>
	public static T GetAttribute<T>( this ResolvedType type ) => type.GetAttributes<T>().Single();
	/// <summary>
	/// Creates a new instance of the specified type using the provided parameters. If the parameter array is null, it is treated as an empty array.
	/// </summary>
	/// <returns>Null if no constructor for the parameter list was found.</returns>
	public static object? CreateInstance( this Type type, object[]? parameters ) => TypeManager.ResolveType( type ).CreateInstance( parameters );
	/// <summary>
	/// Gets the property accessor for the specified property.
	/// </summary>
	public static TypePropertyAccessor GetPropertyAccessor( this ResolvedType type, BindingFlags bindingFlags, string propertyName ) {
		IReadOnlyList<PropertyInfo> properties = type.GetProperties( bindingFlags );
		PropertyInfo property = properties.FirstOrDefault( p => p.Name == propertyName ) ?? throw new InvalidOperationException( $"Property {propertyName} not found." );
		return type.GetPropertyAccessor( property );
	}
}