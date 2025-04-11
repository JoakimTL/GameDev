namespace Sandbox.Logic.Nations.People;

public readonly struct PhysicalTrait {
	private readonly byte _traits;

	public PhysicalTrait( Sex sex, PhenotypicCategory phenotypicCategory ) {
		_traits = (byte) ((byte) sex << 7 | (byte) phenotypicCategory & 0b0111_1111);
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