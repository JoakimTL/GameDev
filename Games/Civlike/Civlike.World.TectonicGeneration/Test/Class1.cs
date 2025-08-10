//using Civlike.World.State;
//using Civlike.World.TectonicGeneration;
//using Civlike.World.TectonicGeneration.NoiseProviders;
//using Engine;
//using System;
//using System.Buffers;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Numerics;

//namespace Game.WorldGen.Tectonics;

///*──────────────────────────────  GEOMETRY  ──────────────────────────────*/
//static class Geometry {
//	public static Vector3<float> RandomUnitVector( Random r ) {
//		float x, y, s;
//		do {
//			x = (float) (r.NextDouble() * 2d - 1d);
//			y = (float) (r.NextDouble() * 2d - 1d);
//			s = x * x + y * y;
//		} while (s >= 1f || s == 0f);
//		float f = MathF.Sqrt( 1f - 2f * s );
//		return new( x * 2f * f, y * 2f * f, 1f - 2f * s );
//	}

//	public static float GreatCircle( Vector3<float> a, Vector3<float> b )
//		=> MathF.Acos( Math.Clamp( Vector3.Dot( a, b ), -1f, 1f ) );
//}

///*────────────────────  VARIABLE-RADIUS POISSON ON SPHERE  ───────────────*/
//static class Poisson {
//	public static List<Vector3<float>> SampleOnSphere( float rMin, float rMax,
//													  int k = 30, Random? rnd = null ) {
//		rnd ??= Random.Shared;
//		var pts = new List<Vector3<float>>();
//		if (rMax <= 0f)
//			return pts;

//		float cell = rMin / MathF.Sqrt( 2f );
//		var buckets = new Dictionary<(int, int), List<int>>();

//		(int, int) Key( Vector3<float> v ) {
//			float θ = MathF.Acos( v.Z );
//			float φ = MathF.Atan2( v.Y, v.X );
//			return ((int) (θ / cell), (int) ((φ + MathF.PI) / cell));
//		}

//		bool Fits( Vector3<float> p ) {
//			var key = Key( p );
//			for (int di = -2; di <= 2; di++)
//				for (int dj = -2; dj <= 2; dj++) {
//					if (!buckets.TryGetValue( (key.Item1 + di, key.Item2 + dj), out var list ))
//						continue;
//					foreach (int i in list)
//						if (Geometry.GreatCircle( p, pts[ i ] ) < rMin)
//							return false;
//				}
//			return true;
//		}

//		Vector3<float> first = Geometry.RandomUnitVector( rnd );
//		pts.Add( first );
//		buckets[ Key( first ) ] = new() { 0 };
//		var active = new List<int> { 0 };

//		while (active.Count > 0) {
//			int idx = active[ rnd.Next( active.Count ) ];
//			Vector3<float> c = pts[ idx ];
//			bool spawned = false;

//			for (int attempt = 0; attempt < k; attempt++) {
//				// random dir
//				float u = (float) rnd.NextDouble(), v = (float) rnd.NextDouble();
//				float cosT = 1f - 2f * u, sinT = MathF.Sqrt( 1f - cosT * cosT );
//				float φ = 2f * MathF.PI * v;
//				var dir = new Vector3<float>( sinT * MathF.Cos( φ ), sinT * MathF.Sin( φ ), cosT );
//				var cand = Vector3.Normalize( dir + c );

//				if (!Fits( cand ))
//					continue;

//				int id = pts.Count;
//				pts.Add( cand );
//				active.Add( id );
//				buckets.TryAdd( Key( cand ), new() );
//				buckets[ Key( cand ) ].Add( id );
//				spawned = true;
//				break;
//			}
//			if (!spawned)
//				active.Remove( idx );
//		}
//		return pts;
//	}
//}

///*──────────────────────────  ONE-PASS LLOYD  ────────────────────────────*/
//static class VoronoiRelax {
//	public static void Relax( List<Vector3<float>> seeds, IReadOnlyList<Vector3<float>> verts ) {
//		int n = seeds.Count;
//		var acc = ArrayPool<Vector3<float>>.Shared.Rent( n );
//		var count = ArrayPool<int>.Shared.Rent( n );
//		Array.Fill( acc, Vector3<float>.Zero );
//		Array.Fill( count, 0 );

//		foreach (var v in verts) {
//			int best = 0;
//			float bestDot = Vector3.Dot( v, seeds[ 0 ] );
//			for (int s = 1; s < n; s++) {
//				float d = Vector3.Dot( v, seeds[ s ] );
//				if (d > bestDot) { bestDot = d; best = s; }
//			}
//			acc[ best ] += v;
//			count[ best ]++;
//		}

//		for (int s = 0; s < n; s++)
//			if (count[ s ] > 0)
//				seeds[ s ] = Vector3.Normalize( acc[ s ] / count[ s ] );

//		ArrayPool<Vector3<float>>.Shared.Return( acc, true );
//		ArrayPool<int>.Shared.Return( count, true );
//	}
//}

///*───────────────────────────  EDGE WIGGLE  ──────────────────────────────*/
//static class EdgePerturber {
//	public static void WiggleEdges( IEnumerable<Edge> edges, float freq, float amp ) {
//		foreach (var e in edges) {
//			for (int i = 1; i < e.Points.Length - 1; i++) {
//				float t = i / (float) (e.Points.Length - 1);
//				float n = Noise3.Noise( e.Seed, freq, t );
//				e.Points[ i ] += Vector3.Normalize( e.Points[ i ] ) * n * amp;
//			}
//		}
//	}
//}

///*──────────────────────  CONTINENTAL FLOOD-FILL  ───────────────────────*/
//static class FloodFill {
//	public static void MarkContinental( TectonicPlate plate, float kernelPct ) {
//		int tot = plate.Faces.Count, keep = (int) (tot * kernelPct);
//		plate.Faces.Sort( ( a, b ) => Vector3.Dot( plate.Position, b.Center )
//							  .CompareTo( Vector3.Dot( plate.Position, a.Center ) ) );
//		for (int i = 0; i < tot; i++)
//			plate.Faces[ i ].IsContinental = i < keep;
//	}
//	public static void MarkOceanic( TectonicPlate p ) { foreach (var f in p.Faces) if (!f.IsContinental) f.IsOceanic = true; }
//}

///*────────────────────────────  ISOSTASY  ───────────────────────────────*/
//static class Isostasy {
//	private const float ρm = 3.30f;
//	public static float Elev( float thKm, float ρc ) => (ρm - ρc) / ρm * thKm * 1000f;
//}

///*───────────────────────  DYNAMIC TOPOGRAPHY  ───────────────────────────*/
//sealed class DynamicTopoField {
//	readonly Noise3 _n; readonly float _amp;
//	DynamicTopoField( Noise3 n, float a ) { _n = n; _amp = a; }
//	public float Sample( Vector3<float> p ) => _n.Noise( p ) * _amp;
//	public static DynamicTopoField Build( int seed ) => new( new( seed, 0.3f ), 1500f );
//}

///*────────────────────────  INTERIOR FEATURES  ───────────────────────────*/
//enum FeatureKind { Shield, Basin, Rift, Hotspot }
//readonly record struct Feature( FeatureKind Kind, int FaceIdx, float RadiusKm, float Δh );

//static class FeaturePlanner {
//	public static List<Feature> Plan( IReadOnlyList<TectonicPlate> plates, Random r ) {
//		var list = new List<Feature>();
//		foreach (var pl in plates) {
//			foreach (var f in pl.Faces) {
//				if (f.IsContinental && f.CrustAgeGa > 1.8f && r.NextSingle() < .1f)
//					list.Add( new( FeatureKind.Shield, f.Index, 400, r.NextSingle() * 700 + 800 ) );
//				if (f.IsContinental && f.CrustThicknessKm < 40 && r.NextSingle() < .05f)
//					list.Add( new( FeatureKind.Basin, f.Index, 600, -(r.NextSingle() * 600 + 300) ) );
//			}
//			if (r.NextSingle() < .2f) {
//				var any = pl.Faces[ r.Next( pl.Faces.Count ) ];
//				list.Add( new( FeatureKind.Hotspot, any.Index, 700, r.NextSingle() * 800 + 200 ) );
//			}
//			if (r.NextSingle() < .3f) {
//				var any = pl.Faces[ r.Next( pl.Faces.Count ) ];
//				list.Add( new( FeatureKind.Rift, any.Index, 120, -1500 ) );
//			}
//		}
//		return list;
//	}
//}

//sealed class FeatureField {
//	readonly Dictionary<int, float> _add = new();
//	public FeatureField( IEnumerable<Feature> feats ) {
//		foreach (var f in feats)
//			_add[ f.FaceIdx ] = _add.GetValueOrDefault( f.FaceIdx ) + f.Δh;
//	}
//	public float Sample( int face ) => _add.GetValueOrDefault( face );
//}

///*────────────────  UPDATED TECTONIC LANDMASS GENERATOR  ────────────────*/
//sealed class TectonicLandmassGenerator {
//	public void Process( Globe globe, TectonicGenerationParameters p ) {
//		var rnd = new Random( p.SeedProvider.Next() );

//		/* 1️⃣  Build plates with new helpers --------------------------------*/
//		float rMin = .04f, rMax = .15f;                       // radians
//		var seeds = Poisson.SampleOnSphere( rMin, rMax, rnd: rnd );
//		VoronoiRelax.Relax( seeds, globe.AllVertices );

//		var plates = new List<TectonicPlate>();
//		for (int i = 0; i < seeds.Count; i++)
//			plates.Add( new TectonicPlate { Id = i, Position = seeds[ i ] } );

//		globe.AssignFacesToPlates( plates );                    // ← your method
//		EdgePerturber.WiggleEdges( globe.PlateEdges, 3f, .015f );

//		/* 2️⃣  Continental kernels & crust properties ------------------------*/
//		foreach (var pl in plates) {
//			FloodFill.MarkContinental( pl, .30f );
//			FloodFill.MarkOceanic( pl );
//			foreach (var f in pl.Faces) {
//				f.CrustAgeGa = f.IsContinental
//									? rnd.NextSingle() * 1.7f + 1.8f
//									: rnd.NextSingle() * .2f;
//				f.CrustThicknessKm = f.IsContinental
//									? rnd.NextSingle() * 25 + 35
//									: rnd.NextSingle() * 3 + 7;
//				f.CrustDensity = f.IsContinental ? 2.80f : 2.90f;
//			}
//		}

//		/* 3️⃣  Dynamic topography & interior features ------------------------*/
//		var dynTopo = DynamicTopoField.Build( rnd.Next() );
//		var featField = new FeatureField( FeaturePlanner.Plan( plates, rnd ) );

//		/* 4️⃣  Node pass ------------------------------------------------------*/
//		ConcurrentBag<Exception> errs = new();
//		globe.ParallelNodes( ( n, idx ) => {
//			float baseIso = Isostasy.Elev( n.CrustThicknessKm, n.CrustDensity );
//			float elev = baseIso
//						  + dynTopo.Sample( n.Vector )
//						  + featField.Sample( n.Face.Index );

//			// existing ridge/fault logic from your original code ------------
//			elev += globe.FaultHeight( idx ) + globe.MountainHeight( idx );
//			n.Height = elev;
//		}, errs );

//		if (!errs.IsEmpty)
//			throw errs.First();
//	}
//}

//record struct Edge( Vector3<float>[] Points, uint Seed );
//sealed class TectonicPlate {
//	public required int Id;
//	public required Vector3<float> Position;
//	public List<Face> Faces { get; } = new();
//}
//sealed class Face {
//	public required int Index;
//	public required Vector3<float> Center;
//	public bool IsContinental;
//	public bool IsOceanic;
//	public float CrustAgeGa;
//	public float CrustThicknessKm;
//	public float CrustDensity;
//}
//sealed class Node {
//	public required Vector3<float> Vector;
//	public required Face Face;
//	public float Height;
//	public float CrustThicknessKm;
//	public float CrustDensity;
//}
//sealed class Globe {
//	public IReadOnlyList<Vector3<float>> AllVertices { get; init; } = [];
//	public IReadOnlyList<Edge> PlateEdges { get; init; } = [];
//	public void AssignFacesToPlates( List<TectonicPlate> p ) { }
//	public void ParallelNodes( Action<Node, int> work, ConcurrentBag<Exception> errs ) { }
//	public float FaultHeight( int idx ) => 0f;   // stub
//	public float MountainHeight( int idx ) => 0f;   // stub
//}
//sealed class Noise3 {
//	readonly int _seed; readonly float _freq;
//	public Noise3( int s, float f ) { _seed = s; _freq = f; }
//	public float Noise( Vector3<float> p )
//		=> (float) Math.Sin( (p.X + p.Y + p.Z + _seed) * _freq );
//	public static float Noise( uint seed, float freq, float t )
//		=> (float) Math.Sin( (t + seed) * freq );
//}
//sealed class SeedProvider { public int Next() => Random.Shared.Next(); }
//sealed class TectonicGenerationParameters {
//	public required SeedProvider SeedProvider;
//}