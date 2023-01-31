using Engine.GlobalServices;
using Engine.Structure.Interfaces;
using Engine.Structure.ServiceProvider;

namespace Engine.Structure;
public abstract class ModuleBase : Identifiable
{

    public bool Active { get; private set; }
    public bool Essential { get; private set; }

    public ModuleBase(bool essential)
    {
        Essential = essential;
        Active = true;
        Global.Get<ModuleContainerService>().Add(this);
    }

    /// <summary>
    /// Stops the module, and initiates disposal and memory release.
    /// </summary>
    protected void Stop()
    {
        Active = false;
    }

    internal void ForceStop() => Stop();
}

public abstract class ModuleBase<TBase> : ModuleBase where TBase : IModuleService
{

    protected readonly RestrictedServiceProvider<TBase> _serviceProvider;

    public ModuleBase(bool essential) : base(essential)
    {
        _serviceProvider = new();
    }

    protected void Add<T>() where T : TBase => _serviceProvider.Get<T>();
    protected T Get<T>() where T : TBase => _serviceProvider.Get<T>();
}
