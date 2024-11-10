using Engine.Time;

namespace Engine.Modularity;

internal class ModuleOverseer {

	private readonly ModuleBase _module;
	private readonly TimedThreadBlocker<StopwatchTickSupplier> _blocker;
	private readonly Thread _moduleThread;
	public bool Running { get; private set; }
	public uint PeriodMs => _blocker.PeriodMs;
	public event Action? PeriodChanged;

	public ModuleOverseer( ModuleBase module ) {
		_module = module;
		_module.FrequencyAltered += SetNewDelay;
		_blocker = new( new ThreadBlocker(), 1000 );
		SetNewDelay();
		_moduleThread = new Thread( RunModule ) {
			Name = $"{GetType().Name} Thread",
			IsBackground = !_module.Important
		};
	}

	internal void Start() => _moduleThread.Start();

	private void RunModule() {
		Running = true;
		_module.LogLine( "Starting module..." );
		while (_blocker.Block() != TimedBlockerState.Cancelled) {
			if (!_module.DoTick())
				_blocker.Cancel();
		}
		_module.LogLine( "Shutting down module..." );
		_module.Dispose();
		Running = false;
	}

	private void SetNewDelay() {
		uint newDelay = _module.ExecutionFrequency.ToPeriodMs();
		if (newDelay == _blocker.PeriodMs)
			return;
		_blocker.SetPeriod( newDelay );
		PeriodChanged?.Invoke();
	}
}
