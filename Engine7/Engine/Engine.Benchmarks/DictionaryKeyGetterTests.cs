using BenchmarkDotNet.Attributes;

namespace Engine.Benchmarks;

[MemoryDiagnoser]
public class DictionaryKeyGetterTests {

	private string _lastValue;
	private Dictionary<int, string> _intKey;
	private Dictionary<string, string> _stringKey;
	private Dictionary<Type, string> _typeKey;
	private Dictionary<Guid, string> _guidKey;
	private int[] _intsToAdd;
	private string[] _stringsToAdd;
	private Type[] _typesToAdd;
	private Guid[] _guidsToAdd;

	[GlobalSetup]
	public void Setup() {
		this._intKey = [];
		this._stringKey = [];
		this._typeKey = [];
		this._guidKey = [];
		this._intsToAdd = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
		this._stringsToAdd = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
		this._typesToAdd = new Type[] { typeof( int ), typeof( string ), typeof( float ), typeof( double ), typeof( long ), typeof( short ), typeof( byte ), typeof( char ), typeof( bool ), typeof( sbyte ) };
		this._guidsToAdd = new Guid[] {
			new( "d401a272-6c30-4796-b898-ffa824bd3caf" ),
			new( "7f792831-24f1-448c-8ba7-49fbc81ddb75" ),
			new( "03e4a761-5a23-4e0e-8cd6-f7feb0910ea2" ),
			new( "8df79a17-dc67-4c77-9d26-b707bd5827a3" ),
			new( "e897b1f9-bf22-4287-9711-3873cf11e56d" ),
			new( "050d6f4e-cb33-46e7-922c-a9ef720ce786" ),
			new( "4ea6bde1-51ee-4b44-9962-f7b26293b45d" ),
			new( "d3a33dd4-0912-47f0-acf9-ba21ebc56614" ),
			new( "c66c0e91-540b-49b0-a104-0b2e63b98e2a" ),
			new( "2615bd5b-2ce0-4689-b904-10709ff4ff80" )
		};

		for (int i = 0; i < 10; i++) {
			this._intKey.Add( _intsToAdd[ i ], i.ToString() );
			this._stringKey.Add( _stringsToAdd[ i ], i.ToString() );
			this._typeKey.Add( _typesToAdd[ i ], i.ToString() );
			this._guidKey.Add( _guidsToAdd[ i ], i.ToString() );
		}
	}

	[Benchmark]
	public void GetTestInt() {
		for (int i = 0; i < 10; i++) {
			this._intKey.TryGetValue( _intsToAdd[ i ], out this._lastValue );
		}
	}

	[Benchmark]
	public void GetTestString() {
		for (int i = 0; i < 10; i++) {
			this._stringKey.TryGetValue( _stringsToAdd[ i ], out this._lastValue );
		}
	}

	[Benchmark]
	public void GetTestType() {
		for (int i = 0; i < 10; i++) {
			this._typeKey.TryGetValue( _typesToAdd[ i ], out this._lastValue );
		}
	}

	[Benchmark]
	public void GetTestGuid() {
		for (int i = 0; i < 10; i++) {
			this._guidKey.TryGetValue( _guidsToAdd[ i ], out this._lastValue );
		}
	}
}
