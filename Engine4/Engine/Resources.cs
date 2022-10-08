using Engine.Structure;

namespace Engine;
public static class Resources {
	private static readonly ServiceProvider<object> _serviceProvider = new();
	public static T GlobalService<T>() where T : Identifiable => _serviceProvider.GetOrAdd<T>();
	internal static void Update( float time, float deltaTime ) => _serviceProvider.Update( time, deltaTime );
	internal static void Dispose() => _serviceProvider.Dispose();

	public static Rendering.RenderResources Render => _serviceProvider.GetOrAdd<Rendering.RenderResources>();
}
