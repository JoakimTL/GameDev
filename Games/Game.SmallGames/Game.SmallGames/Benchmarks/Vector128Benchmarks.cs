using BenchmarkDotNet.Attributes;
using Engine.Data;
using System.Numerics;
using System.Runtime.Intrinsics;

namespace Game.SmallGames.Benchmarks;
public class Vector128Benchmarks {

	private Vector128<int>[] _vectors;
	private Vector3i[] _engineVector3s;
	private Vector4i[] _engineVector4s;

	[GlobalSetup]
	public void Setup() {
		_vectors = new Vector128<int>[ 1000 ];
		_engineVector3s = new Vector3i[ 1000 ];
		_engineVector4s = new Vector4i[ 1000 ];
		var random = new Random();
		for (int i = 0; i < _vectors.Length; i++) {
			_vectors[ i ] = Vector128.Create( random.Next(), random.Next(), random.Next(), random.Next() );
			_engineVector3s[ i ] = new Vector3i( random.Next(), random.Next(), random.Next() );
			_engineVector4s[ i ] = new Vector4i( random.Next(), random.Next(), random.Next(), random.Next() );
		}
	}

	[Benchmark]
	public void V128HashCode() {
		long l = 0;
		for (int i = 0; i < _vectors.Length; i++) {
			l += _vectors[ i ].GetHashCode();
		}
	}

	[Benchmark]
	public void EngineV3HashCode() {
		long l = 0;
		for (int i = 0; i < _engineVector3s.Length; i++) {
			l += _engineVector3s[ i ].GetHashCode();
		}
	}

	[Benchmark]
	public void EngineV4HashCode() {
		long l = 0;
		for (int i = 0; i < _engineVector4s.Length; i++) {
			l += _engineVector4s[ i ].GetHashCode();
		}
	}

}

public class VectorKeyDictionaryBenchmarks {

	public Dictionary<Vector128<int>, object> _v128Dict;
	public Dictionary<Vector3i, object> _engineV3iDict;
	public Dictionary<Vector3, object> _engineV3Dict;
	public Dictionary<Vector3, object> _engineV3bcDict;
	public Dictionary<Int128, object> _i128Dict;

	public Vector128<int>[] _lookupV128s;
	public Vector3i[] _lookupV3is;

	public Vector3 V3iToV3( Vector3i v3 ) {
		int x = v3.X;
		int y = v3.Y;
		int z = v3.Z;
		unsafe {
			float xb = *(float*) &x;
			float yb = *(float*) &y;
			float zb = *(float*) &z;
			return new Vector3( xb, yb, zb );
		}
	}
	public Vector3 V3iBitConvert( Vector3i v3 ) {
		unsafe {
			return *(Vector3*) &v3;
		}
	}
	public Int128 V3iBitConvertI128( Vector3i v3 ) {
		unsafe {
			return *(Int128*) &v3;
		}
	}

	[GlobalSetup]
	public void Setup() {
		_v128Dict = new Dictionary<Vector128<int>, object>();
		_i128Dict = new Dictionary<Int128, object>();
		_engineV3iDict = new Dictionary<Vector3i, object>();
		_engineV3Dict = new Dictionary<Vector3, object>();
		_engineV3bcDict = new Dictionary<Vector3, object>();
		_lookupV128s = new Vector128<int>[ 1000 ];
		_lookupV3is = new Vector3i[ 1000 ];
		var random = new Random();

		//Fill the lookup arrays, then the dictionaries. The amount of data in the dictionaries should be independent from the lookup arrays.

		for (int i = 0; i < _lookupV128s.Length; i++) {
			Vector4i v = new( random.Next( 0, 100 ), random.Next( 0, 100 ), random.Next( 0, 100 ), random.Next( 0, 100 ) );
			_lookupV128s[ i ] = Vector128.Create( v.X, v.Y, v.Z, v.W );
			_lookupV3is[ i ] = new Vector3i( v.X, v.Y, v.Z );
		}

		int dictionarySize = 50000;
		for (int i = 0; i < dictionarySize; i++) {
			Vector4i v = new( random.Next( 0, 100 ), random.Next( 0, 100 ), random.Next( 0, 100 ), random.Next( 0, 100 ) );
			_v128Dict[ Vector128.Create( v.X, v.Y, v.Z, v.W ) ] = $"{v.X}, {v.Y}, {v.Z}, {v.W}";
			_engineV3iDict[ new Vector3i( v.X, v.Y, v.Z ) ] = $"{v.X}, {v.Y}, {v.Z}, {v.W}";
			_engineV3Dict[ V3iToV3( new Vector3i( v.X, v.Y, v.Z ) ) ] = $"{v.X}, {v.Y}, {v.Z}, {v.W}";
			_engineV3bcDict[ V3iBitConvert( new Vector3i( v.X, v.Y, v.Z ) ) ] = $"{v.X}, {v.Y}, {v.Z}, {v.W}";
			_i128Dict[ V3iBitConvertI128( new Vector3i( v.X, v.Y, v.Z ) ) ] = $"{v.X}, {v.Y}, {v.Z}, {v.W}";

		}
    }

	[Benchmark]
	public void V128DictLookup() {
		bool l = false;
		for (int i = 0; i < _lookupV128s.Length; i++) {
			l = _v128Dict.TryGetValue( _lookupV128s[ i ], out object o );
		}
	}

	[Benchmark]
	public void EngineV3iDictLookup() {
		bool l = false;
		for (int i = 0; i < _lookupV3is.Length; i++) {
			l = _engineV3iDict.TryGetValue( _lookupV3is[ i ], out object o );
		}
	}

	[Benchmark]
	public void EngineV3DictLookup() {
		bool l = false;
		for (int i = 0; i < _lookupV3is.Length; i++) {
			l = _engineV3Dict.TryGetValue( V3iToV3( _lookupV3is[ i ] ), out object o );
		}
	}
	[Benchmark]
	public void EngineV3DictBitConvertLookup() {
		bool l = false;
		for (int i = 0; i < _lookupV3is.Length; i++) {
			l = _engineV3bcDict.TryGetValue( V3iBitConvert( _lookupV3is[ i ] ), out object o );
		}
	}

	[Benchmark]
	public void I128DictBitConvertLookup() {
		bool l = false;
		for (int i = 0; i < _lookupV3is.Length; i++) {
			l = _i128Dict.TryGetValue( V3iBitConvertI128( _lookupV3is[ i ] ), out object o );
		}
	}



}
