using BenchmarkDotNet.Running;
using Engine.Physics;
using Engine.Rendering;
using System.Diagnostics;
using System.Numerics;
using TestPlatform;

Console.WriteLine( Vector.IsHardwareAccelerated );
var i_ = 1f / 12 * 1 * ( 1 * 1 + 1 * 1 );

var cubeVertices = new[] {
	new Vector3( 0, 0, 0 ) - new Vector3(.5f),
	new Vector3( 1, 0, 0 ) - new Vector3(.5f),
	new Vector3( 1, 1, 0 ) - new Vector3(.5f),
	new Vector3( 0, 1, 0 ) - new Vector3(.5f),
	new Vector3( 0, 0, 1 ) - new Vector3(.5f),
	new Vector3( 1, 0, 1 ) - new Vector3(.5f),
	new Vector3( 1, 1, 1 ) - new Vector3(.5f),
	new Vector3( 0, 1, 1 ) - new Vector3(.5f)
};
var hull1 = new ConvexHull3( cubeVertices );
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
var ine3 = Vector3.Transform( new Vector3( 0, 0, 1 ), inertia3 );

//BenchmarkRunner.Run<Benchmark2>();
//new RenderModule();

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
Console.ReadLine();

//Find out how to transform the tensor