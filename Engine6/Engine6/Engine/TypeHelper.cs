namespace Engine;

public static class TypeHelper {
	public static IReadOnlyList<Type> AllTypes { get; } = AppDomain.CurrentDomain.GetAssemblies().SelectMany( x => x.GetTypes() ).ToArray().AsReadOnly();
}