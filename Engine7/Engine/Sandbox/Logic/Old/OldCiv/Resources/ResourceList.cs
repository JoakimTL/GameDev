namespace Sandbox.Logic.Old.OldCiv.Resources;

public static class ResourceList {

	private static readonly Dictionary<int, ResourceBase> _resources;
	private static readonly Dictionary<string, ResourceBase> _resourcesByResourceIdentifier;
	private static readonly Dictionary<string, IReadOnlyList<ResourceBase>> _resourcesByTag;

	static ResourceList() {
		_resources = [];
		_resourcesByResourceIdentifier = [];

		IEnumerable<Type> resourceTypes = TypeManager.Registry.ImplementationTypes.Where( p => p.BaseType == typeof( ResourceBase ) );

		foreach (Type resourceType in resourceTypes) {
			ResourceBase instance = resourceType.CreateInstance( null ) as ResourceBase ?? throw new InvalidOperationException( "Failed to construct resource type instance. Must have a parameterless constructor." );
			if (instance is IInitializable initializable)
				initializable.Initialize();
			if (_resources.TryGetValue( instance.Id, out ResourceBase? existing ))
				throw new InvalidOperationException( $"Resource type with id {instance.Id} already exists: {existing.ResourceIdentifier}." );
			_resources.Add( instance.Id, instance );
			if (_resourcesByResourceIdentifier.TryGetValue( instance.ResourceIdentifier, out ResourceBase? existingByResourceId ))
				throw new InvalidOperationException( $"Resource type with resource identifier {instance.ResourceIdentifier} already exists: {existingByResourceId.ResourceIdentifier}." );
			_resourcesByResourceIdentifier.Add( instance.ResourceIdentifier, instance );
		}

		_resourcesByTag = [];
		foreach (IGrouping<string, (string Tag, ResourceBase Resource)> tagGroup in _resources.Values.SelectMany( p => p.Tags.Select( t => (Tag: t, Resource: p) ) ).GroupBy( p => p.Tag ))
			_resourcesByTag.Add( tagGroup.Key, tagGroup.Select( p => p.Resource ).ToList().AsReadOnly() );
	}

	public static ResourceBase GetResource( int id ) {
		if (!_resources.TryGetValue( id, out ResourceBase? resource ))
			throw new InvalidOperationException( $"Resource with id {id} not found." );
		return resource;
	}

	public static ResourceBase GetResource( string resourceIdentifier ) {
		if (!_resourcesByResourceIdentifier.TryGetValue( resourceIdentifier, out ResourceBase? resource ))
			throw new InvalidOperationException( $"Resource with resource identifier {resourceIdentifier} not found." );
		return resource;
	}

	public static IReadOnlyList<ResourceBase> GetResourcesByTag( string tag ) {
		if (!_resourcesByTag.TryGetValue( tag, out IReadOnlyList<ResourceBase>? resources ))
			throw new InvalidOperationException( $"No resources found with tag {tag}." );
		return resources;
	}
}