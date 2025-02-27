namespace Sandbox.Logic.Setup;

public sealed class ResourceContainer {

	private readonly Dictionary<ResourceTypeBase, double> _resources;

	public ResourceContainer() {
		_resources = [];
	}

	public void ChangeResourceAmount( ResourceTypeBase resource, double amount ) {
		if (amount == 0)
			return;
		if (!_resources.ContainsKey( resource ))
			_resources.Add( resource, 0 );
		_resources[ resource ] += amount;
	}

	public void SetResourceAmount( ResourceTypeBase resource, double amount ) => _resources[ resource ] = amount;

	public double GetResourceAmount( ResourceTypeBase resource ) => _resources.TryGetValue( resource, out double amount ) ? amount : 0;

	public void ConsumeResources( ResourceTable consumption ) {
		foreach (ResourceAmount resourceAmount in consumption.Resources)
			ChangeResourceAmount( resourceAmount.Resource, -resourceAmount.AmountKg );
	}
	public void AddResources( ResourceTable addition ) {
		foreach (ResourceAmount resourceAmount in addition.Resources)
			ChangeResourceAmount( resourceAmount.Resource, resourceAmount.AmountKg );
	}

	public ResourceTable GetAllResources() => new( _resources.Select( p => new ResourceAmount( p.Key, p.Value ) ).ToList() );

	public void UpdateTableValues( ResourceTable table ) {
		foreach (KeyValuePair<ResourceTypeBase, double> kvp in _resources) {
			ResourceAmount? resourceAmount = table.Resources.FirstOrDefault( p => p.Resource == kvp.Key );
			if (resourceAmount == null)
				table.AddResource( new( kvp.Key, kvp.Value ) );
			else
				resourceAmount.AmountKg = kvp.Value;
		}
	}

}
