using Civlike.World.TectonicGeneration.OpenCL;
using Silk.NET.OpenCL;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Civlike.World.TectonicGeneration;

#region Host structs matching particle.h
[StructLayout( LayoutKind.Sequential )]
public struct Float4 { public float x, y, z, w; }

[StructLayout( LayoutKind.Sequential )]
public struct Particle {
	public Float4 pos;
	public Float4 vel;
	public float mass;
	public int state;
	public float _pad0, _pad1;
}
#endregion

public class TestOCL {
	public static void Main() {
		using OpenClHost host = new();
		using KernelRegistry reg = new( host );

		string kernelsDir = Path.GetFullPath( Path.Combine( AppContext.BaseDirectory, "res/kernels" ) );
		string includeDir = Path.Combine( kernelsDir, "include" );

		// Use forward slashes + quotes to survive spaces
		string incOpt = $"-I \"{includeDir.Replace( "\\", "/" )}\" -cl-fast-relaxed-math";

		// Load with absolute folder + options
		reg.LoadFolder( kernelsDir, incOpt );
		// Include path for headers + fast math (optional)
		//reg.LoadFolder( "res\\kernels", "-I kernels/include -cl-fast-relaxed-math" );

		// ---------------------------
		// 1) Simple "Add" demonstration
		// ---------------------------
		const int N = 379_625_062;
		Span<float> a = Enumerable.Range( 0, N ).Select( i => (float) i ).ToArray();
		Span<float> b = Enumerable.Range( 0, N ).Select( i => (float) (2 * i) ).ToArray();
		Span<float> c = new float[ N ];
		nuint bytesF = (nuint) N * sizeof( float );
		nint bufA = host.CreateBufferBytes( bytesF, MemFlags.ReadOnly );
		nint bufB = host.CreateBufferBytes( bytesF, MemFlags.ReadOnly );
		nint bufC = host.CreateBufferBytes( bytesF, MemFlags.WriteOnly );
		host.WriteBuffer( bufA, (ReadOnlySpan<float>) a );
		host.WriteBuffer( bufB, (ReadOnlySpan<float>) b );

		nint kAdd = reg.Get( "Add" );
		OpenClHost.SetArgMem( kAdd, 0, bufA );
		OpenClHost.SetArgMem( kAdd, 1, bufB );
		OpenClHost.SetArgMem( kAdd, 2, bufC );
		OpenClHost.SetArgVal( kAdd, 3, N );
		double currentTimeS = (double)Stopwatch.GetTimestamp() / Stopwatch.Frequency;
		Console.WriteLine("Start add");
		host.Run1D( kAdd,  N );
		double currentTimeS2 = (double) Stopwatch.GetTimestamp() / Stopwatch.Frequency;
		Console.WriteLine($"Concluded add, used {currentTimeS2 - currentTimeS:N5}s");
		host.ReadBuffer( bufC, c );
		Console.WriteLine( $"Add: {c[ 0 ]}, {c[ 1 ]}, {c[ 2 ]} .. {c[ ^3 ]}, {c[ ^2 ]}, {c[ ^1 ]}" );

		// -------------------------------------
		// 2) Physics: ping-pong between buffers
		// -------------------------------------
		int M = 200_000; // particle count
		Span<Particle> particles = new Particle[ M ];
		for (int i = 0; i < M; i++) {
			particles[ i ].pos = new Float4 { x = i * 0.001f, y = 1, z = 0, w = 0 };
			particles[ i ].vel = new Float4 { x = 0, y = -9.8f, z = 0, w = 0 };
			particles[ i ].mass = 1f;
		}

		nuint bytesP = (nuint) (Marshal.SizeOf<Particle>() * (long) M);
		nint bufCur = host.CreateBufferBytes( bytesP, MemFlags.ReadWrite );
		nint bufNext = host.CreateBufferBytes( bytesP, MemFlags.ReadWrite );
		host.WriteBuffer( bufCur, (ReadOnlySpan<Particle>) particles ); // upload once

		nint kIntegrate = reg.Get( "Integrate" );
		nint kCollide = reg.Get( "Collide" );

		float dt = 0.016f, floorY = 0.0f, k = 50.0f;

		for (int step = 0; step < 60; step++) // 60 simulation steps
		{
			// Step A: Integrate (cur -> next)
			OpenClHost.SetArgMem( kIntegrate, 0, bufCur );
			OpenClHost.SetArgMem( kIntegrate, 1, bufNext );
			OpenClHost.SetArgVal( kIntegrate, 2, dt );
			OpenClHost.SetArgVal( kIntegrate, 3, M );
			host.Run1D( kIntegrate, (nuint) M );

			// Step B: Collide (next -> cur)
			OpenClHost.SetArgMem( kCollide, 0, bufNext );
			OpenClHost.SetArgMem( kCollide, 1, bufCur );
			OpenClHost.SetArgVal( kCollide, 2, floorY );
			OpenClHost.SetArgVal( kCollide, 3, k );
			OpenClHost.SetArgVal( kCollide, 4, M );
			host.Run1D( kCollide, (nuint) M );

			// Swap roles for next iteration (ping-pong)
			(bufCur, bufNext) = (bufNext, bufCur);
		}

		// Read back a small header or a few elements (partial read)
		Span<Particle> few = new Particle[ 4 ];
		host.ReadBufferRegion( bufCur, 0, few ); // first 4
		Console.WriteLine( $"After sim: p0=({few[ 0 ].pos.x:F2},{few[ 0 ].pos.y:F2}) v0=({few[ 0 ].vel.x:F2},{few[ 0 ].vel.y:F2})" );

		Console.WriteLine("Done!");
		Console.ReadLine();
	}
}


public sealed class TectonicGlobeGenerator : IGlobeGenerator {
	public int Subdivisions => 7;

	public double Radius => 6378000;

	public void GenerateInitialGlobeState( Globe globe ) {

	}
}