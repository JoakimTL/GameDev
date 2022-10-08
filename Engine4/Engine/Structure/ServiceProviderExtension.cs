namespace Engine.Structure;

public abstract class ServiceProviderExtension<BASE> : DisposableIdentifiable {}
public abstract class HierarchicalServiceProviderExtension<BASE> : ServiceProviderExtension<BASE> {
	protected readonly BidirectionalTreeStructureProvider _tree;
	public HierarchicalServiceProviderExtension( Type treeType ) {
		this._tree = new( treeType );
	}
}
