namespace Civlike.Logic.Setup;

public abstract class ResourceBundleBase {
	protected readonly Dictionary<ResourceTypeBase, double> _resources = [];
	public IReadOnlyDictionary<ResourceTypeBase, double> Resources => _resources;

	public bool HasEnough( ResourceBundleBase requirements ) {
		foreach (var kvp in requirements.Resources)
			if (!_resources.TryGetValue( kvp.Key, out double amount ) || amount < kvp.Value)
				return false;
		return true;
	}

	public double GetAmount( ResourceTypeBase resource ) => _resources.GetValueOrDefault( resource );
}

