//using Engine.Structure;

//namespace Engine.Modularity.Modules;

//public abstract class Submodule : DisposableIdentifiable, IRemoveable, IUpdateable {

//	public delegate void SubmoduleUpdateHandler( float time, float deltaTime );

//	private Module? _parent;
//	protected Module Parent => this._parent ?? throw new NullReferenceException( "Parent module can't be null!" );
//	public bool Essential { get; }
//	public bool Active { get; private set; }

//	private bool _initialized;

//	protected event Action? OnParentSet;
//	protected event Action? OnInitialization;
//	protected event SubmoduleUpdateHandler? OnUpdate;
//	public event Action<IRemoveable>? Removed;

//	protected Submodule( bool essential ) {
//		this.Active = true;
//		this.Essential = essential;
//		this._initialized = false;
//	}

//	internal void SetParentModule( Module module ) {
//		if ( this._parent is not null )
//			throw new Exception( "Cannot set parent module when parent module already has been set." );
//		if ( module is null )
//			throw new ArgumentNullException( nameof( module ) );
//		this._parent = module;
//		OnParentSet?.Invoke();
//	}

//	protected void Remove() {
//		this.Active = false;
//		Removed?.Invoke( this );
//	}

//	public void Update( float time, float deltaTime ) {
//		if ( !this._initialized ) {
//			this.Active = true;
//			OnInitialization?.Invoke();
//			this._initialized = true;
//			return;
//		}
//		if ( !this.Active )
//			return;
//		OnUpdate?.Invoke( time, deltaTime );
//	}
//}