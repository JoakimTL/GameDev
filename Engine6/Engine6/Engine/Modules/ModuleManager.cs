using Engine.Time;
using System.Reflection;

namespace Engine.Modules;

public sealed class ModuleManager {

	//When all active primary modules are "closed" we should close the remaining modules and dispose.

	private readonly Thread _observeThread;
	private readonly AutoResetEvent _shutdownEvent = new( false );
	private readonly List<ModuleThread> _modules;
	private readonly double _graceTime;
	private readonly uint _observeDelayMs;
	private readonly Clock<double, StopwatchTickSupplier> _observerClock;
	private volatile bool _initialized;

	public ModuleManager( double graceTime = 1, double observeFrequency = 1 ) {
		_modules = [];
		_observerClock = new( 1 );
		_initialized = false;
		_observeThread = new( ObserveActivity ) {
			IsBackground = false,
			Name = "Module Observer"
		};
		_observeThread.Start();
		this._graceTime = graceTime;
		this._observeDelayMs = observeFrequency.ToPeriodMs( 0.05, 500 );
	}

	private void ObserveActivity() {
		while (true) {
			if (!_shutdownEvent.WaitOne( (int) _observeDelayMs )) {
				if (_observerClock.Time > _graceTime && !_initialized) {
					this.LogWarning( "No initialization has taken place before grace time ended." );
					break;
				}
				continue;
			}
			//A shutdown has occured!
			lock (_modules) {
				for (int ir = _modules.Count - 1; ir >= 0; ir--) {
					if (!_modules[ ir ].Alive) {
						_modules[ ir ].Module.MessageEmitted -= OnMessageReceived;
						this.LogLine( $"Module [{_modules[ ir ]}] was shut down!" );
						_modules.RemoveAt( ir );
					}
				}
				this.LogLine( $"Modules remaining: {_modules.Count( p => p.PrimaryModule )} primary, {_modules.Count( p => !p.PrimaryModule )} background" );
				//At this point all modules in the list are alive.
				if (!_modules.Any( m => m.PrimaryModule ))
					break;
			}
		}
		this.LogLine( "No active primary modules. Shutting down application." );
		ShutdownAllModules();
	}

	private void OnMessageReceived( object obj ) {
		lock (_modules) {
			foreach (ModuleThread m in _modules) {
				if (m.Alive)
					m.Module.MessageReceived( obj );
			}
		}
	}

	private void ShutdownAllModules() {
		lock (_modules) {
			foreach (ModuleThread m in _modules) {
				m.Module.MessageEmitted -= OnMessageReceived;
				m.Stop();
			}
		}
		Log.Stop();
		//Log.Dispose();
	}

	public void Start( ModuleBase o ) {
		ModuleThread m = new( o );
		m.Shutdown += OnShutdown;
		m.Module.MessageEmitted += OnMessageReceived;
		lock (_modules)
			_modules.Add( m );
		_initialized |= m.PrimaryModule;
		m.Start();
	}

	/// <summary>
	/// This happens on the module's thread.
	/// </summary>
	private void OnShutdown() => _shutdownEvent.Set();

	private sealed class ModuleThread {
		public ModuleBase Module { get; }
		private readonly Thread _thread;
		public event Action? Shutdown;
		public bool PrimaryModule { get; }
		public bool Alive => Module.Alive;

		public ModuleThread( ModuleBase module ) {
			Module = module;
			_thread = new( Run );
			Type type = module.GetType();
			ThreadPriorityAttribute? priorityAttribute = type.GetCustomAttribute<ThreadPriorityAttribute>();
			if (priorityAttribute is not null)
				_thread.Priority = priorityAttribute.Priority;
			_thread.IsBackground = type.GetCustomAttribute<PrimaryModuleAttribute>() is null;
			_thread.Name = type.Name.Replace( "Module", "" );
			PrimaryModule = !_thread.IsBackground;
		}

		private void Run() {
			Module.Run();
			//Module is done running.
			if (!Module.Disposed) {
				Module.LogError( "Not disposed but done executing." );
				Module.Dispose();
			}
			Shutdown?.Invoke();
		}

		internal void Start()
			=> _thread.Start();

		public void Stop()
			=> Task.Run( () => {
				Module.Stop();
				_thread.Join();
			} );
	}

}