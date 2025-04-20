namespace Sandbox.Logic.Nations.People;

public sealed class Census {

	private readonly Dictionary<PhysicalTrait, Dictionary<ushort, Dictionary<ushort, Dictionary<ushort, Population>>>> _popsByPhysicalTraits = [];

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
		IEnumerable<KeyValuePair<PhysicalTrait, Dictionary<ushort, Dictionary<ushort, Dictionary<ushort, Population>>>>> physicalTraitKvps = physicalTraitPredicate is not null ? _popsByPhysicalTraits.Where( p => physicalTraitPredicate( p.Key ) ) : _popsByPhysicalTraits;

		foreach (KeyValuePair<PhysicalTrait, Dictionary<ushort, Dictionary<ushort, Dictionary<ushort, Population>>>> physicalTraitKvp in physicalTraitKvps) {
			IEnumerable<KeyValuePair<ushort, Dictionary<ushort, Dictionary<ushort, Population>>>> populationCenterKvps = populationCenterPredicate is not null ? physicalTraitKvp.Value.Where( p => populationCenterPredicate( p.Key ) ) : physicalTraitKvp.Value;

			foreach (KeyValuePair<ushort, Dictionary<ushort, Dictionary<ushort, Population>>> populationCenterKvp in populationCenterKvps) {
				IEnumerable<KeyValuePair<ushort, Dictionary<ushort, Population>>> cultureKvps = culturePredicate is not null ? populationCenterKvp.Value.Where( p => culturePredicate( p.Key ) ) : populationCenterKvp.Value;

				foreach (KeyValuePair<ushort, Dictionary<ushort, Population>> cultureKvp in cultureKvps) {
					IEnumerable<KeyValuePair<ushort, Population>> birthYearKvps = birthYearPredicate is not null ? cultureKvp.Value.Where( p => birthYearPredicate( p.Key ) ) : cultureKvp.Value;

					foreach (KeyValuePair<ushort, Population> birthYearKvp in birthYearKvps) {
						yield return new PopulationWithKey( birthYearKvp.Value, new PopulationKey( physicalTraitKvp.Key, populationCenterKvp.Key, cultureKvp.Key, birthYearKvp.Key ) );
					}
				}
			}
		}
	}

	public IEnumerable<PopulationWithKey> GetAll() {
		foreach (KeyValuePair<PhysicalTrait, Dictionary<ushort, Dictionary<ushort, Dictionary<ushort, Population>>>> physicalTraitKvp in _popsByPhysicalTraits) {
			foreach (KeyValuePair<ushort, Dictionary<ushort, Dictionary<ushort, Population>>> populationCenterKvp in physicalTraitKvp.Value) {
				foreach (KeyValuePair<ushort, Dictionary<ushort, Population>> cultureKvp in populationCenterKvp.Value) {
					foreach (KeyValuePair<ushort, Population> birthYearKvp in cultureKvp.Value) {
						yield return new PopulationWithKey( birthYearKvp.Value, new PopulationKey( physicalTraitKvp.Key, populationCenterKvp.Key, cultureKvp.Key, birthYearKvp.Key ) );
					}
				}
			}
		}
	}
}