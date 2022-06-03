namespace Engine.Structure;
public class UpdateableSingletonProvider<BASE> : SingletonProvider<BASE>, IUpdateable {

	private readonly BidirectionalTreeStructureProvider _updateableTypeTree;
	private readonly List<IUpdateable> _sortedUpdateables;
	public bool Active => true;

	public UpdateableSingletonProvider() {
		this._sortedUpdateables = new();
		this._updateableTypeTree = new( typeof( IUpdateable ) );
		this._updateableTypeTree.TreeUpdated += TreeUpdated;
		SingletonAdded += CheckUpdateable;
		SingletonRemoved += OnSingletonRemoved;
	}

	private void OnSingletonRemoved( Type type ) {
		lock ( this._updateableTypeTree )
			this._updateableTypeTree.Remove( type );
	}

	private void TreeUpdated() {
		this._sortedUpdateables.Clear();
		this._sortedUpdateables.AddRange( this._updateableTypeTree.WalkTreeForward().Select( GetOrDefault ).OfType<IUpdateable>() );
	}

	private void CheckUpdateable( object obj ) {
		if ( obj is IUpdateable )
			this._updateableTypeTree.Add( obj.GetType() );
	}

	public void Update( float time, float deltaTime ) {
		lock ( this._updateableTypeTree )
			this._updateableTypeTree.Update();
		foreach ( IUpdateable updateable in this._sortedUpdateables )
			if ( updateable.Active )
				updateable.Update( time, deltaTime );
	}
}
