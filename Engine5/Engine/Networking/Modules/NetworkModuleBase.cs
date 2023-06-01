using Engine.Structure.Interfaces;
using Engine.Structure.ServiceProvider;
using Engine.Structure;

namespace Engine.Networking.Module;
public abstract class NetworkModuleBase<TBase> : ModuleBase<TBase>, IInitializable, IUpdateable, IDisposable, ISystem where TBase : IModuleService {

	private readonly ServiceProviderUpdateExtension _serviceProviderUpdater;
	private readonly ServiceProviderInitializationExtension _serviceProviderInitializer;
	private readonly ServiceProviderDisposalExtension _serviceProviderDisposer;
	public bool SystemEssential => false;

	public abstract bool IsServer { get; }

	public NetworkModuleBase() : base( false ) {
		_serviceProviderUpdater = new( _serviceProvider );
		_serviceProviderInitializer = new( _serviceProvider );
		_serviceProviderDisposer = new( _serviceProvider );
	}

	public abstract void Initialize();

	public T? GetService<T>() where T : TBase => Get<T>();

	public void Update( float time, float deltaTime ) {
		_serviceProviderInitializer.Update( time, deltaTime );
		_serviceProviderUpdater.Update( time, deltaTime );
	}

	public void Dispose() {
		this.LogLine( "Disposing", Log.Level.NORMAL );
		_serviceProviderDisposer.Dispose();
		GC.SuppressFinalize( this );
	}
}
