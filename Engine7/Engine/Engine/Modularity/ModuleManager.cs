using Engine.Logging;
using Engine.Time;

namespace Engine.Modularity;

internal static class ModuleManager {

	private static Thread? _moduleMonitoringThread;
	private static readonly List<ModuleOverseer> _moduleOverseers = [];
	private static readonly List<IModuleModification> _currentModifications = [];

	internal static void Initialize() {
		if (_moduleOverseers.Count == 0)
			throw new InvalidOperationException( "No modules have been added." );
		Thread.CurrentThread.Name = "Entrypoint";
		Log.Line( "Starting engine..." );
		foreach (ModuleOverseer overseer in _moduleOverseers)
			overseer.Start();
		UpdatePeriod();
		_moduleMonitoringThread = new Thread( MonitorModules ) {
			Name = $"{nameof( ModuleManager )} Thread",
			IsBackground = true
		};
		_moduleMonitoringThread.Start();
		Log.Line( "Scanning for module modifications...", Log.Level.VERBOSE );
		IReadOnlyList<ModuleBase> modules = _moduleOverseers.Select( overseer => overseer.Module ).ToList().AsReadOnly();
		List<Type> mods = [ .. TypeManager.Registry.ImplementationTypes.Where( p => p.IsAssignableTo( typeof( IModuleModification ) ) && p.HasParameterlessConstructor( false ) ) ];
		Log.Line( mods.Count == 0 ? "Found no module mods." : $"Found {mods.Count} module mod{(mods.Count > 1 ? "s" : "")}! Initializing..." );
		foreach (Type? mod in mods) {
			IModuleModification? moduleModification = (IModuleModification?) mod.CreateInstance( null );
			if (moduleModification is null) {
				Log.Warning( $"- {mod.FullName} failed to initialize!" );
				continue;
			}
			Log.Line( $"- {mod.FullName}", Log.Level.VERBOSE );
			_currentModifications.Add( moduleModification );
			foreach (ModuleBase module in modules)
				moduleModification.ModifyModule( module );
		}
	}

	private static void UpdatePeriod() {
		uint lowestNonZeroPeriod = uint.MaxValue;
		foreach (ModuleOverseer overseer in _moduleOverseers) {
			if (overseer.PeriodMs > 0 && overseer.PeriodMs < lowestNonZeroPeriod)
				lowestNonZeroPeriod = overseer.PeriodMs;
		}
		TimePeriod.Begin( lowestNonZeroPeriod );
	}

	private static void MonitorModules() {
		IThreadBlocker threadBlocker = new ThreadBlocker();
		while (true) {
			for (int i = _moduleOverseers.Count - 1; i >= 0; i--) {
				if (_moduleOverseers[ i ].Running)
					continue;
				_moduleOverseers[ i ].Module.LogLine( "Shutdown registered." );
				_moduleOverseers[ i ].PeriodChanged -= UpdatePeriod;
				_moduleOverseers.RemoveAt( i );
			}
			if (_moduleOverseers.Count( p => p.Module.Important ) == 0)
				foreach (ModuleOverseer overseer in _moduleOverseers)
					overseer.Module.Stop();
			if (_moduleOverseers.Count == 0)
				break;
			threadBlocker.Block( 1000 );
		}
		Log.Line( "Engine is shuting down..." );
		threadBlocker.Block( 1000 );
		Log.Stop();
	}

	internal static void StartModule<T>() where T : ModuleBase, new() {
		ModuleOverseer overseer = new( new T() );
		_moduleOverseers.Add( overseer );
		overseer.PeriodChanged += UpdatePeriod;
	}

	public static IEnumerable<T> GetModules<T>() where T : ModuleBase
		=> _moduleOverseers.Select( overseer => overseer.Module ).OfType<T>();
}
