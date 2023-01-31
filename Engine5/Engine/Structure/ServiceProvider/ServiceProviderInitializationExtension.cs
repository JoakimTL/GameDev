using Engine.Structure.Interfaces;

namespace Engine.Structure.ServiceProvider;

public sealed class ServiceProviderInitializationExtension : HierarchicalServiceProviderExtension<IInitializable>, IUpdateable
{
    public ServiceProviderInitializationExtension(ServiceProvider serviceProvider) : base(serviceProvider, typeof(IInitializable)) { }

    public void Update(float time, float deltaTime)
    {
        if (_tree.Update())
        {
            for (int i = 0; i < _sortedServices.Count; i++)
                _sortedServices[i].Initialize();
            _tree.Clear();
        }
    }
}
