namespace Engine;

public static class InstanceManagement {
	public static IInstanceProvider CreateProvider() => new InstanceProvider( new( new() ) );
}