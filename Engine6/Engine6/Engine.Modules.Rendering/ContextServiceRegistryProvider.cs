namespace Engine.Modules.Rendering;

internal static class ContextServiceRegistryProvider {

	public static IServiceRegistry ServiceRegistry { get; } = CreateServiceRegistry();

	private static IServiceRegistry CreateServiceRegistry() {
		IServiceRegistry serviceRegistry = Services.CreateServiceRegistry();



		return serviceRegistry;
	}
}