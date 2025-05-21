namespace Civlike.Logic.Setup;

public sealed class StaticResourceBundle : ResourceBundleBase {
	public StaticResourceBundle( IEnumerable<(ResourceTypeBase, double)> requirements ) {
		foreach ((ResourceTypeBase resource, double amount) in requirements)
			_resources[ resource ] = amount;
	}
}