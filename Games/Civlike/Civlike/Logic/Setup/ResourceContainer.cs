namespace Civlike.Logic.Setup;

public sealed class ResourceContainer : ResourceBundleBase {

	private readonly Dictionary<ResourceTypeBase, double> _resourceLimits = [];

	public void SetLimit( ResourceTypeBase resource, double limit ) {
		if (limit < 0) {
			this._resourceLimits.Remove( resource );
			return;
		}
		this._resourceLimits[ resource ] = limit;
	}

	public bool Change( ResourceTypeBase resource, double amount ) {
		double existingAmount = this._resources.GetValueOrDefault( resource );
		double newAmount = existingAmount + amount;
		if (newAmount < 0)
			return false;
		double limit = this._resourceLimits.GetValueOrDefault( resource, double.MaxValue );
		if (newAmount > limit)
			newAmount = limit;
		this._resources[ resource ] = newAmount;
		return true;
	}

	public bool Subtract( ResourceBundleBase amounts ) {
		if (!HasEnough( amounts ))
			return false;
		foreach (KeyValuePair<ResourceTypeBase, double> kvp in amounts.Resources)
			Change( kvp.Key, -kvp.Value );
		return true;
	}

	public void Add( ResourceBundleBase amounts ) {
		foreach (KeyValuePair<ResourceTypeBase, double> kvp in amounts.Resources)
			Change( kvp.Key, kvp.Value );
	}

	public void AddAllExcept( ResourceBundleBase amounts, IReadOnlyList<ResourceTypeBase> exceptions ) {
		foreach (KeyValuePair<ResourceTypeBase, double> kvp in amounts.Resources)
			if (!exceptions.Contains( kvp.Key ))
				Change( kvp.Key, kvp.Value );
	}
}

