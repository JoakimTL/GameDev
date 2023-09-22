using BenchmarkDotNet.Attributes;
using System.Numerics;

namespace TestPlatformClient;

public struct Vector4d {
	public double X, Y, Z, W;

	public Vector4d( double x, double y, double z, double w ) {
		this.X = x;
		this.Y = y;
		this.Z = z;
		this.W = w;
	}

	public Vector4d Add( Vector4d other )
		=> new( this.X + other.X, this.Y + other.Y, this.Z + other.Z, this.W + other.W );
}

public struct Vector4dSIMD {
	private readonly Vector<double> _underlying;

	private Vector4dSIMD( Vector<double> underlying ) {
		this._underlying = underlying;
	}
	public Vector4dSIMD( double x, double y, double z, double w ) {
		this._underlying = new Vector<double>( stackalloc double[] { x, y, z, w } );
	}

	public Vector4dSIMD Add( Vector4dSIMD other )
		=> new( Vector.Add( this._underlying, other._underlying ) );
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
		this._naive1 = new( 4.3, 12.1, 7.4, 52.5 );
		this._naive2 = new( 17.043, -904.16, 2.1, -6.852 );
		this._simd1 = new( 4.3, 12.1, 7.4, 52.5 );
		this._simd2 = new( 17.043, -904.16, 2.1, -6.852 );

		this._naives = new Vector4d[ 1024 ];
		this._simds = new Vector4dSIMD[ 1024 ];
		Random random = new();
		for ( int i = 0; i < 1024; i++ ) {
			this._naives[ i ] = new( random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextDouble() );
			this._simds[ i ] = new( random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextDouble() );
		}
	}

	[Benchmark]
	public void NaiveSimple() => this._naive3 = this._naive1.Add( this._naive2 );

	[Benchmark]
	public void NaiveArray() {
		this._naive3 = new( 0, 0, 0, 0 );
		for ( int i = 0; i < this._naives.Length; i++ )
			this._naive3 = this._naive3.Add( this._naives[ i ] );
	}

	[Benchmark]
	public void SIMDSimple() => this._simd3 = this._simd1.Add( this._simd2 );

	[Benchmark]
	public void SIMDArray() {
		this._simd3 = new( 0, 0, 0, 0 );
		for ( int i = 0; i < this._simds.Length; i++ )
			this._simd3 = this._simd3.Add( this._simds[ i ] );
	}
}