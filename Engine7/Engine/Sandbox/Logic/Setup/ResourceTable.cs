namespace Sandbox.Logic.Setup;

public sealed class ResourceTable {
	private readonly List<ResourceAmount> _resources;

	public ResourceTable( params List<ResourceAmount> resources ) {
		this._resources = resources;
	}

	public void AddResource( ResourceAmount resourceAmount ) => _resources.Add( resourceAmount );

	public IReadOnlyList<ResourceAmount> Resources => _resources;
}
