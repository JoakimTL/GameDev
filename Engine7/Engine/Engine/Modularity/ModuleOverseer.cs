using Engine.Logging;
using Engine.Time;

namespace Engine.Modularity;

internal class ModuleOverseer {
	private readonly TimedThreadBlocker<StopwatchTickSupplier> _blocker;
	private readonly Thread _moduleThread;
	public bool Running { get; private set; }
	public uint PeriodMs => this._blocker.PeriodMs;
	public event Action? PeriodChanged;
	public ModuleBase Module { get; }

	public ModuleOverseer( ModuleBase module ) {
		this.Module = module;
		this.Module.FrequencyAltered += SetNewDelay;
		this._blocker = new( new ThreadBlocker(), 1000 );
		SetNewDelay();
		this._moduleThread = new Thread( RunModule ) {
			Name = $"{module.GetType().Name} Thread",
			IsBackground = !this.Module.Important
		};
	}

	internal void Start() => this._moduleThread.Start();

	private void RunModule() {
		this.Running = true;
		this.Module.LogLine( "Starting module..." );
		this.Module.Initialize();
		while (this._blocker.Block() != TimedBlockerState.Cancelled) {
			if (!this.Module.DoTick())
				this._blocker.Cancel();
		}
		this.Module.LogLine( "Shutting down module..." );
		this.Module.Dispose();
		this.Running = false;
	}

	private void SetNewDelay() {
		uint newDelay = this.Module.ExecutionFrequency.ToPeriodMs();
		if (newDelay == this._blocker.PeriodMs)
			return;
		this._blocker.SetPeriod( newDelay );
		PeriodChanged?.Invoke();
	}
}
