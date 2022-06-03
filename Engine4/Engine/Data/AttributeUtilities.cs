namespace Engine.Data;

public static class AttributeUtilities {
	public static T? GetAttribute<T>( Type toCheck, bool inherit ) where T : Attribute {
		object? attribute = toCheck.GetCustomAttributes( typeof( T ), inherit ).FirstOrDefault();
		if ( attribute is T outT )
			return outT;
		return null;
	}
}
