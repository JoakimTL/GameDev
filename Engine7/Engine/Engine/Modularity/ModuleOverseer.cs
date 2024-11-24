using Engine.Logging;
using Engine.Time;

namespace Engine.Modularity;

internal class ModuleOverseer {

	private readonly ModuleBase _module;
	private readonly TimedThreadBlocker<StopwatchTickSupplier> _blocker;
	private readonly Thread _moduleThread;
	public bool Running { get; private set; }
	public uint PeriodMs => this._blocker.PeriodMs;
	public event Action? PeriodChanged;

	public ModuleOverseer( ModuleBase module ) {
		this._module = module;
		this._module.FrequencyAltered += SetNewDelay;
		this._blocker = new( new ThreadBlocker(), 1000 );
		SetNewDelay();
		this._moduleThread = new Thread( RunModule ) {
			Name = $"{GetType().Name} Thread",
			IsBackground = !this._module.Important
		};
	}

	internal void Start() => this._moduleThread.Start();

	private void RunModule() {
		this.Running = true;
		this._module.LogLine( "Starting module..." );
		this._module.Initialize();
		while (this._blocker.Block() != TimedBlockerState.Cancelled) {
			if (!this._module.DoTick())
				this._blocker.Cancel();
		}
		this._module.LogLine( "Shutting down module..." );
		this._module.Dispose();
		this.Running = false;
	}

	private void SetNewDelay() {
		uint newDelay = this._module.ExecutionFrequency.ToPeriodMs();
		if (newDelay == this._blocker.PeriodMs)
			return;
		this._blocker.SetPeriod( newDelay );
		PeriodChanged?.Invoke();
	}
}
