using Engine.Structure;
using Engine.Structure.Interfaces;
using Engine.Time;

namespace Engine.GlobalServices;

public class ModuleContainerService : IGlobalService
{

    private TickingTimer _moduleTickTimer;

    private HashSet<ModuleBase> _modules;
    private Dictionary<ModuleBase, ModuleSystemTickerBase> _moduleTickers;

    public ModuleContainerService()
    {
        _modules = new();
        _moduleTickers = new();
        _moduleTickTimer = new("Module Tick Timer", 1000, true);
        _moduleTickTimer.Elapsed += CheckModules;
        _moduleTickTimer.Start();
    }

    private void CheckModules(double time, double deltaTime)
    {
        lock (_modules)
        {
            var inactiveModules = _modules.Where(p => !p.Active);
            foreach (var module in inactiveModules)
            {
                _modules.Remove(module);
                if (module is ISystem && module is IDisposable disposable)
                    disposable.Dispose();
            }

            if (!_modules.Any(p => p.Essential))
                foreach (var m in _modules)
                    m.ForceStop();

            foreach (var kvp in _moduleTickers.Where(p => !p.Key.Active))
                _moduleTickers.Remove(kvp.Key);

            if (_modules.Count == 0 && _moduleTickers.Count == 0)
                Shutdown();
        }
    }

    private void Shutdown()
    {
        _moduleTickTimer.Stop();
        Global.Shutdown();
    }

    internal void Add(ModuleBase moduleBase)
    {
        lock (_modules)
            if (_modules.Add(moduleBase))
            {
                if (moduleBase is ISystem system)
                {
                    ITimedSystem? timedSystem = moduleBase as ITimedSystem;
                    ModuleSystemTickerBase ticker = timedSystem is not null
                        ? new ModuleSystemTickerBase.ModuleSystemTickerTimed(moduleBase, system.SystemEssential, timedSystem)
                        : new ModuleSystemTickerBase.ModuleSystemTickerContinous(moduleBase, system.SystemEssential);
                    _moduleTickers.Add(moduleBase, ticker);
                }
            }
    }

    private abstract class ModuleSystemTickerBase : IUpdateable, IDisposable
    {
        private ModuleBase _moduleBase;
        private IInitializable? _initializableModule;
        private IUpdateable? _updateableModule;
        private IDisposable? _disposableModule;

        public ModuleSystemTickerBase(ModuleBase moduleBase)
        {
            _moduleBase = moduleBase;
            _initializableModule = moduleBase as IInitializable;
            _updateableModule = moduleBase as IUpdateable;
            _disposableModule = moduleBase as IDisposable;
        }

        public abstract void Dispose();

        public void Update(float time, float deltaTime)
        {
            if (_moduleBase.Active)
                try
                {
                    if (_initializableModule is not null)
                    {
                        _initializableModule.Initialize();
                        _initializableModule = null;
                    }
                    if (_updateableModule is not null)
                        _updateableModule.Update(time, deltaTime);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            else
            {
                if (_disposableModule is not null)
                {
                    _disposableModule.Dispose();
                    _disposableModule = null;
                }
                Dispose();
            }
        }

        public class ModuleSystemTickerTimed : ModuleSystemTickerBase
        {

            private int _interval;
            private readonly TickingTimer _timer;
            private readonly ITimedSystem _timedSystem;

            public ModuleSystemTickerTimed(ModuleBase moduleBase, bool essential, ITimedSystem timedSystem) : base(moduleBase)
            {
                this._timedSystem = timedSystem;
                _interval = _timedSystem.SystemTickInterval;
                _timer = new TickingTimer(moduleBase.Name, _interval, !essential);
                _timer.Elapsed += Tick;
                _timer.Start();
            }

            public override void Dispose()
            {
                _timer.Stop();
            }

            private void Tick(double time, double deltaTime)
            {
                if (_interval != _timedSystem.SystemTickInterval)
                {
                    _interval = _timedSystem.SystemTickInterval;
                    _timer.SetInterval(_interval);
                    Console.WriteLine("Time interval changed!");
                }
                Update((float)time, (float)deltaTime);
            }
        }

        public class ModuleSystemTickerContinous : ModuleSystemTickerBase
        {

            private bool _active;
            private readonly Thread _thread;
            private float _lastTick;

            public ModuleSystemTickerContinous(ModuleBase moduleBase, bool essential) : base(moduleBase)
            {
                _active = true;
                _thread = Global.Get<ThreadService>().Start(ModuleThreadRun, moduleBase.Name, !essential);
            }

            private void ModuleThreadRun()
            {
                while (_active)
                {
                    float tickTime = Clock32.StartupTime;
                    float deltaTime = tickTime - _lastTick;
                    _lastTick = tickTime;
                    Update(tickTime, deltaTime);
                }
            }

            public override void Dispose()
            {
                _active = false;
            }
        }
    }
}
