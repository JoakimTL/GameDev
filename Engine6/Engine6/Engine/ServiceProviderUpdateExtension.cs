namespace Engine;

public sealed class ServiceProviderUpdateExtension( IServiceProvider serviceProvider ) : HierarchicalServiceProviderExtension<IUpdateable, IUpdateable>( serviceProvider, false ), IUpdateable {
	public void Update( in double time, in double deltaTime ) {
		_tree.Update();
		for (int i = 0; i < _sortedServices.Count; i++)
			_sortedServices[ i ].Update( time, deltaTime );
	}
}
