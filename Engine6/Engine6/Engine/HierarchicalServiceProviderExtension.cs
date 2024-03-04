namespace Engine;

public abstract class HierarchicalServiceProviderExtension<T> : Identifiable {
	protected readonly IServiceProvider _serviceProvider;
	protected readonly BidirectionalTypeTree _tree;
	protected readonly List<T> _sortedServices;

	public HierarchicalServiceProviderExtension( IServiceProvider serviceProvider, Type processType, bool addConstructorParameterTypesAsParents ) {
		_sortedServices = [];
		_tree = new( processType, addConstructorParameterTypesAsParents );
		_tree.TreeUpdated += TreeUpdated;
		_serviceProvider = serviceProvider;
		_serviceProvider.ServiceAdded += OnServiceAdded;
	}

	private void TreeUpdated() {
		_sortedServices.Clear();
		_sortedServices.AddRange( _tree.GetNodesSorted().Select( _serviceProvider.GetService ).OfType<T>() );
	}

	private void OnServiceAdded( object service ) {
		if (service is T t)
			_tree.Add( t.GetType() );
	}

}
