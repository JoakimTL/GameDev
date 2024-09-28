namespace Engine;

public abstract class HierarchicalServiceProviderExtension<TServiceType, TProcessType> : Identifiable {
	protected readonly IServiceProvider _serviceProvider;
	protected readonly BidirectionalTypeTree<TProcessType> _tree;
	protected readonly List<TServiceType> _sortedServices;

	public HierarchicalServiceProviderExtension( IServiceProvider serviceProvider, bool addConstructorParameterTypesAsParents ) {
		_sortedServices = [];
		_tree = new( addConstructorParameterTypesAsParents );
		_tree.TreeUpdated += TreeUpdated;
		_serviceProvider = serviceProvider;
		_serviceProvider.ServiceAdded += OnServiceAdded;
	}

	private void TreeUpdated() {
		_sortedServices.Clear();
		_sortedServices.AddRange( _tree.GetNodesSorted().Select( _serviceProvider.GetService ).OfType<TServiceType>() );
	}

	private void OnServiceAdded( object service ) {
		if (service is TServiceType t)
			_tree.Add( t.GetType() );
	}

}
