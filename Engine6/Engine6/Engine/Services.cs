namespace Engine;

public static class Services {

	public static IServiceProvider Global { get; }
	public static IServiceRegistry GlobalRegistry { get; }
	public static IServiceProvider ThreadLocal => _threadLocal.Value ?? throw new ServiceProviderException(typeof(IServiceProvider), "Unable to provide thread local service provider.");
	public static IServiceRegistry ThreadLocalRegistry => _threadLocalRegistry.Value ?? throw new ServiceProviderException( typeof( IServiceRegistry ), "Unable to provide thread local service registry." );
	private static readonly ThreadLocal<IServiceProvider> _threadLocal = new( () => new ServiceProvider( ThreadLocalRegistry ) );
	private static readonly ThreadLocal<IServiceRegistry> _threadLocalRegistry = new( () => new ServiceRegistry() );

	static Services() {
		GlobalRegistry = new ServiceRegistry();
		Global = new ServiceProvider( GlobalRegistry );
	}

}


