using Engine.Structure;
using Engine.Structure.Interfaces;
using Engine.Structure.ServiceProvider;

namespace Engine.ECS;
public class ECSModule : ModuleBase<IECSService>, IInitializable, IUpdateable {

	private readonly ServiceProviderUpdateExtension _serviceProviderUpdater;
	private readonly ServiceProviderInitializationExtension _serviceProviderInitializer;

	public ECSModule() : base( true ) {
		_serviceProviderUpdater = new( _serviceProvider );
		_serviceProviderInitializer = new( _serviceProvider );
	}

	public void Initialize() {
		Get<EntityContainerService>();
	}

	public void Update( float time, float deltaTime ) {
		_serviceProviderInitializer.Update( time, deltaTime );
		_serviceProviderUpdater.Update( time, deltaTime );
	}
}
