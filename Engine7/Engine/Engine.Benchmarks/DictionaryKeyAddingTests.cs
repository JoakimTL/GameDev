using BenchmarkDotNet.Attributes;

namespace Engine.Benchmarks;

[MemoryDiagnoser]
public class DictionaryKeyAddingTests {

	private Dictionary<int, string> _intKey;
	private Dictionary<string, string> _stringKey;
	private Dictionary<Type, string> _typeKey;
	private int[] _intsToAdd;
	private string[] _stringsToAdd;
	private Type[] _typesToAdd;

	[GlobalSetup]
	public void Setup() {
		this._intKey = new();
		this._stringKey = new();
		this._typeKey = new();
		this._intsToAdd = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
		this._stringsToAdd = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
		this._typesToAdd = new Type[] { typeof( int ), typeof( string ), typeof( float ), typeof( double ), typeof( long ), typeof( short ), typeof( byte ), typeof( char ), typeof( bool ), typeof( sbyte ) };
	}

	[Benchmark]
	public void AddRemoveTestInt() {
		for (int i = 0; i < 10; i++) {
			this._intKey.Add( _intsToAdd[ i ], i.ToString() );
		}
		for (int i = 0; i < 10; i++) {
			this._intKey.Remove( _intsToAdd[ i ] );
		}
	}

	[Benchmark]
	public void AddRemoveTestString() {
		for (int i = 0; i < 10; i++) {
			this._stringKey.Add( _stringsToAdd[ i ], i.ToString() );
		}
		for (int i = 0; i < 10; i++) {
			this._stringKey.Remove( _stringsToAdd[ i ] );
		}
	}

	[Benchmark]
	public void AddRemoveTestType() {
		for (int i = 0; i < 10; i++) {
			this._typeKey.Add( this._typesToAdd[ i ], i.ToString() );
		}
		for (int i = 0; i < 10; i++) {
			this._typeKey.Remove( this._typesToAdd[ i ] );
		}
	}
}
