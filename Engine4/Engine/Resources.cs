using Engine.Structure;

namespace Engine;
public static class Resources {
	private static readonly UpdateableSingletonProvider<object> _singletonProvider = new();
	public static T Get<T>() where T : Identifiable => _singletonProvider.Get<T>();
	internal static void Update( float time, float deltaTime ) => _singletonProvider.Update( time, deltaTime );
	internal static void Dispose() => _singletonProvider.Dispose();

	public static Rendering.RenderResources Render => _singletonProvider.Get<Rendering.RenderResources>();
}
