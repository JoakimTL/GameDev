namespace Engine;

public static class InstanceManagement {
	public static IInstanceProvider CreateProvider( bool allowSelfhosting ) => new InstanceProvider( new( new() ), allowSelfhosting );
}