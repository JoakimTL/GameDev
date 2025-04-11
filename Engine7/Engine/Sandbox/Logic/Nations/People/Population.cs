namespace Sandbox.Logic.Nations.People;

public sealed class Population {

	public uint Births { get; private set; }
	public uint Deaths { get; private set; }
	public uint Emigrations { get; private set; }
	public uint Immigrations { get; private set; }

	public uint PopulationCount => Births + Immigrations - Deaths - Emigrations;

	public Population() {
		Births = 0;
		Deaths = 0;
	}

	public void AddBirths( uint count ) {
		Births += count;
	}

	public void AddDeaths( uint count ) {
		if (PopulationCount < count)
			throw new ArgumentOutOfRangeException( nameof( count ), "Deaths cannot exceed current population." );
		Deaths += count;
	}

	public void AddImmigrants( uint count ) {
		Immigrations += count;
	}

	public void AddEmigrants( uint count ) {
		if (PopulationCount < count)
			throw new ArgumentOutOfRangeException( nameof( count ), "Emigrations cannot exceed current population." );
		Emigrations += count;
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