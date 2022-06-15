using System.Diagnostics;
using System.Numerics;
using PhysicsTesting;


Engine.Rendering.Utilities.MeshGenerator.GenerateIcosphereVectors( 2, out List<Vector3> icosphere, out List<uint> indices );
//InertiaPhysicsObject test = new( icosphere.Select( p => new Vector4( p, 1 ) ).ToArray() );
InertiaPhysicsObject test = new( new Vector4[] {
	new( -1, -1, -1, 1 ),
	new( 1, -1, -1, 1 ),
	new( 1, 1, -1, 1 ),
	new( -1, 1, -1, 1 ),
	new( -1, -1, 1, 1 ),
	new( 1, -1, 1, 1 ),
	new( 1, 1, 1, 1 ),
	new( -1, 1, 1, 1 ),
	new( 0, 0, 1, 1 ),
	new( 0, 0, -1, 1 ),
	new( 0, 1, 0, 1 ),
	new( 0, -1, 0, 1 ),
	new( 1, 0, 0, 1 ),
	new( -1, 0, 0, 1 ),
} );

Stopwatch w = Stopwatch.StartNew();
float i16 = 0, i32 = 0, i64 = 0, i128 = 0, i256 = 0;
{
	InertiaCalculator calc = new( 16 );
	Vector3 com = new( 0 );
	double avgMs = 0;
	for ( int i = 0; i < 20; i++ ) {
		long time = w.ElapsedTicks;
		i16 = calc.GetInertia( new Vector3( 0, 1, 0 ), com, new Vector3( 1 ), 1f, ( Vector3 p ) => p.LengthSquared() < 1 ? 1 : 0, out float totalMass );
		long time2 = w.ElapsedTicks;
		avgMs += (double) ( time2 - time ) / TimeSpan.TicksPerMillisecond;
	}
	Console.WriteLine( $"16c in {avgMs / 20:N2} ms" );
} {
	InertiaCalculator calc = new( 32 );
	Vector3 com = new( 0 );
	double avgMs = 0;
	for ( int i = 0; i < 20; i++ ) {
		long time = w.ElapsedTicks;
		i32 = calc.GetInertia( new Vector3( 0, 1, 0 ), com, new Vector3( 1 ), 1f, ( Vector3 p ) => p.LengthSquared() < 1 ? 1 : 0, out float totalMass );
		long time2 = w.ElapsedTicks;
		avgMs += (double) ( time2 - time ) / TimeSpan.TicksPerMillisecond;
	}
	Console.WriteLine( $"32c in {avgMs / 20:N2} ms" );
} {
	InertiaCalculator calc = new( 64 );
	Vector3 com = new( 0 );
	double avgMs = 0;
	for ( int i = 0; i < 20; i++ ) {
		long time = w.ElapsedTicks;
		i64 = calc.GetInertia( new Vector3( 0, 1, 0 ), com, new Vector3( 1 ), 1f, ( Vector3 p ) => p.LengthSquared() < 1 ? 1 : 0, out float totalMass );
		long time2 = w.ElapsedTicks;
		avgMs += (double) ( time2 - time ) / TimeSpan.TicksPerMillisecond;
	}
	Console.WriteLine( $"64c in {avgMs / 20:N2} ms" );
} {
	InertiaCalculator calc = new( 128 );
	Vector3 com = new( 0 );
	double avgMs = 0;
	for ( int i = 0; i < 20; i++ ) {
		long time = w.ElapsedTicks;
		i128 = calc.GetInertia( new Vector3( 0, 1, 0 ), com, new Vector3( 1 ), 1f, ( Vector3 p ) => p.LengthSquared() < 1 ? 1 : 0, out float totalMass );
		long time2 = w.ElapsedTicks;
		avgMs += (double) ( time2 - time ) / TimeSpan.TicksPerMillisecond;
	}
	Console.WriteLine( $"128c in {avgMs / 20:N2} ms" );
} {
	InertiaCalculator calc = new( 256 );
	Vector3 com = new( 0 );
	double avgMs = 0;
	for ( int i = 0; i < 20; i++ ) {
		long time = w.ElapsedTicks;
		i256 = calc.GetInertia( new Vector3( 0, 1, 0 ), com, new Vector3( 1 ), 1f, ( Vector3 p ) => p.LengthSquared() < 1 ? 1 : 0, out float totalMass );
		long time2 = w.ElapsedTicks;
		avgMs += (double) ( time2 - time ) / TimeSpan.TicksPerMillisecond;
	}
	Console.WriteLine( $"256c in {avgMs / 20:N2} ms" );
}

Console.WriteLine( $"c16:   {i16}" );
Console.WriteLine( $"c32:   {i32}" );
Console.WriteLine( $"c64:   {i64}" );
Console.WriteLine( $"c128:  {i128}" );
Console.WriteLine( $"c256:  {i256}" );
Console.WriteLine( $"c16/c256:  {( i256 - i16 ) / i256}%" );
Console.WriteLine( $"c32/c256:  {( i256 - i32 ) / i256}%" );
Console.WriteLine( $"c64/c256:  {( i256 - i64 ) / i256}%" );
Console.WriteLine( $"c128/c256: {( i256 - i128 ) / i256}%" ); 
{
	InertiaCalculator calc = new( 16 );
	Vector3 com = new( 0 );
	float iexp = calc.GetInertiaExp( new Vector3( 0, 1, 0 ), com, new Vector3( 1 ), 1f, ( Vector3 p ) => p.LengthSquared() < 1 ? 1 : 0, out float totalMass );
	Console.WriteLine( $"iexp:   {iexp}" );
}

//https://stackoverflow.com/questions/25179693/how-to-check-whether-the-point-is-in-the-tetrahedron-or-not
//http://www.cs.cmu.edu/~quake/triangle.html