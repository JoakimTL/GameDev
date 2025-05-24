namespace Civlike.Logic.Setup;

public sealed class StaticResourceBundle : ResourceBundleBase {
	public StaticResourceBundle( IEnumerable<(ResourceTypeBase, double)> requirements ) {
		foreach ((ResourceTypeBase resource, double amount) in requirements)
			this._resources[ resource ] = amount;
	}
}