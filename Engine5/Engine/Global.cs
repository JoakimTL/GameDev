using Engine.Structure.Interfaces;
using Engine.Structure.ServiceProvider;
using Engine.Utilities;

namespace Engine;

public static class Global {
	private static readonly RestrictedServiceProvider<IGlobalService> _serviceProvider = new();
	private static readonly ServiceProviderDisposalExtension _serviceProviderDisposal = new( _serviceProvider );
	public static T Get<T>() where T : IGlobalService => _serviceProvider.Get<T>();

	static Global() {
		ReflectionUtilities.LoadAllAssemblies();
	}

	internal static void Shutdown() {
		_serviceProviderDisposal.Dispose();
		Log.Stop();
	}
}
