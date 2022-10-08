using System.Reflection;
using Engine.Structure;

namespace Engine.Modularity.Domains;

public abstract class Module : DisposableIdentifiable, IRemoveable, IUpdateable, IInitializable {
	public bool Essential { get; }
	public bool Active { get; protected set; }
	/// <summary>
	/// Allows the module to hide itself from other modules in a domain. The domain will not allow access to hidden modules through the domain.
	/// </summary>
	public bool IsVisible { get; protected set; }
	public int Frequency { get; }
	public bool Initialized { get; private set; }
	public Domain Domain { get; private set; } = null!;

	private readonly ServiceProvider<ModuleService> _serviceProvider;
	private readonly PropertyInfo _serviceModuleProperty;
	public event Action<IRemoveable>? Removed;

	/// <param name="essential">If a module is essential, it will stop the shutdown of a the encompassing domain</param>
	/// <param name="frequency">The desired frequency of updates. If this number is negative the module will not be updated automatically</param>
	public Module( bool essential, int frequency ) {
		this.Essential = essential;
		this.Frequency = frequency;
		this.Active = true;
		this._serviceProvider = new();
		this._serviceModuleProperty = typeof( ModuleService ).GetProperty( "Module" ) ?? throw new InvalidOperationException( "Service Module property not found!" );
		this._serviceProvider.ServiceAdded += ServiceAdded;
	}

	private void ServiceAdded( object obj ) {
		if ( obj is ModuleService ds )
			this._serviceModuleProperty.SetValue( ds, this );
	}

	public T ModuleService<T>() where T : ModuleService => this._serviceProvider.GetOrAdd<T>();
	public T DomainService<T>() where T : DomainService => this.Domain.Service<T>();

	public void Update( float time, float deltaTime ) {
		if ( !this.Initialized ) {
			Initialize();
			this.Initialized = true;
		}
		if ( this.Active ) {
			this._serviceProvider.Update( time, deltaTime );
			ModuleUpdate( time, deltaTime );
		}
		if ( !this.Active )
			Dispose();
	}

	public virtual void Initialize() { }
	protected virtual void ModuleUpdate( float time, float deltaTime ) { }

	protected override bool OnDispose() {
		this._serviceProvider.Dispose();
		Removed?.Invoke( this );
		return true;
	}

}
