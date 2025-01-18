namespace Engine.Module.Render;

public sealed class RenderServiceAccess( IInstanceProvider instanceProvider ) : Identifiable, IInitializable {
	private readonly IInstanceProvider _instanceProvider = instanceProvider;

	public T Get<T>() where T : IRenderServiceProvider => _instanceProvider.Get<T>();

	public void Initialize() {
		foreach (Type t in TypeManager.Registry.ImplementationTypes.Where( p => p.IsAssignableTo( typeof( IRenderServiceProvider ) ) ))
			_instanceProvider.Catalog.Host( t );
	}
}