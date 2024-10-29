namespace Engine;

public sealed class ServiceInitializerExtension( IServiceProvider serviceProvider ) : ServiceProviderExtensionBase<IServiceProvider, IInitializable>( serviceProvider ), IUpdateable {
	private readonly List<Type> _initializedServices = [];

	public void Update( double time, double deltaTime ) {
		foreach (IInitializable initializable in this.SortedServices) {
			initializable.Initialize();
			_initializedServices.Add( initializable.GetType() );
		}

		if (_initializedServices.Count == this.SortedServices.Count) {
			Clear();
		} else
			RemoveAll( _initializedServices );

		_initializedServices.Clear();
	}
}