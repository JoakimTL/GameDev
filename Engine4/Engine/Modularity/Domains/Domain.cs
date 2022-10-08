using System.Collections.Concurrent;
using System.Reflection;
using Engine.Structure;

namespace Engine.Modularity.Domains;

public abstract class Domain : DisposableIdentifiable {

	private readonly ConcurrentDictionary<Type, Module> _modules;
	private readonly List<ModuleUpdater> _moduleUpdaters;
	private readonly ServiceProvider<DomainService> _serviceProvider;
	private readonly PropertyInfo _moduleDomainProperty;
	private readonly PropertyInfo _serviceDomainProperty;

	public Domain( string name ) : base( name ) {
		this._modules = new();
		this._serviceProvider = new();
		this._moduleUpdaters = new();
		this._moduleDomainProperty = typeof( Module ).GetProperty( "Domain" ) ?? throw new InvalidOperationException( "Module Domain property not found!" );
		this._serviceDomainProperty = typeof( DomainService ).GetProperty( "Domain" ) ?? throw new InvalidOperationException( "Service Domain property not found!" );
		this._serviceProvider.ServiceAdded += ServiceAdded;
	}

	private void ServiceAdded( object obj ) {
		if ( obj is DomainService ds )
			this._serviceDomainProperty.SetValue( ds, this );
	}

	private void ModuleDisposed( object obj ) {
		this._modules.TryRemove( obj.GetType(), out _ );
		if ( this._modules.Values.Any( p => p.Disposed ) )
			this.LogError( $"Disposed modules still remains: ({string.Join( ", ", this._modules.Values.Where( p => p.Disposed ) )})" );
		if ( !this._modules.Values.Any( p => p.Essential && !p.Disposed ) ) {
			Dispose();
			Shutdown();
		}
	}

	protected void Add( Module module ) {
		if ( this._modules.TryAdd( module.GetType(), module ) ) {
			if ( module.GetType().GetCustomAttribute<SystemAttribute>() is not null )
				this._moduleUpdaters.Add( new ModuleUpdater( module ) );
			module.OnDisposed += ModuleDisposed;
			this._moduleDomainProperty.SetValue( module, this );
		}
	}

	/// <summary>
	/// Gets a specific module contained in the domain if it exists and is visible.
	/// </summary>
	public T? Module<T>() where T : Module {
		if ( this._modules.TryGetValue( typeof( T ), out Module? module ) && module.IsVisible && module is T t )
			return t;
		return null;
	}

	/// <summary>
	/// Gets a service on domain level. These services are not updated, as this is the role of modules.
	/// </summary>
	public T Service<T>() where T : DomainService => this._serviceProvider.GetOrAdd<T>();

	private void Shutdown() {
		this.LogLine( "Shutting down.", Log.Level.HIGH );
		throw new NotImplementedException();
	}
}
