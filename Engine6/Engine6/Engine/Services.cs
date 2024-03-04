namespace Engine;

public static class Services {

	public static IServiceProvider Global { get; }
	public static IServiceRegistry GlobalRegistry { get; }

	static Services() {
		GlobalRegistry = new ServiceRegistry();
		Global = new ServiceProvider( GlobalRegistry );
	}

	public static IServiceProvider CreateServiceProvider( IServiceRegistry registry ) => new ServiceProvider( registry );

	public static IServiceRegistry CreateServiceRegistry() => new ServiceRegistry();

}


