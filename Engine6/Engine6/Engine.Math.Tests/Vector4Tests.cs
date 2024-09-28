namespace Engine.Math.Tests;

[TestFixture]
public sealed class Vector4Tests {
	#region Initializations
	[Test]
	public void Constructors() {
		Vector4<int> vectorA = new( 1, 2, 3, 4 );
		Vector4<double> vectorB = new( 1.0, 2.0, 3.0, 4.0 );

		Assert.That( vectorA.X, Is.EqualTo( 1 ) );
		Assert.That( vectorA.Y, Is.EqualTo( 2 ) );
		Assert.That( vectorA.Z, Is.EqualTo( 3 ) );
		Assert.That( vectorA.W, Is.EqualTo( 4 ) );

		Assert.That( vectorB.X, Is.EqualTo( 1.0 ) );
		Assert.That( vectorB.Y, Is.EqualTo( 2.0 ) );
		Assert.That( vectorB.Z, Is.EqualTo( 3.0 ) );
		Assert.That( vectorB.W, Is.EqualTo( 4.0 ) );
	}

	[Test]
	public void Identities() {
		Vector4<int> addtitiveIdentityA = Vector4<int>.AdditiveIdentity;
		Vector4<int> multiplicativeIdentityA = Vector4<int>.MultiplicativeIdentity;
		Vector4<int> zeroA = Vector4<int>.Zero;
		Vector4<int> oneA = Vector4<int>.One;
		Vector4<double> addtitiveIdentityB = Vector4<double>.AdditiveIdentity;
		Vector4<double> multiplicativeIdentityB = Vector4<double>.MultiplicativeIdentity;
		Vector4<double> zeroB = Vector4<double>.Zero;
		Vector4<double> oneB = Vector4<double>.One;

		Assert.That( addtitiveIdentityA, Is.EqualTo( zeroA ) );
		Assert.That( multiplicativeIdentityA, Is.EqualTo( oneA ) );

		Assert.That( zeroA.X, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Y, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Z, Is.EqualTo( 0 ) );
		Assert.That( zeroA.W, Is.EqualTo( 0 ) );

		Assert.That( oneA.X, Is.EqualTo( 1 ) );
		Assert.That( oneA.Y, Is.EqualTo( 1 ) );
		Assert.That( oneA.Z, Is.EqualTo( 1 ) );
		Assert.That( oneA.W, Is.EqualTo( 1 ) );

		Assert.That( addtitiveIdentityB, Is.EqualTo( zeroB ) );
		Assert.That( multiplicativeIdentityB, Is.EqualTo( oneB ) );

		Assert.That( zeroB.X, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Y, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Z, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.W, Is.EqualTo( 0.0 ) );

		Assert.That( oneB.X, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Y, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Z, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.W, Is.EqualTo( 1.0 ) );
	}
	#endregion

	#region Operators
	[Test]
	public void EqualOperator_Int() {
		bool equal = new Vector4<int>( 1, 2, 3, 4 ) == new Vector4<int>( 1, 2, 3, 4 );
		bool equalX_isfalse = new Vector4<int>( 1, 2, 3, 4 ) == new Vector4<int>( 0, 2, 3, 4 );
		bool equalY_isfalse = new Vector4<int>( 1, 2, 3, 4 ) == new Vector4<int>( 1, 0, 3, 4 );
		bool equalZ_isfalse = new Vector4<int>( 1, 2, 3, 4 ) == new Vector4<int>( 1, 2, 0, 4 );
		bool equalW_isfalse = new Vector4<int>( 1, 2, 3, 4 ) == new Vector4<int>( 1, 2, 3, 0 );

		Assert.That( equal, Is.True );
		Assert.That( equalX_isfalse, Is.False );
		Assert.That( equalY_isfalse, Is.False );
		Assert.That( equalZ_isfalse, Is.False );
		Assert.That( equalW_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Int() {
		bool equal_isfalse = new Vector4<int>( 1, 2, 3, 4 ) != new Vector4<int>( 1, 2, 3, 4 );
		bool notequalX = new Vector4<int>( 1, 2, 3, 4 ) != new Vector4<int>( 0, 2, 3, 4 );
		bool notequalY = new Vector4<int>( 1, 2, 3, 4 ) != new Vector4<int>( 1, 0, 3, 4 );
		bool notequalZ = new Vector4<int>( 1, 2, 3, 4 ) != new Vector4<int>( 1, 2, 0, 4 );
		bool notequalW = new Vector4<int>( 1, 2, 3, 4 ) != new Vector4<int>( 1, 2, 3, 0 );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( notequalX, Is.True );
		Assert.That( notequalY, Is.True );
		Assert.That( notequalZ, Is.True );
		Assert.That( notequalW, Is.True );
	}

	[Test]
	public void EqualOperator_Double() {
		bool equal = new Vector4<double>( 1.0, 2.0, 3.0, 4.0 ) == new Vector4<double>( 1.0, 2.0, 3.0, 4.0 );
		bool equalX_isfalse = new Vector4<double>( 1.0, 2.0, 3.0, 4.0 ) == new Vector4<double>( 0.0, 2.0, 3.0, 4.0 );
		bool equalY_isfalse = new Vector4<double>( 1.0, 2.0, 3.0, 4.0 ) == new Vector4<double>( 1.0, 0.0, 3.0, 4.0 );
		bool equalZ_isfalse = new Vector4<double>( 1.0, 2.0, 3.0, 4.0 ) == new Vector4<double>( 1.0, 2.0, 0.0, 4.0 );
		bool equalW_isfalse = new Vector4<double>( 1.0, 2.0, 3.0, 4.0 ) == new Vector4<double>( 1.0, 2.0, 3.0, 0.0 );

		Assert.That( equal, Is.True );
		Assert.That( equalX_isfalse, Is.False );
		Assert.That( equalY_isfalse, Is.False );
		Assert.That( equalZ_isfalse, Is.False );
		Assert.That( equalW_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Double() {
		bool equal_isfalse = new Vector4<double>( 1.0, 2.0, 3.0, 4.0 ) != new Vector4<double>( 1.0, 2.0, 3.0, 4.0 );
		bool notequalX = new Vector4<double>( 1.0, 2.0, 3.0, 4.0 ) != new Vector4<double>( 0.0, 2.0, 3.0, 4.0 );
		bool notequalY = new Vector4<double>( 1.0, 2.0, 3.0, 4.0 ) != new Vector4<double>( 1.0, 0.0, 3.0, 4.0 );
		bool notequalZ = new Vector4<double>( 1.0, 2.0, 3.0, 4.0 ) != new Vector4<double>( 1.0, 2.0, 0.0, 4.0 );
		bool notequalW = new Vector4<double>( 1.0, 2.0, 3.0, 4.0 ) != new Vector4<double>( 1.0, 2.0, 3.0, 0.0 );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( notequalX, Is.True );
		Assert.That( notequalY, Is.True );
		Assert.That( notequalZ, Is.True );
		Assert.That( notequalW, Is.True );
	}

	[Test]
	public void NegateOperator() {
		Vector4<int> vectorA = new( 1, 2, 3, 4 );

		Vector4<int> expected = new( -1, -2, -3, -4 );
		Assert.That( -vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void AddOperator() {
		Vector4<int> vectorA = new( 1, 2, 3, 4 );
		Vector4<int> vectorB = new( 2, 3, 4, 5 );

		Vector4<int> expected = new( 3, 5, 7, 9 );
		Assert.That( vectorA + vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void SubtractOperator() {
		Vector4<int> vectorA = new( 1, 2, 3, 4 );
		Vector4<int> vectorB = new( 2, 3, 4, 5 );

		Vector4<int> expected = new( -1, -1, -1, -1 );
		Assert.That( vectorA - vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarMultiplyOperator() {
		Vector4<int> vectorA = new( 1, 2, 3, 4 );

		Vector4<int> expected = new( 2, 4, 6, 8 );
		Assert.That( vectorA * 2, Is.EqualTo( expected ) );
		Assert.That( 2 * vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarDivideOperator() {
		Vector4<int> vectorA = new( 2, 4, 6, 8 );

		Vector4<int> expected = new( 1, 2, 3, 4 );
		Assert.That( vectorA / 2, Is.EqualTo( expected ) );
	}

	[Test]
	public void DivideScalarOperator() {
		Vector4<int> vectorA = new( 2, 4, 8, 16 );

		Vector4<int> expected = new( 16, 8, 4, 2 );
		Assert.That( 32 / vectorA, Is.EqualTo( expected ) );
	}
	[Test]
	public void MultiplyMatrix4x4Operator() {
		Vector4<int> a = new( 1, 2, 3, 4 );
		Matrix4x4<int> b = new(
			1, 0, 0, 0,
			0, 1, 0, 0,
			0, 0, 1, 0,
			0, 0, 0, 1
		);
		Matrix4x4<int> c = new(
			0, 0, 0, 0,
			1, 1, 0, 0,
			0, 0, 1, 1,
			0, 0, 0, 0
		);
		Matrix4x4<int> d = new(
			0, 0, 1, 1,
			0, 0, 1, 0,
			0, 1, 1, 0,
			1, 0, 1, 0
		);

		Vector4<int> expected_ab = new( 1, 2, 3, 4 );
		Vector4<int> expected_ac = new( 2, 2, 3, 3 );
		Vector4<int> expected_ad = new( 4, 3, 10, 1 );

		Assert.That( a * b, Is.EqualTo( expected_ab ) );
		Assert.That( a * c, Is.EqualTo( expected_ac ) );
		Assert.That( a * d, Is.EqualTo( expected_ad ) );
	}

	[Test]
	public void ComparisonOperators() {
		Vector4<int> a = new( 1, 2, 3, 4 );
		Vector4<int> b = new( 2, 3, 4, 5 );
		Vector4<int> c = new( 2, 2, 3, 4 );
		Vector4<int> d = new( 1, 2, 3, 4 );

		Assert.That( a < b, Is.True );
		Assert.That( a > b, Is.False );
		Assert.That( a <= b, Is.True );
		Assert.That( a >= b, Is.False );
		Assert.That( a < c, Is.False );
		Assert.That( a > c, Is.False );
		Assert.That( a <= c, Is.True );
		Assert.That( a >= c, Is.False );
		Assert.That( a > d, Is.False );
		Assert.That( a < d, Is.False );
		Assert.That( a >= d, Is.True );
		Assert.That( a <= d, Is.True );
		Assert.That( b < c, Is.False );
		Assert.That( b > c, Is.False );
		Assert.That( b <= c, Is.False );
		Assert.That( b >= c, Is.True );
	}
	#endregion

	#region Methods
	[Test]
	public void GetMultivector() {
		Vector4<int> a = new( 1, 2, 3, 4 );

		Multivector4<int> multivector = a.GetMultivector();

		Assert.That( multivector.Scalar, Is.EqualTo( 0 ) );
		Assert.That( multivector.Vector.X, Is.EqualTo( 1 ) );
		Assert.That( multivector.Vector.Y, Is.EqualTo( 2 ) );
		Assert.That( multivector.Vector.Z, Is.EqualTo( 3 ) );
		Assert.That( multivector.Vector.W, Is.EqualTo( 4 ) );
		Assert.That( multivector.Bivector.XY, Is.EqualTo( 0 ) );
		Assert.That( multivector.Bivector.ZX, Is.EqualTo( 0 ) );
		Assert.That( multivector.Bivector.XW, Is.EqualTo( 0 ) );
		Assert.That( multivector.Bivector.YZ, Is.EqualTo( 0 ) );
		Assert.That( multivector.Bivector.YW, Is.EqualTo( 0 ) );
		Assert.That( multivector.Bivector.ZW, Is.EqualTo( 0 ) );
		Assert.That( multivector.Trivector.XYZ, Is.EqualTo( 0 ) );
		Assert.That( multivector.Trivector.XYW, Is.EqualTo( 0 ) );
		Assert.That( multivector.Trivector.XZW, Is.EqualTo( 0 ) );
		Assert.That( multivector.Trivector.YZW, Is.EqualTo( 0 ) );
		Assert.That( multivector.Quadvector.XYZW, Is.EqualTo( 0 ) );
	}

	[Test]
	public void MultiplyEntrywise() {
		Vector4<int> a = new( 1, 2, 3, 4 );
		Vector4<int> b = new( 2, 3, 4, 5 );

		Vector4<int> expected = new( 2, 6, 12, 20 );

		Assert.That( a.MultiplyEntrywise( b ), Is.EqualTo( expected ) );
	}

	[Test]
	public void DivideEntrywise() {
		Vector4<int> a = new( 4, 6, 8, 10 );
		Vector4<int> b = new( 2, 3, 4, 5 );

		Vector4<int> expected = new( 2, 2, 2, 2 );

		Assert.That( a.DivideEntrywise( b ), Is.EqualTo( expected ) );
	}

	[Test]
	public void EntrywiseOperation() {
		Vector4<int> a = new( 1, 2, 1, 3 );

		Vector4<int> expected = new( 2, 4, 2, 6 );

		Assert.That( a.EntrywiseOperation( x => x * 2 ), Is.EqualTo( expected ) );
	}

	[Test]
	public void Dot() {
		Vector4<int> a = new( 1, 2, 3, 4 );
		Vector4<int> b = new( 2, 3, 4, 5 );

		int dot = a.Dot( b );

		Assert.That( dot, Is.EqualTo( 40 ) );
	}

	[Test]
	public void MagnitudeSquared() {
		Vector4<int> a = new( 1, 2, 3, 4 );

		int magnitudeSquared = a.MagnitudeSquared();

		Assert.That( magnitudeSquared, Is.EqualTo( 30 ) );
	}

	//[Test]
	//public void Wedge() {
	//	Vector4<int> a = new( 1, 2, 3, 4 );
	//	Vector4<int> b = new( 2, 3, 3, 4 );

	//	Bivector4<int> wedge = a.Wedge( b );

	//	Assert.That( wedge.XY, Is.EqualTo( -1 ) );
	//}

	[Test]
	public void Min() {
		Vector4<int> a = new( 1, 2, 3, -1 );
		Vector4<int> b = new( 2, 1, 4, 2 );

		Vector4<int> expected = new( 1, 1, 3, -1 );

		Assert.That( a.Min( b ), Is.EqualTo( expected ) );
	}

	[Test]
	public void Max() {
		Vector4<int> a = new( 1, 2, 3, -1 );
		Vector4<int> b = new( 2, 1, 4, 2 );

		Vector4<int> expected = new( 2, 2, 4, 2 );

		Assert.That( a.Max( b ), Is.EqualTo( expected ) );
	}

	[Test]
	public void SumOfParts() {
		Vector4<int> a = new( 2, 3, 5, 7 );

		Assert.That( a.SumOfParts(), Is.EqualTo( 17 ) );
	}

	[Test]
	public void ProductOfParts() {
		Vector4<int> a = new( 2, 3, 5, 7 );

		Assert.That( a.ProductOfParts(), Is.EqualTo( 210 ) );
	}

	[Test]
	public void SumOfUnitBasisAreas() {
		Vector4<int> a = new( 2, 3, 5, 7 );
		//2*3+2*5+2*7+3*5+3*7+5*7
		//6+10+14+15+21+35
		//101
		Assert.That( a.SumOfUnitBasisAreas(), Is.EqualTo( 101 ) );
	}

	[Test]
	public void SumOfUnitBasisVolumes() {
		Vector4<int> a = new( 2, 3, 5, 7 );
		//2*3*5+3*5*7+2*5*7+2*3*7
		//30+105+70+42
		//247
		Assert.That( a.SumOfUnitBasisVolumes(), Is.EqualTo( 247 ) );
	}

	//[Test]
	//public void ReflectNormal() {
	//	Vector4<int> a = new( 1, 2, 1, -5 );
	//	Vector4<int> normal = new( 0, 1, 0, 0 );

	//	Vector2<int> expected = new( -1, 2 );

	//	Assert.That( a.ReflectNormal( normal ), Is.EqualTo( expected ) );
	//}

	[Test]
	public void Test_Equals() {
		Vector4<int> a = new( 1, 2, 3, 4 );

		Assert.That( a.Equals( a ), Is.True );
		Assert.That( a.Equals( new Vector4<int>( 1, 2, 3, 4 ) ), Is.True );
		Assert.That( a.Equals( new Vector4<int>( 2, -3, 66, 123 ) ), Is.False );
		Assert.That( a.Equals( "Test" ), Is.False );
	}

	[Test]
	public void Test_GetHashCode() {
		Vector4<int> a = new( 1, 2, 3, 4 );
		Vector4<int> b = new( 1, 2, 3, 4 );
		Vector4<int> c = new( 2, 5, 5, 6 );
		Vector4<int> d = new( 2, 5, 5, 6 );

		Assert.That( a.GetHashCode(), Is.EqualTo( b.GetHashCode() ) );
		Assert.That( a.GetHashCode(), Is.Not.EqualTo( c.GetHashCode() ) );
		Assert.That( c.GetHashCode(), Is.EqualTo( d.GetHashCode() ) );
	}

	[Test]
	public void Test_ToString() {
		Vector4<int> a = new( 1, -56300, 2, 4 );
		Vector4<double> b = new( -69.4201, 7031, 33, 41 );
		Vector4<int> allNegative = new( -1, -2, -3, -4 );

		Assert.That( a.ToString(), Is.EqualTo( "[1X - 56,300Y + 2Z + 4W]" ) );
		Assert.That( b.ToString(), Is.EqualTo( "[-69.42X + 7,031Y + 33Z + 41W]" ) );
		Assert.That( allNegative.ToString(), Is.EqualTo( "[-1X - 2Y - 3Z - 4W]" ) );
	}
	#endregion

	#region Casts
	[Test]
	public void CastFromScalar() {
		int value = 5;
		Vector4<int> vector = value;

		Vector4<int> expected = new( 5, 5, 5, 5 );

		Assert.That( vector, Is.EqualTo( expected ) );
	}

	[Test]
	public void CastFromTuple() {
		(int, int, int, int) tuple = (5, 7, 9, 11);
		Vector4<int> vector = tuple;

		Vector4<int> expected = new( 5, 7, 9, 11 );

		Assert.That( vector, Is.EqualTo( expected ) );
	}
	#endregion
}
