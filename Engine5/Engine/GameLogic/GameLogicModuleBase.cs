using Engine.GameLogic.ECPS;
using Engine.Structure;
using Engine.Structure.Interfaces;
using Engine.Structure.ServiceProvider;

namespace Engine.GameLogic;

public abstract class GameLogicModuleBase : ModuleBase<IGameLogicService>, IInitializable, IUpdateable, IDisposable, ISystem
{

    private readonly ServiceProviderUpdateExtension _serviceProviderUpdater;
    private readonly ServiceProviderInitializationExtension _serviceProviderInitializer;
    private readonly ServiceProviderDisposalExtension _serviceProviderDisposer;

    public bool SystemEssential => true;

    public GameLogicModuleBase() : base(true)
    {
        _serviceProviderUpdater = new(_serviceProvider);
        _serviceProviderInitializer = new(_serviceProvider);
        _serviceProviderDisposer = new(_serviceProvider);
    }

    public void Initialize()
    {
        Get<EntityContainerService>();
		Get<EntitySystemContainerService>();
		Get<EntityProcessorContainerService>();
		OnInitialize();
    }

    public T? GetService<T>() where T : IGameLogicService => Get<T>();

    public void Update(float time, float deltaTime)
    {
        _serviceProviderInitializer.Update(time, deltaTime);
        OnUpdate(time, deltaTime);
        _serviceProviderUpdater.Update(time, deltaTime);
    }

    protected abstract void OnInitialize();
    protected abstract void OnUpdate(float time, float deltaTime);

    public void Dispose()
    {
        this.LogLine("Disposing", Log.Level.NORMAL);
        _serviceProviderDisposer.Dispose();
		Stop();
		GC.SuppressFinalize(this);
    }
}
