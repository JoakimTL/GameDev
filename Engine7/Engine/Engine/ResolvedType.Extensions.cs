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
}