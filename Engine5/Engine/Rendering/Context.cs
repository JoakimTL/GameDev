using Engine.Rendering.Contexts.Services.Assets;
using Engine.Rendering.OGL;
using Engine.Structure;
using Engine.Structure.Interfaces;
using Engine.Structure.ServiceProvider;
using OpenGL;

namespace Engine.Rendering;

public sealed class Context : Identifiable, IUpdateable, IInitializable
{
	private readonly RestrictedServiceProvider<IContextService> _serviceProvider;
	private readonly ServiceProviderDisposalExtension _serviceProviderDisposer;
	private readonly ServiceProviderUpdateExtension _serviceProviderUpdater;
	private readonly ServiceProviderInitializationExtension _serviceProviderInitializer;
	private readonly Window _window;
	private Thread? _contextThread = null;

	public Viewport Viewport { get; }
	public IDependencyInjector Injector { get; }

	public Context(Window window)
	{
		_window = window;
		_serviceProvider = new();
        _serviceProvider.AddConstant(_window);
        _serviceProvider.AddConstant(this);
        _serviceProvider.AddConstant(Viewport = new());
        _serviceProviderUpdater = new(_serviceProvider);
		_serviceProviderDisposer = new(_serviceProvider);
		_serviceProviderInitializer = new(_serviceProvider);
		Injector = new DependencyInjector(this);
	}

	public void Bind()
	{
		ContextUtilities.MakeContextCurrent(_window.Pointer);
		_contextThread = Thread.CurrentThread;
	}

	public bool IsContextThread() => Thread.CurrentThread == _contextThread;

	private object? Service(Type t) => _serviceProvider.Get(t);
	public T Service<T>() where T : IContextService => _serviceProvider.Get<T>();

    public void Initialize()
    {
        _serviceProvider.Get<AssetSceneObjectManagerService>();
        foreach (GetPName paramName in Enum.GetValues<GetPName>())
        {
            Gl.GetInteger(paramName, out uint value);
            this.LogLine($"{paramName}: {value}", Log.Level.NORMAL);
        }
    }

	public void Update(float time, float deltaTime)
	{
		_serviceProviderInitializer.Update(time, deltaTime);
		_serviceProviderUpdater.Update(time, deltaTime);
	}

	public void Dispose() => _serviceProviderDisposer.Dispose();


    public sealed class DependencyInjector : DependencyInjectorBase, IDependencyInjector
	{
		private readonly Context _context;

		public DependencyInjector(Context context)
        {
			_context = context;
		}

		public object? Get(Type type) => this.Create(type, false);

        protected override object? GetInternal(Type t) => _context.Service(t);
    }
}
