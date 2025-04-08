using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Logic.Nations.People;
public sealed class Census {
}

public sealed class CensusDictionary {

	private Dictionary<ushort, Dictionary<ushort, Dictionary<ushort, >>>


}

public readonly struct PopulationKey {
	// - Year of birth
	// - Sex
	// - Culture
	// - Phenotype
	// - Birthcountry
	public readonly ushort BirthYear;
	public readonly Sex Sex;
	public readonly PhenotypicCategory Phenotype;
	public readonly ushort CultureId;
	public readonly ushort BirthPopulationCenterId;
}

public enum PhenotypicCategory : byte {
	SubSaharanAfrican,		// Very dark skin, broad noses, tightly curled hair
	NorthAfrican,			// Medium-dark skin, narrow features, overlap with Middle East
	MiddleEastern,			// Olive to light brown skin, prominent noses, dark hair
	Mediterranean,			// Light to olive skin, straight hair, common in Southern Europe
	NorthEuropean,			// Pale skin, light eyes/hair, Northern Europe
	EastEuropean,			// Pale skin, heavier bone structure, slightly different profile
	SouthAsian,				// Medium to dark skin, finer features, Indian subcontinent
	CentralAsian,			// Mixed East/West features, strong cheekbones, Turkic peoples
	EastAsian,				// Lighter yellow/olive tone, epicanthic fold, straight black hair
	SoutheastAsian,			// Darker skin than East Asians, rounder faces, shorter stature
	ArcticIndigenous,		// Inuit/Sami-type groups, robust facial structure
	Polynesian,				// Brown skin, large builds, straight hair
	NativeNorthAmerican,	// Broad cheekbones, straight black hair, tan to reddish skin
	MesoAmerican,           // Medium brown skin, straight black hair, prominent cheekbones, shorter stature; common in regions like ancient Maya and Aztec civilizations
	NativeSouthAmerican,    // Light to medium brown skin, flatter noses, wide faces, and smaller builds; common in Andean and Amazonian populations like Quechua, Aymara, and Yanomami
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