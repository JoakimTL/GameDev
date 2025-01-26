using BenchmarkDotNet.Attributes;

namespace Engine.Benchmarks;

[MemoryDiagnoser]
public class CodeMark {

	private int _lastInt;
	private string[] _codesToIntify;

	[GlobalSetup]
	public void Setup() {
		this._codesToIntify = new string[] {
			"OPEN",
			"FORM",
			"SAVE",
			"LOAD",
			"EXIT",
			"YUCK",
			"0000",
			"TACK",
			"JUMP",
			"KICK"
		};
	}


	[Benchmark]
	public void EncodingTest() {
		unsafe {
			byte* codeBytes = stackalloc byte[ 4 ];
			for (int i = 0; i < 10; i++) {
				string code = this._codesToIntify[ i ];
				System.Text.Encoding.UTF8.GetBytes( code, new Span<byte>( codeBytes, 4 ) );
				_lastInt = *(int*) codeBytes;
			}
		}
	}

	[Benchmark]
	public void ManualTest() {
		for (int i = 0; i < 10; i++) {
			string code = this._codesToIntify[ i ];
			_lastInt = (byte) (code[ 0 ]) << 24 | (byte) (code[ 1 ]) << 16 | (byte) (code[ 2 ]) << 8 | (byte) code[ 3 ];
		}
	}

	[Benchmark]
	public void ManualTest2() {
		unsafe {
			byte* codeBytes = stackalloc byte[ 4 ];
			for (int i = 0; i < 10; i++) {
				string code = this._codesToIntify[ i ];
				codeBytes[ 0 ] = (byte) code[ 0 ];
				codeBytes[ 1 ] = (byte) code[ 1 ];
				codeBytes[ 2 ] = (byte) code[ 2 ];
				codeBytes[ 3 ] = (byte) code[ 3 ];
				_lastInt = *(int*) codeBytes;
			}
		}
	}

	[Benchmark]
	public void ManualTest3() {
		for (int i = 0; i < 10; i++) {
			_lastInt = this._codesToIntify[ i ].ToIntCode();
		}
	}

}
