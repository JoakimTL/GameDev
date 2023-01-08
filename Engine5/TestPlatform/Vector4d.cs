using BenchmarkDotNet.Attributes;
using System.Numerics;

namespace TestPlatform;

public struct Vector4d {
	public double X, Y, Z, W;

	public Vector4d( double x, double y, double z, double w ) {
		this.X = x;
		this.Y = y;
		this.Z = z;
		this.W = w;
	}

	public Vector4d Add( Vector4d other )
		=> new( X + other.X, Y + other.Y, Z + other.Z, W + other.W );
}

public struct Vector4dSIMD {
	private readonly Vector<double> _underlying;

	private Vector4dSIMD( Vector<double> underlying ) {
		_underlying = underlying;
	}
	public Vector4dSIMD( double x, double y, double z, double w ) {
		_underlying = new Vector<double>( stackalloc double[] { x, y, z, w } );
	}

	public Vector4dSIMD Add( Vector4dSIMD other )
		=> new( System.Numerics.Vector.Add( _underlying, other._underlying ) );
}

public class Benchmark2 {

	private Vector4d _naive1;
	private Vector4d _naive2;
	private Vector4d _naive3;
	private Vector4d[] _naives;
	private Vector4dSIMD _simd1;
	private Vector4dSIMD _simd2;
	private Vector4dSIMD _simd3;
	private Vector4dSIMD[] _simds;

	[GlobalSetup]
	public void Setup() {
		_naive1 = new( 4.3, 12.1, 7.4, 52.5 );
		_naive2 = new( 17.043, -904.16, 2.1, -6.852 );
		_simd1 = new( 4.3, 12.1, 7.4, 52.5 );
		_simd2 = new( 17.043, -904.16, 2.1, -6.852 );

		_naives = new Vector4d[ 1024 ];
		_simds = new Vector4dSIMD[ 1024 ];
		Random random = new();
		for ( int i = 0; i < 1024; i++ ) {
			_naives[ i ] = new( random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextDouble() );
			_simds[ i ] = new( random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextDouble() );
		}
	}

	[Benchmark]
	public void NaiveSimple() {
		_naive3 = _naive1.Add( _naive2 );
	}

	[Benchmark]
	public void NaiveArray() {
		_naive3 = new( 0, 0, 0, 0 );
		for ( int i = 0; i < _naives.Length; i++ ) {
			_naive3 = _naive3.Add( _naives[ i ] );
		}
	}

	[Benchmark]
	public void SIMDSimple() {
		_simd3 = _simd1.Add( _simd2 );
	}

	[Benchmark]
	public void SIMDArray() {
		_simd3 = new( 0, 0, 0, 0 );
		for ( int i = 0; i < _simds.Length; i++ ) {
			_simd3 = _simd3.Add( _simds[ i ] );
		}
	}
}