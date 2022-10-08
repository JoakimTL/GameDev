using System.Numerics;
using System.Runtime.InteropServices;

namespace PhysicsTesting;
public class InertiaPhysicsObject {

	public Vector3 Translation { get; set; }
	public Vector3 Scale { get; set; }

	public Vector4[] ShapePoints { get; private set; }
	public Vector3 CenterOfMass { get; private set; }
	public float TotalMass { get; private set; }

	public InertiaPhysicsObject( params Vector4[] shapePoints ) {
		this.ShapePoints = shapePoints;
		this.CenterOfMass = new Vector3();
		this.TotalMass = 0;
		foreach ( var item in shapePoints ) {
			this.CenterOfMass += new Vector3( item.X, item.Y, item.Z ) * item.W;
			this.TotalMass += item.W;
		}
		this.CenterOfMass /= this.TotalMass;
	}

	public float GetInertia( Vector3 direction ) {
		//length of cross with direction, then multiply with dot with self, dot between self and direction, dot with direction added together
		float inertia = 0;
		/*for ( int i = 0; i < this.ShapePoints.Length; i++ ) {
			Vector4 i1 = this.ShapePoints[ i ];
			Vector4 i2 = this.ShapePoints[ ( i + 1 ) % this.ShapePoints.Length ];
			Vector4 i3 = this.ShapePoints[ ( i + 2 ) % this.ShapePoints.Length ];
			Vector4 i4 = this.ShapePoints[ ( i + 3 ) % this.ShapePoints.Length ];
			Vector3 v1 = new( i1.X, i1.Y, i1.Z );
			Vector3 v2 = new( i2.X, i2.Y, i2.Z );
			Vector3 v3 = new( i3.X, i3.Y, i3.Z );
			Vector3 v4 = new( i4.X, i4.Y, i4.Z );
			Console.WriteLine($"#{i}");
			float volume = MathF.Abs( Vector3.Dot( v1 - v4, Vector3.Cross( v2 - v4, v3 - v4 ) ) ) / 6;
			Console.WriteLine(volume);
			Vector3 com = ( v1 * i1.W + v2 * i2.W + v3 * i3.W + v4 * i4.W ) / ( i1.W + i2.W + i3.W + i4.W );
			Console.WriteLine( com );
			Vector3 c1 = Vector3.Cross( com, direction );
			Console.WriteLine(c1);
			inertia += c1.Length() * volume * 5;
		}
		inertia /= 20;*/
		for ( int i = 0; i < this.ShapePoints.Length; i++ ) {
			Vector4 i1 = this.ShapePoints[ i ];
			Vector3 v1 = new( i1.X, i1.Y, i1.Z );
			Vector3 c1 = Vector3.Cross( v1 - this.CenterOfMass, direction );
			inertia += c1.LengthSquared() * i1.W;
		}

		return inertia;
	}

}

public unsafe class InertiaCalculator : IDisposable {

	private readonly Vector3* _indexedTranslations;
	private readonly float* _densities;
	private readonly int _length;
	private readonly int _steps;
	private readonly Vector3 _stepSize;

	public InertiaCalculator( int steps ) {
		this._steps = steps;
		this._indexedTranslations = (Vector3*) NativeMemory.AllocZeroed( (nuint) ( this._steps * this._steps * this._steps * sizeof( Vector3 ) ) );
		this._densities = (float*) NativeMemory.AllocZeroed( (nuint) ( this._steps * this._steps * this._steps * sizeof( float ) ) );
		this._length = this._steps * this._steps * this._steps;
		this._stepSize = new( 2f / steps );
		Vector3 start = new( -1, -1, -1 );
		int index = 0;
		for ( int x = 0; x < this._steps; x++ )
			for ( int y = 0; y < this._steps; y++ )
				for ( int z = 0; z < this._steps; z++ ) {
					this._indexedTranslations[ index++ ] = ( this._stepSize * new Vector3( x, y, z ) ) + start;
				}

	}

	public float GetInertia( Vector3 direction, Vector3 centerOfMass, Vector3 scale, float massPerMeterCubed, Func<Vector3, float> densityFunction, out float totalMass ) {
		float inertia = 0;
		float massPerDensity = massPerMeterCubed * this._stepSize.X * this._stepSize.Y * this._stepSize.Z * scale.X * scale.Y * scale.Z;
		totalMass = 0;
		for ( int i = 0; i < this._length; i++ ) {
			this._densities[ i ] = densityFunction( this._indexedTranslations[ i ] * scale );
		}
		for ( int i = 0; i < this._length; i++ ) {
			Vector3 c = Vector3.Cross( this._indexedTranslations[ i ] * scale - centerOfMass, direction );
			float mass = this._densities[ i ] * massPerDensity;
			inertia += c.LengthSquared() * mass;
			totalMass += mass;
		}
		return inertia;
	}

	public float GetInertiaExp( Vector3 direction, Vector3 centerOfMass, Vector3 scale, float massPerMeterCubed, Func<Vector3, float> densityFunction, out float totalMass ) {
		float inertia = 0;
		float massPerDensity = massPerMeterCubed * this._stepSize.X * this._stepSize.Y * this._stepSize.Z * scale.X * scale.Y * scale.Z;
		totalMass = 0;
		Vector3 ndir = Vector3.Normalize( direction );
		float pitch = MathF.Asin( ndir.Y );
		float yaw = MathF.Atan2( ndir.X, ndir.Z );
		Quaternion q = Quaternion.CreateFromYawPitchRoll( yaw, pitch, 0 );
		for ( int x = 0; x < this._steps; x++ ) {
			for ( int y = 0; y < this._steps; y++ ) {
				Vector3 p = Vector3.Transform( this._stepSize * new Vector3( x, y, 0 ) + new Vector3( -1, -1, 0 ), q ) * scale;
				float density = densityFunction( p );
				Vector3 c = Vector3.Cross( p - centerOfMass, direction );
				float mass = density * massPerDensity;
				inertia += c.LengthSquared() * mass;
				totalMass += mass;
			}
		}
		return inertia;
	}

	public void Dispose() {
		NativeMemory.Free( this._indexedTranslations );
		NativeMemory.Free( this._densities );
	}
}

public readonly struct Triangle<V> where V : unmanaged {
	public readonly V A;
	public readonly V B;
	public readonly V C;

	public Triangle( V a, V b, V c ) {
		this.A = a;
		this.B = b;
		this.C = c;
	}
}
