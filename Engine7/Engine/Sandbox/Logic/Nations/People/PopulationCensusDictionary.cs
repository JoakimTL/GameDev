namespace Sandbox.Logic.Nations.People;

public sealed class PopulationCensusDictionary {

	private readonly Dictionary<PhysicalTrait, Dictionary<ushort, Dictionary<ushort, Dictionary<ushort, Population>>>> _popsByPhysicalTraits;

	public PopulationCensusDictionary() {
		_popsByPhysicalTraits = [];
	}

	public PopulationWithKey GetOrCreate( PopulationKey key ) {
		if (!_popsByPhysicalTraits.TryGetValue( key.PhysicalTrait, out Dictionary<ushort, Dictionary<ushort, Dictionary<ushort, Population>>>? popByPopulationCenter ))
			_popsByPhysicalTraits.Add( key.PhysicalTrait, popByPopulationCenter = [] );
		if (!popByPopulationCenter.TryGetValue( key.BirthPopulationCenterId, out Dictionary<ushort, Dictionary<ushort, Population>>? popByCulture ))
			popByPopulationCenter.Add( key.BirthPopulationCenterId, popByCulture = [] );
		if (!popByCulture.TryGetValue( key.CultureId, out Dictionary<ushort, Population>? popByYear ))
			popByCulture.Add( key.CultureId, popByYear = [] );
		if (!popByYear.TryGetValue( key.BirthYear, out Population? population ))
			popByYear.Add( key.BirthYear, population = new() );
		return new PopulationWithKey( population, key );
	}

	public bool TryGet( PopulationKey key, out PopulationWithKey? populationWithKey ) {
		populationWithKey = null;
		if (!_popsByPhysicalTraits.TryGetValue( key.PhysicalTrait, out Dictionary<ushort, Dictionary<ushort, Dictionary<ushort, Population>>>? popByPopulationCenter ))
			return false;
		if (!popByPopulationCenter.TryGetValue( key.BirthPopulationCenterId, out Dictionary<ushort, Dictionary<ushort, Population>>? popByCulture ))
			return false;
		if (!popByCulture.TryGetValue( key.CultureId, out Dictionary<ushort, Population>? popByYear ))
			return false;
		if (!popByYear.TryGetValue( key.BirthYear, out Population? population ))
			return false;
		populationWithKey = new( population, key );
		return true;
	}

	public IEnumerable<PopulationWithKey> GetWhere( Func<PhysicalTrait, bool>? physicalTraitPredicate, Func<ushort, bool>? populationCenterPredicate, Func<ushort, bool>? culturePredicate, Func<ushort, bool>? birthYearPredicate ) {
		var physicalTraitKvps = physicalTraitPredicate is not null ? _popsByPhysicalTraits.Where( p => physicalTraitPredicate( p.Key ) ) : _popsByPhysicalTraits;

		foreach (var physicalTraitKvp in physicalTraitKvps) {
			var populationCenterKvps = populationCenterPredicate is not null ? physicalTraitKvp.Value.Where( p => populationCenterPredicate( p.Key ) ) : physicalTraitKvp.Value;

			foreach (var populationCenterKvp in populationCenterKvps) {
				var cultureKvps = culturePredicate is not null ? populationCenterKvp.Value.Where( p => culturePredicate( p.Key ) ) : populationCenterKvp.Value;

				foreach (var cultureKvp in cultureKvps) {
					var birthYearKvps = birthYearPredicate is not null ? cultureKvp.Value.Where( p => birthYearPredicate( p.Key ) ) : cultureKvp.Value;

					foreach (var birthYearKvp in birthYearKvps) {
						yield return new PopulationWithKey( birthYearKvp.Value, new PopulationKey( physicalTraitKvp.Key, populationCenterKvp.Key, cultureKvp.Key, birthYearKvp.Key ) );
					}
				}
			}
		}
	}

	public IEnumerable<PopulationWithKey> GetAll() {
		foreach (var physicalTraitKvp in _popsByPhysicalTraits) {
			foreach (var populationCenterKvp in physicalTraitKvp.Value) {
				foreach (var cultureKvp in populationCenterKvp.Value) {
					foreach (var birthYearKvp in cultureKvp.Value) {
						yield return new PopulationWithKey( birthYearKvp.Value, new PopulationKey( physicalTraitKvp.Key, populationCenterKvp.Key, cultureKvp.Key, birthYearKvp.Key ) );
					}
				}
			}
		}
	}
}


//public sealed class NestedDynamicDictionary<TKey, TValue> where TKey : struct {
//	private readonly Dictionary<object, NestedDynamicDictionary<TKey, TValue>>? _nestingDictionary;
//	private readonly Dictionary<object, TValue>? _valueDictionary;

//	private readonly Func<NestedDynamicDictionary<TKey, TValue>>

//	public NestedDynamicDictionary( uint level = 0 ) {
//		_dictionary = [];
//	}

//	public TValue this[ TKey key ] {
//		get {
//			if (!_dictionary.TryGetValue( key, out var nestedDict )) {
//				nestedDict = new NestedDynamicDictionary<TKey, TValue>( _valueFactory );
//				_dictionary[ key ] = nestedDict;
//			}
//			return nestedDict._valueFactory( key );
//		}
//	}
//}