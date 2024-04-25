using Engine.Math.NewVectors;

namespace Engine.Math.Tests.NewVectors;

[TestFixture]
public sealed class Multivector4Tests {
	#region Initializations
	[Test]
	public void Constructors() {
		Multivector4<int> vectorA = new( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) );
		Multivector4<double> vectorB = new( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) );
		Multivector4<int> vectorC = new( 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 );

		Assert.That( vectorA.Scalar, Is.EqualTo( 1 ) );
		Assert.That( vectorA.Vector.X, Is.EqualTo( 2 ) );
		Assert.That( vectorA.Vector.Y, Is.EqualTo( 3 ) );
		Assert.That( vectorA.Vector.Z, Is.EqualTo( 4 ) );
		Assert.That( vectorA.Vector.W, Is.EqualTo( 5 ) );
		Assert.That( vectorA.Bivector.YZ, Is.EqualTo( 6 ) );
		Assert.That( vectorA.Bivector.ZX, Is.EqualTo( 7 ) );
		Assert.That( vectorA.Bivector.XY, Is.EqualTo( 8 ) );
		Assert.That( vectorA.Bivector.YW, Is.EqualTo( 9 ) );
		Assert.That( vectorA.Bivector.ZW, Is.EqualTo( 10 ) );
		Assert.That( vectorA.Bivector.XW, Is.EqualTo( 11 ) );
		Assert.That( vectorA.Trivector.YZW, Is.EqualTo( 12 ) );
		Assert.That( vectorA.Trivector.XZW, Is.EqualTo( 13 ) );
		Assert.That( vectorA.Trivector.XYW, Is.EqualTo( 14 ) );
		Assert.That( vectorA.Trivector.XYZ, Is.EqualTo( 15 ) );
		Assert.That( vectorA.Quadvector.XYZW, Is.EqualTo( 16 ) );

		Assert.That( vectorB.Scalar, Is.EqualTo( 1.0 ) );
		Assert.That( vectorB.Vector.X, Is.EqualTo( 2.0 ) );
		Assert.That( vectorB.Vector.Y, Is.EqualTo( 3.0 ) );
		Assert.That( vectorB.Vector.Z, Is.EqualTo( 4.0 ) );
		Assert.That( vectorB.Vector.W, Is.EqualTo( 5.0 ) );
		Assert.That( vectorB.Bivector.YZ, Is.EqualTo( 6.0 ) );
		Assert.That( vectorB.Bivector.ZX, Is.EqualTo( 7.0 ) );
		Assert.That( vectorB.Bivector.XY, Is.EqualTo( 8.0 ) );
		Assert.That( vectorB.Bivector.YW, Is.EqualTo( 9.0 ) );
		Assert.That( vectorB.Bivector.ZW, Is.EqualTo( 10.0 ) );
		Assert.That( vectorB.Bivector.XW, Is.EqualTo( 11.0 ) );
		Assert.That( vectorB.Trivector.YZW, Is.EqualTo( 12.0 ) );
		Assert.That( vectorB.Trivector.XZW, Is.EqualTo( 13.0 ) );
		Assert.That( vectorB.Trivector.XYW, Is.EqualTo( 14.0 ) );
		Assert.That( vectorB.Trivector.XYZ, Is.EqualTo( 15.0 ) );
		Assert.That( vectorB.Quadvector.XYZW, Is.EqualTo( 16.0 ) );

		Assert.That( vectorC.Scalar, Is.EqualTo( 1 ) );
		Assert.That( vectorC.Vector.X, Is.EqualTo( 2 ) );
		Assert.That( vectorC.Vector.Y, Is.EqualTo( 3 ) );
		Assert.That( vectorC.Vector.Z, Is.EqualTo( 4 ) );
		Assert.That( vectorC.Vector.W, Is.EqualTo( 5 ) );
		Assert.That( vectorC.Bivector.YZ, Is.EqualTo( 6 ) );
		Assert.That( vectorC.Bivector.ZX, Is.EqualTo( 7 ) );
		Assert.That( vectorC.Bivector.XY, Is.EqualTo( 8 ) );
		Assert.That( vectorC.Bivector.YW, Is.EqualTo( 9 ) );
		Assert.That( vectorC.Bivector.ZW, Is.EqualTo( 10 ) );
		Assert.That( vectorC.Bivector.XW, Is.EqualTo( 11 ) );
		Assert.That( vectorC.Trivector.YZW, Is.EqualTo( 12 ) );
		Assert.That( vectorC.Trivector.XZW, Is.EqualTo( 13 ) );
		Assert.That( vectorC.Trivector.XYW, Is.EqualTo( 14 ) );
		Assert.That( vectorC.Trivector.XYZ, Is.EqualTo( 15 ) );
		Assert.That( vectorC.Quadvector.XYZW, Is.EqualTo( 16 ) );
	}

	[Test]
	public void Identities() {
		Multivector4<int> addtitiveIdentityA = Multivector4<int>.AdditiveIdentity;
		Multivector4<int> multiplicativeIdentityA = Multivector4<int>.MultiplicativeIdentity;
		Multivector4<int> zeroA = Multivector4<int>.Zero;
		Multivector4<int> oneA = Multivector4<int>.One;
		Multivector4<double> addtitiveIdentityB = Multivector4<double>.AdditiveIdentity;
		Multivector4<double> multiplicativeIdentityB = Multivector4<double>.MultiplicativeIdentity;
		Multivector4<double> zeroB = Multivector4<double>.Zero;
		Multivector4<double> oneB = Multivector4<double>.One;

		Assert.That( addtitiveIdentityA, Is.EqualTo( zeroA ) );
		Assert.That( multiplicativeIdentityA, Is.EqualTo( oneA ) );

		Assert.That( zeroA.Scalar, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Vector.X, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Vector.Y, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Vector.Z, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Vector.W, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Bivector.YZ, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Bivector.ZX, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Bivector.XY, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Bivector.YW, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Bivector.ZW, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Bivector.XW, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Trivector.YZW, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Trivector.XZW, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Trivector.XYW, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Trivector.XYZ, Is.EqualTo( 0 ) );
		Assert.That( zeroA.Quadvector.XYZW, Is.EqualTo( 0 ) );

		Assert.That( oneA.Scalar, Is.EqualTo( 1 ) );
		Assert.That( oneA.Vector.X, Is.EqualTo( 1 ) );
		Assert.That( oneA.Vector.Y, Is.EqualTo( 1 ) );
		Assert.That( oneA.Vector.Z, Is.EqualTo( 1 ) );
		Assert.That( oneA.Vector.W, Is.EqualTo( 1 ) );
		Assert.That( oneA.Bivector.YZ, Is.EqualTo( 1 ) );
		Assert.That( oneA.Bivector.ZX, Is.EqualTo( 1 ) );
		Assert.That( oneA.Bivector.XY, Is.EqualTo( 1 ) );
		Assert.That( oneA.Bivector.YW, Is.EqualTo( 1 ) );
		Assert.That( oneA.Bivector.ZW, Is.EqualTo( 1 ) );
		Assert.That( oneA.Bivector.XW, Is.EqualTo( 1 ) );
		Assert.That( oneA.Trivector.YZW, Is.EqualTo( 1 ) );
		Assert.That( oneA.Trivector.XZW, Is.EqualTo( 1 ) );
		Assert.That( oneA.Trivector.XYW, Is.EqualTo( 1 ) );
		Assert.That( oneA.Trivector.XYZ, Is.EqualTo( 1 ) );
		Assert.That( oneA.Quadvector.XYZW, Is.EqualTo( 1 ) );

		Assert.That( addtitiveIdentityB, Is.EqualTo( zeroB ) );
		Assert.That( multiplicativeIdentityB, Is.EqualTo( oneB ) );

		Assert.That( zeroB.Scalar, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Vector.X, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Vector.Y, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Vector.Z, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Vector.W, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Bivector.YZ, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Bivector.ZX, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Bivector.XY, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Bivector.YW, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Bivector.ZW, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Bivector.XW, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Trivector.YZW, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Trivector.XZW, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Trivector.XYW, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Trivector.XYZ, Is.EqualTo( 0.0 ) );
		Assert.That( zeroB.Quadvector.XYZW, Is.EqualTo( 0.0 ) );

		Assert.That( oneB.Scalar, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Vector.X, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Vector.Y, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Vector.Z, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Vector.W, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Bivector.YZ, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Bivector.ZX, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Bivector.XY, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Bivector.YW, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Bivector.ZW, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Bivector.XW, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Trivector.YZW, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Trivector.XZW, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Trivector.XYW, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Trivector.XYZ, Is.EqualTo( 1.0 ) );
		Assert.That( oneB.Quadvector.XYZW, Is.EqualTo( 1.0 ) );
	}
	#endregion

	#region Operators
	[Test]
	public void EqualOpertator_Int() {
		bool equal = new Multivector4<int>( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) ) == new Multivector4<int>( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) );
		bool equal_scalar_isfalse = new Multivector4<int>( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) ) == new Multivector4<int>( 0, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) );
		bool equal_vector_isfalse = new Multivector4<int>( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) ) == new Multivector4<int>( 1, new( 0, 0, 0, 0 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) );
		bool equal_bivector_isfalse = new Multivector4<int>( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) ) == new Multivector4<int>( 1, new( 2, 3, 4, 5 ), new( 0, 0, 0, 0, 0, 0 ), new( 12, 13, 14, 15 ), new( 16 ) );
		bool equal_trivector_isfalse = new Multivector4<int>( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) ) == new Multivector4<int>( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 0, 0, 0, 0 ), new( 16 ) );
		bool equal_quadvector_isfalse = new Multivector4<int>( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) ) == new Multivector4<int>( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 0 ) );

		Assert.That( equal, Is.True );
		Assert.That( equal_scalar_isfalse, Is.False );
		Assert.That( equal_vector_isfalse, Is.False );
		Assert.That( equal_bivector_isfalse, Is.False );
		Assert.That( equal_trivector_isfalse, Is.False );
		Assert.That( equal_quadvector_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Int() {
		bool equal_isfalse = new Multivector4<int>( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) ) != new Multivector4<int>( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) );
		bool notequal_scalar = new Multivector4<int>( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) ) != new Multivector4<int>( 0, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) );
		bool notequal_vector = new Multivector4<int>( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) ) != new Multivector4<int>( 1, new( 0, 0, 0, 0 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) );
		bool notequal_bivector = new Multivector4<int>( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) ) != new Multivector4<int>( 1, new( 2, 3, 4, 5 ), new( 0, 0, 0, 0, 0, 0 ), new( 12, 13, 14, 15 ), new( 16 ) );
		bool notequal_trivector = new Multivector4<int>( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) ) != new Multivector4<int>( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 0, 0, 0, 0 ), new( 16 ) );
		bool notequal_quadvector = new Multivector4<int>( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) ) != new Multivector4<int>( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 0 ) );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( notequal_scalar, Is.True );
		Assert.That( notequal_vector, Is.True );
		Assert.That( notequal_bivector, Is.True );
		Assert.That( notequal_trivector, Is.True );
		Assert.That( notequal_quadvector, Is.True );
	}

	[Test]
	public void EqualOperator_Double() {
		bool equal = new Multivector4<double>( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) ) == new Multivector4<double>( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) );
		bool equal_scalar_isfalse = new Multivector4<double>( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) ) == new Multivector4<double>( 0.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) );
		bool equal_vector_isfalse = new Multivector4<double>( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) ) == new Multivector4<double>( 1.0, new( 0.0, 0.0, 0.0, 0.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) );
		bool equal_bivector_isfalse = new Multivector4<double>( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) ) == new Multivector4<double>( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) );
		bool equal_trivector_isfalse = new Multivector4<double>( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) ) == new Multivector4<double>( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 0.0, 0.0, 0.0, 0.0 ), new( 16.0 ) );
		bool equal_quadvector_isfalse = new Multivector4<double>( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) ) == new Multivector4<double>( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 0.0 ) );

		Assert.That( equal, Is.True );
		Assert.That( equal_scalar_isfalse, Is.False );
		Assert.That( equal_vector_isfalse, Is.False );
		Assert.That( equal_bivector_isfalse, Is.False );
		Assert.That( equal_trivector_isfalse, Is.False );
		Assert.That( equal_quadvector_isfalse, Is.False );
	}

	[Test]
	public void NotEqualOperator_Double() {
		bool equal_isfalse = new Multivector4<double>( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) ) != new Multivector4<double>( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) );
		bool notequal_scalar = new Multivector4<double>( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) ) != new Multivector4<double>( 0.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) );
		bool notequal_vector = new Multivector4<double>( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) ) != new Multivector4<double>( 1.0, new( 0.0, 0.0, 0.0, 0.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) );
		bool notequal_bivector = new Multivector4<double>( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) ) != new Multivector4<double>( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) );
		bool notequal_trivector = new Multivector4<double>( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) ) != new Multivector4<double>( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 0.0, 0.0, 0.0, 0.0 ), new( 16.0 ) );
		bool notequal_quadvector = new Multivector4<double>( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 16.0 ) ) != new Multivector4<double>( 1.0, new( 2.0, 3.0, 4.0, 5.0 ), new( 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 ), new( 12.0, 13.0, 14.0, 15.0 ), new( 0.0 ) );

		Assert.That( equal_isfalse, Is.False );
		Assert.That( notequal_scalar, Is.True );
		Assert.That( notequal_vector, Is.True );
		Assert.That( notequal_bivector, Is.True );
		Assert.That( notequal_trivector, Is.True );
		Assert.That( notequal_quadvector, Is.True );
	}

	[Test]
	public void NegateOperator() {
		Multivector4<int> vectorA = new( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) );

		Multivector4<int> expected = new( -1, new( -2, -3, -4, -5 ), new( -6, -7, -8, -9, -10, -11 ), new( -12, -13, -14, -15 ), new( -16 ) );
		Assert.That( -vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void AddOperator() {
		Multivector4<int> vectorA = new( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) );
		Multivector4<int> vectorB = new( 17, new( 18, 19, 20, 21 ), new( 22, 23, 24, 25, 26, 27 ), new( 28, 29, 30, 31 ), new( 32 ) );

		Multivector4<int> expected = new( 18, new( 20, 22, 24, 26 ), new( 28, 30, 32, 34, 36, 38 ), new( 40, 42, 44, 46 ), new( 48 ) );
		Assert.That( vectorA + vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void SubtractOperator() {
		Multivector4<int> vectorA = new( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) );
		Multivector4<int> vectorB = new( 17, new( 18, 19, 20, 21 ), new( 22, 23, 24, 25, 26, 27 ), new( 28, 29, 30, 31 ), new( 32 ) );

		Multivector4<int> expected = new( -16, new( -16, -16, -16, -16 ), new( -16, -16, -16, -16, -16, -16 ), new( -16, -16, -16, -16 ), new( -16 ) );
		Assert.That( vectorA - vectorB, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarMultiplyOperator() {
		Multivector4<int> vectorA = new( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) );

		Multivector4<int> expected = new( 2, new( 4, 6, 8, 10 ), new( 12, 14, 16, 18, 20, 22 ), new( 24, 26, 28, 30 ), new( 32 ) );
		Assert.That( vectorA * 2, Is.EqualTo( expected ) );
		Assert.That( 2 * vectorA, Is.EqualTo( expected ) );
	}

	[Test]
	public void ScalarDivideOperator() {
		Multivector4<int> vectorA = new( 2, new( 4, 6, 8, 10 ), new( 12, 14, 16, 18, 20, 22 ), new( 24, 26, 28, 30 ), new( 32 ) );

		Multivector4<int> expected = new( 1, new( 2, 3, 4, 5 ), new( 6, 7, 8, 9, 10, 11 ), new( 12, 13, 14, 15 ), new( 16 ) );
		Assert.That( vectorA / 2, Is.EqualTo( expected ) );
	}

	[Test]
	public void DivideScalarOperator() {
		Multivector4<int> vectorA = new( 2, new( 4, 6, 8, 10 ), new( 12, 14, 16, 18, 20, 22 ), new( 24, 26, 28, 30 ), new( 32 ) );

		Multivector4<int> expected = new( 64, new( 32, 21, 16, 12 ), new( 10, 9, 8, 7, 6, 5 ), new( 5, 4, 4, 4 ), new( 4 ) );
		Assert.That( 128 / vectorA, Is.EqualTo( expected ) );
	}
	#endregion

	#region Methods
	[Test]
	public void GetMultivector() {
		Multivector4<int> a = new( 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 );

		Multivector4<int> multivector = a.GetMultivector();

		Assert.That( multivector.Scalar, Is.EqualTo( 1 ) );
		Assert.That( multivector.Vector.X, Is.EqualTo( 2 ) );
		Assert.That( multivector.Vector.Y, Is.EqualTo( 3 ) );
		Assert.That( multivector.Vector.Z, Is.EqualTo( 4 ) );
		Assert.That( multivector.Vector.W, Is.EqualTo( 5 ) );
		Assert.That( multivector.Bivector.YZ, Is.EqualTo( 6 ) );
		Assert.That( multivector.Bivector.ZX, Is.EqualTo( 7 ) );
		Assert.That( multivector.Bivector.XY, Is.EqualTo( 8 ) );
		Assert.That( multivector.Bivector.YW, Is.EqualTo( 9 ) );
		Assert.That( multivector.Bivector.ZW, Is.EqualTo( 10 ) );
		Assert.That( multivector.Bivector.XW, Is.EqualTo( 11 ) );
		Assert.That( multivector.Trivector.YZW, Is.EqualTo( 12 ) );
		Assert.That( multivector.Trivector.XZW, Is.EqualTo( 13 ) );
		Assert.That( multivector.Trivector.XYW, Is.EqualTo( 14 ) );
		Assert.That( multivector.Trivector.XYZ, Is.EqualTo( 15 ) );
		Assert.That( multivector.Quadvector.XYZW, Is.EqualTo( 16 ) );
	}

	[Test]
	public void Dot() {
		Multivector4<int> vectorA = new( 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 );
		Multivector4<int> vectorB = new( 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 );

		int dot = vectorA.Dot( vectorB );

		Assert.That( dot, Is.EqualTo( -2058 ) );
	}

	[Test]
	public void Test_Equals() {
		Multivector4<int> a = new( 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 );

		Assert.That( a.Equals( a ), Is.True );
		Assert.That( a.Equals( new Multivector4<int>( 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 ) ), Is.True );
		Assert.That( a.Equals( new Multivector4<int>( 2, 5, 6, 12, -6, 12, 15, 66, 5, 5, 5, 6, 6, 7, 8, 111 ) ), Is.False );
		Assert.That( a.Equals( "Test" ), Is.False );
	}

	[Test]
	public void Test_GetHashCode() {
		Multivector4<int> multivector_1 = new( 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 );
		Multivector4<int> multivector_2 = new( 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 );
		Multivector4<int> multivector_3 = new( 2, 5, 6, 10, -5, 12, 14, 16, 2, 2, 3, 4, 5, 6, 7, 112 );
		Multivector4<int> multivector_4 = new( 2, 5, 6, 10, -5, 12, 14, 16, 2, 2, 3, 4, 5, 6, 7, 112 );

		Assert.That( multivector_1.GetHashCode(), Is.EqualTo( multivector_2.GetHashCode() ) );
		Assert.That( multivector_1.GetHashCode(), Is.Not.EqualTo( multivector_3.GetHashCode() ) );
		Assert.That( multivector_3.GetHashCode(), Is.EqualTo( multivector_4.GetHashCode() ) );
	}

	[Test]
	public void Test_ToString() {
		Multivector4<int> a = new( 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 );
		Multivector4<double> b = new( -69.4201, 70.5, 50.9999, 23.232, 10, 12, 13, 66.53412, 2, 342, 1.223, 534.123, 73, 192, 111, -2131 );

		Assert.That( a.ToString(), Is.EqualTo( $"<1 + {a.Vector} + {a.Bivector} + {a.Trivector} + {a.Quadvector}>" ) );
		Assert.That( b.ToString(), Is.EqualTo( $"<-69.42 + {b.Vector} + {b.Bivector} + {b.Trivector} + {b.Quadvector}>" ) );
	}
	#endregion
}
