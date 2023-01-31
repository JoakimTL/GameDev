using BenchmarkDotNet.Running;
using Engine;
using Engine.Datatypes.Buffers;
using TestPlatform;

Random rand = new(32);
BinaryTree<int> _binaryTree;
int[] _toInsert;
int Size = 8192;
_binaryTree = new();
_toInsert = new int[ Size ];
for ( int i = 0; i < Size; i++ ) {
	_toInsert[ i ] = rand.Next( 0, 100 );
}

for ( int i = 0; i < Size; i++ ) {
	_binaryTree.Add( _toInsert[ i ] );
}

Console.WriteLine( $"uns {string.Join( ", ", _toInsert.Select( p => p.ToString() ) )}" );
Console.WriteLine( $"tre {string.Join( ", ", _binaryTree.Select( p => p.ToString() ) )}" );
Console.WriteLine( $"ord {string.Join( ", ", _toInsert.OrderBy( p => p ).Select( p => p.ToString() ) )}" );
Console.WriteLine( _binaryTree.SequenceEqual( _toInsert.OrderBy( p => p ) ) );


BenchmarkRunner.Run<Benchmarking2>();
Console.ReadLine();

//new Engine.Rendering.RenderModule();
//new TestGameLogicModule();

//var t1 = new Transform3();
//t1.Translation = new( 0, -.5f, 0 );
//t1.Rotation = Quaternion.CreateFromYawPitchRoll( 40, 23, 0 );
//t1.Scale = new( 2, 0.5f, 2 );

//var t2 = new Transform3();
//t2.Translation = new( 2, .5f, 0 );
//t2.Scale = new( .5f, 1, .5f );

//Console.WriteLine( $"Local:  {t2.Translation}, {t2.Rotation}, {t2.Scale}" );
//Console.WriteLine( $"GlobalM: {t2.Matrix}" );
//Console.WriteLine( $"Global: {t2.GlobalTranslation}, {t2.GlobalRotation}, {t2.GlobalScale}" );

//t2.SetParent( t1, false );
//Console.WriteLine( $"Local:   {t2.Translation}, {t2.Rotation}, {t2.Scale}" );
//Console.WriteLine( $"GlobalM: {t2.Matrix}" );
//Console.WriteLine( $"Global:  {t2.GlobalTranslation}, {t2.GlobalRotation}, {t2.GlobalScale}" );

//t2.SetParent( null, false );
//Console.WriteLine( $"Local:  {t2.Translation}, {t2.Rotation}, {t2.Scale}" );
//Console.WriteLine( $"GlobalM: {t2.Matrix}" );
//Console.WriteLine( $"Global: {t2.GlobalTranslation}, {t2.GlobalRotation}, {t2.GlobalScale}" );

//t2.SetParent( t1, true );
//Console.WriteLine( $"Local:  {t2.Translation}, {t2.Rotation}, {t2.Scale}" );
//Console.WriteLine( $"GlobalM: {t2.Matrix}" );
//Console.WriteLine( $"Global: {t2.GlobalTranslation}, {t2.GlobalRotation}, {t2.GlobalScale}" );

//Console.WriteLine( "Translate!" );
//t1.Translation = new( 10, -.5f, 0 );
//Console.WriteLine( $"Local:  {t2.Translation}, {t2.Rotation}, {t2.Scale}" );
//Console.WriteLine( $"GlobalM: {t2.Matrix}" );
//Console.WriteLine( $"Global: {t2.GlobalTranslation}, {t2.GlobalRotation}, {t2.GlobalScale}" );

//t2.SetParent( t1, false );
//Console.WriteLine( $"Local:  {t2.Translation}, {t2.Rotation}, {t2.Scale}" );
//Console.WriteLine( $"GlobalM: {t2.Matrix}" );
//Console.WriteLine( $"Global: {t2.GlobalTranslation}, {t2.GlobalRotation}, {t2.GlobalScale}" );

//t2.SetParent( null, true );
//Console.WriteLine( $"Local:  {t2.Translation}, {t2.Rotation}, {t2.Scale}" );
//Console.WriteLine( $"GlobalM: {t2.Matrix}" );
//Console.WriteLine( $"Global: {t2.GlobalTranslation}, {t2.GlobalRotation}, {t2.GlobalScale}" );


//Console.WriteLine( Vector.IsHardwareAccelerated );
//var i_ = 1f / 12 * 1 * ( 1 * 1 + 1 * 1 );

//MeshGeneration.GenerateIcosphereVectors( 3, out var verticeList, out _ );
//var vertices = verticeList.ToArray();
//var vertices = new Vector3[ 200 ];
//for ( int i = 0; i < 100; i++ ) {
//	var a = i / 100f * MathF.PI * 2;
//	var x = MathF.Cos( a );
//	var y = MathF.Sin( a );
//	vertices[ i ] = new Vector3( x, y, 0 );
//	vertices[ i + 100 ] = new Vector3( x, y, 1 );
//}
//var vertices = new[] {
//	new Vector3( 0, 0, 0 ) - new Vector3(.5f),
//	new Vector3( 1, 0, 0 ) - new Vector3(.5f),
//	new Vector3( 1, 1, 0 ) - new Vector3(.5f),
//	new Vector3( 0, 1, 0 ) - new Vector3(.5f),
//	new Vector3( 0, 0, 1 ) - new Vector3(.5f),
//	new Vector3( 1, 0, 1 ) - new Vector3(.5f),
//	new Vector3( 1, 1, 1 ) - new Vector3(.5f),
//	new Vector3( 0, 1, 1 ) - new Vector3(.5f)
//};

//var scaleX = new Vector3( 3, 1, 1 );
//var scaleY = new Vector3( 1, 3, 1 );
//var scaleZ = new Vector3( 1, 1, 3 );
//var scaleXY = new Vector3( 3, 3, 1 );
//var scaleYZ = new Vector3( 1, 3, 3 );
//var scaleXZ = new Vector3( 3, 1, 3 );
//var scaleXYZ = new Vector3( 3, 3, 3 );
//var mscaleX = Matrix4x4.CreateScale( scaleX );
//var mscaleY = Matrix4x4.CreateScale( scaleY );
//var mscaleZ = Matrix4x4.CreateScale( scaleZ );
//var mscaleXY = Matrix4x4.CreateScale( scaleXY );
//var mscaleYZ = Matrix4x4.CreateScale( scaleYZ );
//var mscaleXZ = Matrix4x4.CreateScale( scaleXZ );
//var mscaleXYZ = Matrix4x4.CreateScale( scaleXYZ );

//var hullBase = new Engine.Physics.D3T.ConvexHull3( vertices, 1 );
////var hullScaleX = new ConvexHull3( vertices.Select( p => Vector3.Transform( p, mscaleX ) ).ToArray(), 1 );
////var hullScaleY = new ConvexHull3( vertices.Select( p => Vector3.Transform( p, mscaleY ) ).ToArray(), 1 );
////var hullScaleZ = new ConvexHull3( vertices.Select( p => Vector3.Transform( p, mscaleZ ) ).ToArray(), 1 );
////var hullScaleXY = new ConvexHull3( vertices.Select( p => Vector3.Transform( p, mscaleXY ) ).ToArray(), 1 );
////var hullScaleYZ = new ConvexHull3( vertices.Select( p => Vector3.Transform( p, mscaleYZ ) ).ToArray(), 1 );
////var hullScaleXZ = new ConvexHull3( vertices.Select( p => Vector3.Transform( p, mscaleXZ ) ).ToArray(), 1 );
////var hullScaleXYZ = new ConvexHull3( vertices.Select( p => Vector3.Transform( p, mscaleXYZ ) ).ToArray(), 1 );
//var inertia = hullBase.GetInertiaTensor();
////var inertiaX = hullScaleX.GetInertiaTensor();
////var inertiaY = hullScaleY.GetInertiaTensor();
////var inertiaZ = hullScaleZ.GetInertiaTensor();
////var inertiaXY = hullScaleXY.GetInertiaTensor();
////var inertiaYZ = hullScaleYZ.GetInertiaTensor();
////var inertiaXZ = hullScaleXZ.GetInertiaTensor();
////var inertiaXYZ = hullScaleXYZ.GetInertiaTensor();


//Console.WriteLine( "base" );
//printmatrix( inertia );
//Console.WriteLine();
//Console.WriteLine( Vector3.Transform( Vector3.One, inertia ) );

//Console.WriteLine( "scales" );
//Console.WriteLine( scaleX );
//printmatrix( inertiaX );
//printmatrix( inertia.ScaleInertia( scaleX ) );
//Console.WriteLine( scaleY );
//printmatrix( inertiaY );
//printmatrix( inertia.ScaleInertia( scaleY ) );
//Console.WriteLine( scaleZ );
//printmatrix( inertiaZ );
//printmatrix( inertia.ScaleInertia( scaleZ ) );
//Console.WriteLine( scaleXY );
//printmatrix( inertiaXY );
//printmatrix( inertia.ScaleInertia( scaleXY ) );
//Console.WriteLine( scaleYZ );
//printmatrix( inertiaYZ );
//printmatrix( inertia.ScaleInertia( scaleYZ ) );
//Console.WriteLine( scaleXZ );
//printmatrix( inertiaXZ );
//printmatrix( inertia.ScaleInertia( scaleXZ ) );
//Console.WriteLine( scaleXYZ );
//printmatrix( inertiaXYZ );
//printmatrix( inertia.ScaleInertia( scaleXYZ ) );

//void printmatrix( Matrix4x4 m ) {
//	Console.WriteLine( $"{m.M11:N3} {m.M12:N3} {m.M13:N3} {m.M14:N3}" );
//	Console.WriteLine( $"{m.M21:N3} {m.M22:N3} {m.M23:N3} {m.M24:N3}" );
//	Console.WriteLine( $"{m.M31:N3} {m.M32:N3} {m.M33:N3} {m.M34:N3}" );
//	Console.WriteLine( $"{m.M41:N3} {m.M42:N3} {m.M43:N3} {m.M44:N3}" );
//	Console.WriteLine();
//} {

//var a = new Matrix4x4(
//	1, 0, 0, 2,
//	0, 1, 0, 0,
//	0, 0, 1, 0,
//	0, 0, 0, 0 );

//var b = new Matrix4x4(
//	1, 0, 0, 0,
//	0, 1, 0, 2,
//	0, 0, 1, 0,
//	0, 0, 0, 0 );

//var c = new Matrix4x4(
//	1, 0, 0, 0,
//	0, 1, 0, 0,
//	0, 0, 1, 2,
//	0, 0, 0, 0 );

//var d = new Matrix4x4(
//	1, 0, 0, 0,
//	0, 1, 0, 0,
//	0, 0, 1, 0,
//	2, 0, 0, 0 );

//var e = new Matrix4x4(
//	1, 0, 0, 0,
//	0, 1, 0, 0,
//	0, 0, 1, 0,
//	0, 2, 0, 0 );

//var f = new Matrix4x4(
//	1, 0, 0, 0,
//	0, 1, 0, 0,
//	0, 0, 1, 0,
//	0, 0, 2, 0 );

//var g = new Matrix4x4(
//	1, 0, 0, 0,
//	0, 1, 0, 0,
//	0, 0, 1, 0,
//	0, 0, 0, 2 );

//Console.WriteLine( Vector3.Transform( Vector3.One, a ) );
//Console.WriteLine( Vector3.TransformNormal( Vector3.One, a ) );
//Console.WriteLine();
//Console.WriteLine( Vector3.Transform( Vector3.One, b ) );
//Console.WriteLine( Vector3.TransformNormal( Vector3.One, b ) );
//Console.WriteLine();
//Console.WriteLine( Vector3.Transform( Vector3.One, c ) );
//Console.WriteLine( Vector3.TransformNormal( Vector3.One, c ) );
//Console.WriteLine();
//Console.WriteLine( Vector3.Transform( Vector3.One, d ) );
//Console.WriteLine( Vector3.TransformNormal( Vector3.One, d ) );
//Console.WriteLine();
//Console.WriteLine( Vector3.Transform( Vector3.One, e ) );
//Console.WriteLine( Vector3.TransformNormal( Vector3.One, e ) );
//Console.WriteLine();
//Console.WriteLine( Vector3.Transform( Vector3.One, f ) );
//Console.WriteLine( Vector3.TransformNormal( Vector3.One, f ) );
//Console.WriteLine();
//Console.WriteLine( Vector3.Transform( Vector3.One, g ) );
//	//Console.WriteLine( Vector3.TransformNormal( Vector3.One, g ) );
//}
/*
Matrix4x4 tMat = Matrix4x4.CreateFromYawPitchRoll( 1.2f, 0.7f, 2.6f ) * Matrix4x4.CreateScale( new Vector3( 2, 2, 1 ) );
var hull1Transformed = new ConvexHull3( cubeVertices.Select( p => Vector3.Transform( p, tMat ) ).ToArray() );

var hull2 = new ConvexHull3( new[] {
	new Vector3( 8.3322f, -11.86875f, 0.93355f ),
	new Vector3( 0.75523f, 5.0000f, 16.37072f ),
	new Vector3( 52.61236f, 5.0000f, -5.38580f ),
	new Vector3( 2.0000f, 5.0000f, 3.0000f )
	}
);

var cyllinderVertices = new Vector3[ 200 ];
for ( int i = 0; i < 100; i++ ) {
	var a = i / 100f * MathF.PI * 2;
	var x = MathF.Cos( a );
	var y = MathF.Sin( a );
	cyllinderVertices[ i ] = new Vector3( x, y, 0 );
	cyllinderVertices[ i + 100 ] = new Vector3( x, y, 1 );
}
Console.WriteLine( Stopwatch.GetTimestamp() / (double) TimeSpan.TicksPerSecond );
var hull3 = new ConvexHull3( cyllinderVertices );
Console.WriteLine( Stopwatch.GetTimestamp() / (double) TimeSpan.TicksPerSecond );

var inertia1 = hull1.GetInertiaTensor();
var inertia1T = hull1Transformed.GetInertiaTensor();
var inertia2 = hull2.GetInertiaTensor();
var inertia3 = hull3.GetInertiaTensor();

Matrix4x4.Invert( tMat, out var itMat );
var inertia1TF = tMat * inertia1;

var ine1 = Vector3.Transform( new Vector3( 0, 0, 1 ), inertia1 );
var ine1T = Vector3.Transform( new Vector3( 0, 0, 1 ), inertia1T );
var ine3 = Vector3.Transform( new Vector3( 0, 0, 1 ), inertia3 );*/

//BenchmarkRunner.Run<Benchmark2>();

//Console.WriteLine( (typeof( Log ), typeof( Identifiable )).GetHashCode() );
//Console.WriteLine( (typeof( Log ), typeof( Identifiable )).GetHashCode() );

//Int128 number =  new( 1517421763156, 12300551235240233304 );
//Int128 number2 = new( 1314123315677, 12300541235340231304 );
//var n3 = number - number2;
//var n3l = (long)n3;
//var n4 = (int) n3;
//var n4fl = (int) n3l;
//var n4uc = unchecked((int) n3);
//var n4fluc = unchecked((int) n3l);

//Console.WriteLine( $"{number} - {number2} = {n3}" );

//Console.WriteLine( n3 );
//Console.WriteLine( n3l );
//Console.WriteLine( n4 );
//Console.WriteLine( n4fl );
//Console.WriteLine( n4uc );
//Console.WriteLine( n4fluc );
//Console.WriteLine( (double) n3 );
//Console.WriteLine( (float) n3 );
////https://blog.demofox.org/2017/11/21/floating-point-precision/
//Console.ReadLine();

//Find out how to transform the tensor (check comment under)
//Rather inverse transform the force vector direction to check inertia at local level rather than global level