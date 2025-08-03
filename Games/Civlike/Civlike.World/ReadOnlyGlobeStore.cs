using System.Collections.Frozen;

namespace Civlike.World;

internal static class ReadOnlyGlobeStore {

	private static FrozenDictionary<int, WeakReference<ReadOnlyGlobe>>? _staticGlobes;
	private static readonly Lock _lock = new();

	public static ReadOnlyGlobe Get( int subdivisions ) {
		ReadOnlyGlobe? globe = null;
		lock (_lock) {
			if ((_staticGlobes?.TryGetValue( subdivisions, out WeakReference<ReadOnlyGlobe>? globeRef ) ?? false) && globeRef.TryGetTarget( out globe ))
				return globe;

			List<KeyValuePair<int, WeakReference<ReadOnlyGlobe>>> currentGlobes = _staticGlobes?.ToList() ?? [];

			globe = new( subdivisions );
			globe.Generate();

			currentGlobes.Add( new KeyValuePair<int, WeakReference<ReadOnlyGlobe>>( subdivisions, new( globe ) ) );
			_staticGlobes = currentGlobes.Where( p => p.Value.TryGetTarget( out _ ) ).ToDictionary().ToFrozenDictionary();
		}
		return globe;
	}
}
