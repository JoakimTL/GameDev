using Engine.Structure.Interfaces;

namespace Engine.Structure.ServiceProvider;

public sealed class ServiceProviderInitializationExtension : HierarchicalServiceProviderExtension<IInitializable>, IUpdateable
{
    public ServiceProviderInitializationExtension(ServiceProvider serviceProvider) : base(serviceProvider, typeof(IInitializable), true) { }

    public void Update(float time, float deltaTime)
    {
        if (_tree.Update())
        {
            for (int i = 0; i < _sortedServices.Count; i++)
            {
                Log.Line($"Initializing {_sortedServices[i]}...", Log.Level.LOW, ConsoleColor.DarkCyan);
                _sortedServices[i].Initialize();
                Log.Line($"Initialized {_sortedServices[i]}!", Log.Level.LOW, ConsoleColor.DarkCyan);
            }
            _tree.Clear();
        }
    }
}
