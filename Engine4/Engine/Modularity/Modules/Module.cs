using System.Collections.Concurrent;
using Engine.Structure;
using Engine.Time;

namespace Engine.Modularity.Modules;
public sealed class Module : DisposableIdentifiable {

	public uint ModuleId { get; }
	public bool Essential { get; }
	private bool _shutdown;
	private readonly ConcurrentQueue<Type> _incoming;
	private readonly Queue<Submodule> _outgoing;
	private readonly UpdateableSingletonProvider<Submodule> _submodules;
	private readonly Timer32 _internalTimer;
	private readonly ModuleSingletonProvider _singletonProvider;
	private float _lastUpdate;

	public event Action<Module>? OnDisposal;

	public Module( string modulename, uint moduleId, bool essential, int delay = 0 ) : base( $"{modulename}:{moduleId}" ) {
		this.ModuleId = moduleId;
		this.Essential = essential;
		this._incoming = new();
		this._outgoing = new();
		this._submodules = new();
		this._submodules.SingletonRemoved += SubmoduleRemoved;
		this._internalTimer = new( this.FullName, delay, false );
		this._internalTimer.Elapsed += Update;
		this._singletonProvider = new( this );
		this._shutdown = false;
	}

	private void SubmoduleRemoved( Type obj ) => this._shutdown = !this._submodules.GetAll().OfType<Submodule>().Any( p => p.Essential );

	public T Singleton<T>() where T : ModuleSingletonBase => this._singletonProvider.Get<T>();

	public void AddSubmodule( Type submoduleType ) => this._submodules.Get( submoduleType );

	internal void Initialize() => this._internalTimer.Start();

	private void Update() {
		float time = Clock32.StartupTime;
		float deltaTime = time - this._lastUpdate;
		this._lastUpdate = time;

		this._singletonProvider.Update( time, deltaTime );
		this._submodules.Update( time, deltaTime );

		if ( this._shutdown ) {
			this.LogLine( "No more essential submodules. Disposing.", Log.Level.CRITICAL );
			Dispose();
		}
	}

	protected override bool OnDispose() {
		this._internalTimer.Dispose();
		this._submodules.Dispose();
		this._singletonProvider.Dispose();
		OnDisposal?.Invoke( this );
		return true;
	}

}
