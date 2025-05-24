namespace Civlike.Logic.Setup;

public abstract class ResourceBundleBase {
	protected readonly Dictionary<ResourceTypeBase, double> _resources = [];
	public IReadOnlyDictionary<ResourceTypeBase, double> Resources => this._resources;

	public bool HasEnough( ResourceBundleBase requirements ) {
		foreach (KeyValuePair<ResourceTypeBase, double> kvp in requirements.Resources)
			if (!this._resources.TryGetValue( kvp.Key, out double amount ) || amount < kvp.Value)
				return false;
		return true;
	}

	public double GetAmount( ResourceTypeBase resource ) => this._resources.GetValueOrDefault( resource );
}

