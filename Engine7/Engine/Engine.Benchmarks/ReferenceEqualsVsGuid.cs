using BenchmarkDotNet.Attributes;

namespace Engine.Benchmarks;

[MemoryDiagnoser]
public class ReferenceEqualsVsGuid {

	private bool _lastBool;

	private ReferenceComparisonClass[] _refs = [ new( new( "81BEAAB3-59E9-49CD-89E2-BC471117B277" ) ), new( new( "663488A3-07FF-43F1-B401-AAB0690EEC7B" ) ), new( new( "210C1335-F165-4A07-B6F0-939A1FE1C318" ) ), new( new( "1107CC85-B18E-44F5-B5BA-4144E1B93CA3" ) ) ];
	private GuidComparisonClass[] _guids = [ new( new( "81BEAAB3-59E9-49CD-89E2-BC471117B277" ) ), new( new( "663488A3-07FF-43F1-B401-AAB0690EEC7B" ) ), new( new( "210C1335-F165-4A07-B6F0-939A1FE1C318" ) ), new( new( "1107CC85-B18E-44F5-B5BA-4144E1B93CA3" ) ) ];


	[Benchmark]
	public void Bench_Reference_ReferenceEquals() {
		for (int i = 0; i < _refs.Length; i++) {
			for (int j = 0; j < _refs.Length; j++) {
				_lastBool = ReferenceEquals( _refs[ i ], _refs[ j ] );
			}
		}
	}

	[Benchmark]
	public void Bench_Reference_ReferenceEqualityOperator() {
		for (int i = 0; i < _refs.Length; i++) {
			for (int j = 0; j < _refs.Length; j++) {
				_lastBool = _refs[ i ] == _refs[ j ];
			}
		}
	}

	[Benchmark]
	public void Bench_Guid_ReferenceEquals() {
		for (int i = 0; i < _guids.Length; i++) {
			for (int j = 0; j < _guids.Length; j++) {
				_lastBool = ReferenceEquals( _guids[ i ], _guids[ j ] );
			}
		}
	}

	[Benchmark]
	public void Bench_Guid_ReferenceEqualityOperator() {
		for (int i = 0; i < _guids.Length; i++) {
			for (int j = 0; j < _guids.Length; j++) {
				_lastBool = _guids[ i ] == _guids[ j ];
			}
		}
	}

	private class ReferenceComparisonClass( Guid id ) {
		public Guid Id { get; } = id;
	}

	private class GuidComparisonClass( Guid id ) {
		public Guid Id { get; } = id;

		public override bool Equals( object? obj ) {
			if (obj is GuidComparisonClass other)
				return this.Id == other.Id;
			return false;
		}

		public override int GetHashCode() {
			return this.Id.GetHashCode();
		}

		public static bool operator ==( GuidComparisonClass left, GuidComparisonClass right ) => left.Id == right.Id;

		public static bool operator !=( GuidComparisonClass left, GuidComparisonClass right ) => left.Id != right.Id;
	}
}