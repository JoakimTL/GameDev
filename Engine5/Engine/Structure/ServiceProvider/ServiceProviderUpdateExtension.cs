using Engine.Structure.Interfaces;

namespace Engine.Structure.ServiceProvider;

public sealed class ServiceProviderUpdateExtension : HierarchicalServiceProviderExtension<IUpdateable>, IUpdateable
{
    public ServiceProviderUpdateExtension(ServiceProvider serviceProvider) : base(serviceProvider, typeof(IUpdateable)) { }

    public void Update(float time, float deltaTime)
    {
        _tree.Update();
        for (int i = 0; i < _sortedServices.Count; i++)
            _sortedServices[i].Update(time, deltaTime);
    }
}
