namespace Engine.Structure;

public class ServiceProviderInitializableExtension<BASE> : HierarchicalServiceProviderExtension<BASE>, IUpdateable where BASE : class {
	private readonly ServiceProvider<BASE> _serviceProvider;
	public bool Active => true;

	public ServiceProviderInitializableExtension( ServiceProvider<BASE> serviceProvider ) : base( typeof( IInitializable ) ) {
		this._serviceProvider = serviceProvider;
		this._serviceProvider.ServiceAdded += OnServiceAdded;
		this._serviceProvider.ServiceRemoved += OnServiceRemoved;
	}

	private void OnServiceAdded( object obj ) {
		if ( obj is IInitializable )
			this._tree.Add( obj.GetType() );
	}

	private void OnServiceRemoved( object obj ) {
		if ( obj is IInitializable )
			this._tree.Remove( obj.GetType() );
	}

	public void Update( float time, float deltaTime ) {
		if ( this._tree.Update() ) {
			foreach ( IInitializable initializable in this._serviceProvider.PeekAll( this._tree.GetNodesSorted() ).OfType<IInitializable>() )
				initializable.Initialize();
			this._tree.Clear();
		}
	}

	protected override bool OnDispose() => true;
}
