namespace Engine;

public sealed class ServiceUpdaterExtension( IServiceProvider serviceProvider ) : ServiceProviderExtensionBase<IServiceProvider, IUpdateable>( serviceProvider ), IUpdateable {
	public void Update( double time, double deltaTime ) {
		foreach (IUpdateable updateable in this.SortedServices)
			updateable.Update( time, deltaTime );
	}
}
