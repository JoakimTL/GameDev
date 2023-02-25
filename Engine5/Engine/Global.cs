using Engine.Structure.Interfaces;
using Engine.Structure.ServiceProvider;

namespace Engine;

public static class Global
{
    private static readonly RestrictedServiceProvider<IGlobalService> _serviceProvider = new();
    private static readonly ServiceProviderDisposalExtension _serviceProviderDisposal = new(_serviceProvider);
    public static T Get<T>() where T : IGlobalService => _serviceProvider.Get<T>();
    internal static object? Get(Type t) => _serviceProvider.Get(t);

    internal static void Shutdown()
    {
        _serviceProviderDisposal.Dispose();
        Log.Stop();
    }
}
