namespace Civs.Logic.Setup;

public sealed class StaticResourceBundle : ResourceBundleBase {
	public StaticResourceBundle( IEnumerable<(ResourceTypeBase, double)> requirements ) {
		foreach (var (resource, amount) in requirements)
			_resources[ resource ] = amount;
	}
}