using Engine.Time;

namespace Engine.Modularity.Domains;

internal class ModuleUpdater : DisposableIdentifiable {

	private readonly Timer32 _timer;
	private readonly Module _module;

	public ModuleUpdater( Module module ) {
		this._module = module;
		this._module.OnDisposed += ModuleDisposed;
		this._timer = new( this._module.Name, this._module.Frequency > 0 ? 1000 / this._module.Frequency : 0 );
		this._timer.Elapsed += TimerUpdate;
		this._timer.Start();
	}

	private void ModuleDisposed( object obj ) => Dispose();

	private void TimerUpdate( double time, double deltaTime ) {
		this._module.Update( (float) time, (float) deltaTime );
		if ( !this._module.Active )
			Dispose();
	}

	protected override bool OnDispose() {
		this._timer.Dispose();
		return true;
	}
}
