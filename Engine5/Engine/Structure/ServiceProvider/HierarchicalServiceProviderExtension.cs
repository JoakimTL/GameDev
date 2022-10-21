namespace Engine.Structure.ServiceProvider;

public abstract class HierarchicalServiceProviderExtension<T>
{
    protected readonly ServiceProvider _serviceProvider;
    protected readonly BidirectionalTypeTree<T> _tree;
    protected readonly List<T> _sortedServices;

    public HierarchicalServiceProviderExtension(ServiceProvider serviceProvider)
    {
        _sortedServices = new();
        _tree = new();
        _tree.TreeUpdated += TreeUpdated;
        _serviceProvider = serviceProvider;
        _serviceProvider.ServiceAdded += OnServiceAdded;
    }

    private void TreeUpdated()
    {
        _sortedServices.Clear();
        _sortedServices.AddRange(_tree.GetNodesSorted().Select(_serviceProvider.Get).OfType<T>());
    }

    private void OnServiceAdded(object service)
    {
        if (service is T t)
            _tree.Add(t.GetType());
    }

}
