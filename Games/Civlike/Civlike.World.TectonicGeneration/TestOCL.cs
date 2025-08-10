using Civlike.World.State;
using Civlike.World.State.States;
using Civlike.World.TectonicGeneration.NoiseProviders;
using Civlike.World.TectonicGeneration.OpenCL;
using Engine;
using Silk.NET.OpenCL;
using System.Collections.Concurrent;
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

public sealed class TectonicLandmassParameters {
	public int PlateCount { get; set; } = 34;
	public float PlateHeight { get; set; } = -100;
	public float PlateHeightVariance { get; set; } = 200;
	public float PlateHeightNoiseScale { get; set; } = 1.3f;
	public float FaultMaxHeight { get; set; } = 4000;
	public float MountainHeight { get; set; } = 700;
	public float BaseHeightVariance { get; set; } = 50;
}

public sealed class TectonicLandmassGenerator {

	public void Process( Globe globe, TectonicGenerationParameters parameters ) {
		float minPlateHeight = parameters.TectonicParameters.PlateHeight - parameters.TectonicParameters.PlateHeightVariance;
		float maxPlateHeight = parameters.TectonicParameters.PlateHeight + parameters.TectonicParameters.PlateHeightVariance;
		TectonicPlateGenerator tectonicRegionGenerator = new( new( parameters.SeedProvider.Next() ), parameters.TectonicParameters.PlateCount, 0.01f, minPlateHeight, maxPlateHeight, parameters.TectonicParameters.PlateHeightNoiseScale );
		Noise3 xShiftNoise = new( parameters.SeedProvider.Next(), 11 );
		Noise3 yShiftNoise = new( parameters.SeedProvider.Next(), 11 );
		Noise3 zShiftNoise = new( parameters.SeedProvider.Next(), 11 );

		uint baseCoarseSeed = unchecked((uint) parameters.SeedProvider.Next());
		float baseCoarseScale = 11;
		uint baseFineSeed = unchecked((uint) parameters.SeedProvider.Next());
		float baseFineScale = 23;

		Noise3 coarseFaultNoise = new( parameters.SeedProvider.Next(), 7 );
		Noise3 fineFaultNoise = new( parameters.SeedProvider.Next(), 27 );

		Noise3 mountainExistingNoise = new( parameters.SeedProvider.Next(), 11 );
		Noise3 mountainStrengthNoise = new( parameters.SeedProvider.Next(), 6 );
		FiniteVoronoiNoise3 mountainRidgeNoise = new( new( parameters.SeedProvider.Next() ), 0.0625f, 1 );

		Noise3 coarseRuggednessNoise = new( parameters.SeedProvider.Next(), 4 );
		Noise3 fineRuggednessNoise = new( parameters.SeedProvider.Next(), 19 );

		float[] seismicActivity = new float[ globe.Nodes.Count ];
		float[] ruggedness = new float[ globe.Nodes.Count ];
		ParallelProcessing.Range( globe.Nodes.Count, ( start, end, taskId ) => {
			List<(TectonicPlate plate, float gradient)> neighbourPlates = [];
			for (int i = start; i < end; i++) {
				Node node = globe.Nodes[ i ];
				Vector3<float> point = node.Vertex.Vector;

				Vector3<float> shift = new Vector3<float>( xShiftNoise.Noise( point ), yShiftNoise.Noise( point ), zShiftNoise.Noise( point ) ) * 2 - 1;
				Vector3<float> translation = point + shift * 0.05f;

				TectonicPlate current = tectonicRegionGenerator.Get( translation, neighbourPlates, 70, 0.0001f );
				float localRuggedness = coarseRuggednessNoise.Noise( point ) * 0.65f + fineRuggednessNoise.Noise( point ) * 0.35f;
				float currentHeight = current.Height + ((Noise3.Noise( baseCoarseSeed + (uint) current.Id, baseCoarseScale, point ) * 0.65f + Noise3.Noise( baseFineSeed + (uint) current.Id, baseFineScale, point ) * 0.35f) * 2 - 1) * (float) parameters.TectonicParameters.BaseHeightVariance;

				float otherAverageHeight = 0;
				float faultHeight = 0;
				float faultIntensity = 0;
				for (int n = 0; n < neighbourPlates.Count; n++) {
					(TectonicPlate other, float gradient) = neighbourPlates[ n ];
					//float gradientSq = gradient * gradient;

					float otherVariableHeight = ((Noise3.Noise( baseCoarseSeed + (uint) other.Id, baseCoarseScale, point ) * 0.7f + Noise3.Noise( baseFineSeed + (uint) other.Id, baseFineScale, point ) * 0.3f) * 2 - 1) * (float) parameters.TectonicParameters.BaseHeightVariance;
					otherAverageHeight += (other.Height + otherVariableHeight) * gradient;

					float faultMovement = current.GetFaultMovementDifference( other );
					float faultPresence = coarseFaultNoise.Noise( point ) * 0.6f + fineFaultNoise.Noise( point ) * 0.4f;
					faultHeight += faultMovement * (float) parameters.TectonicParameters.FaultMaxHeight * faultPresence * gradient;

					faultIntensity += current.GetFaultReactionIntensity( other ) * float.Sqrt( gradient );
				}

				float mountainFactor = mountainRidgeNoise.BorderNoise( translation, 45 ) * mountainExistingNoise.Noise( point ) * mountainStrengthNoise.Noise( point );
				float mountainHeight = mountainFactor * mountainFactor * (float) parameters.TectonicParameters.MountainHeight;

				float nHeight = currentHeight + otherAverageHeight + faultHeight + mountainHeight;

				NodeLandmassState? state = node.GetState<NodeLandmassState>();
				state.Height = nHeight;
				state.SeismicActivity = faultIntensity;
				state.Ruggedness = localRuggedness;
			}
		} );

		//ParallelProcessing.Range( globe.Tiles.Count, ( start, end, taskId ) => {
		//	List<(TectonicPlate plate, float gradient)> neighbourPlates = [];
		//	for (int i = start; i < end; i++) {
		//		Tile tile = globe.Tiles[ i ];
		//		TectonicFaceState state = tile.GetState<>;

		//		float height = 0;
		//		float faceSeismicActivity = 0;
		//		float faceRuggedness = 0;

		//		foreach (Vertex vertex in face.Vertices) {
		//			height += vertex.Height;
		//			faceSeismicActivity += seismicActivity[ vertex.Id ];
		//			faceRuggedness += ruggedness[ vertex.Id ];
		//		}

		//		height /= 3;
		//		faceSeismicActivity /= 3;
		//		faceRuggedness /= 3;

		//		state.BaselineValues.ElevationMean = height;
		//		state.BaselineValues.SeismicActivity = faceSeismicActivity;
		//		state.BaselineValues.RuggednessFactor = faceRuggedness;

		//		float stddev = 0;
		//		foreach (Vertex vertex in face.Vertices) {
		//			float heightDifference = vertex.Height - height;
		//			stddev += heightDifference * heightDifference;
		//		}
		//		stddev = float.Sqrt( stddev / 3 );

		//		state.BaselineValues.ElevationStandardDeviation = stddev;
		//	}
		//} );

		//globe.UpdateFaceLists();
	}
}
//[Engine.Processing.Do<IGenerationStep>.After<CreateFacesStep>]
//public sealed class GenerateLandmassStep : GlobeGenerationProcessingStepBase<TectonicGeneratingGlobe, TectonicGlobeParameters> {
//	public override string StepDisplayName => "Generating landmasses";

//}
public static class ParallelProcessing {

	public static int ReservedThreads { get; set; } = 2;
	public static int TaskCount => Math.Max( Environment.ProcessorCount - ReservedThreads, 1 );

	public static void Range( int count, Action<int, int, int> parallelizedTask ) {
		int maxDop = Math.Max( Environment.ProcessorCount - ReservedThreads, 1 );
		ParallelOptions options = new() { MaxDegreeOfParallelism = maxDop };

		OrderablePartitioner<Tuple<int, int>> partitions = Partitioner.Create( 0, count, (int) Math.Ceiling( count / (double) maxDop ) );
		int taskId = 0;
		Parallel.ForEach( partitions, options, () => Interlocked.Increment( ref taskId ) - 1,
			( range, loopState, id ) => {
				parallelizedTask( range.Item1, range.Item2, id );
				return id;
			},
			_ => { } );
	}
}