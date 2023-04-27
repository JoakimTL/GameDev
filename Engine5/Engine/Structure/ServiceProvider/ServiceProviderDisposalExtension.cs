namespace Engine.Structure.ServiceProvider;

public sealed class ServiceProviderDisposalExtension : HierarchicalServiceProviderExtension<IDisposable>, IDisposable
{
    public ServiceProviderDisposalExtension(ServiceProvider serviceProvider) : base(serviceProvider, typeof(IDisposable), false) { }

    public void Dispose()
    {
        _tree.Update();
        for (int i = 0; i < _sortedServices.Count; i++)
            _sortedServices[i].Dispose();
    }
}
