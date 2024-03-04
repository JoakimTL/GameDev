namespace Engine;

public sealed class ServiceProviderDisposalExtension( IServiceProvider serviceProvider ) : HierarchicalServiceProviderExtension<IDisposable>( serviceProvider, typeof( IDisposable ), false ), IDisposable {
	public void Dispose() {
		_tree.Update();
		for (int i = 0; i < _sortedServices.Count; i++)
			_sortedServices[ i ].Dispose();
	}
}
