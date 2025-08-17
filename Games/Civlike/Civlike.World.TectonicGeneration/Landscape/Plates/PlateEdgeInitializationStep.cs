using Civlike.World.State;
using Civlike.World.TectonicGeneration.Landscape.States;
using Civlike.World.TectonicGeneration.NoiseProviders;
using Engine;
using System.Collections.Concurrent;
using System.Collections.Frozen;

namespace Civlike.World.TectonicGeneration.Landscape.Plates;

[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<PlatePropertySetupStep>]
[Engine.Processing.Do<IGlobeGenerationProcessingStep>.After<PlateEdgeDetectionStep>]
public sealed class PlateEdgeInitializationStep( TectonicGenerationParameters parameters ) : TectonicGlobeGenerationProcessingStepBase( parameters ) {
	private readonly struct Average( float sum, int count ) {
		public readonly float Sum = sum;
		public readonly int Count = count;

		public Average Add( float v ) => new( Sum + v, Count + 1 );
		public static Average Merge( Average a, Average b ) => new( a.Sum + b.Sum, a.Count + b.Count );
		public float Mean => Count > 0 ? Sum / Count : 0f;
	}

	private readonly record struct Partials( Dictionary<Node, Average> RidgeHalfRates, HashSet<Node> RidgeSeeds, HashSet<Node> ConvergentSeeds, Dictionary<Node, float> TransformSlipMax );

	public override void Process( Globe globe ) {
		List<SphericalVoronoiRegion> regions = globe.GetStateOrThrow<GlobeTectonicPlateState>().Regions;
		FrozenDictionary<SphericalVoronoiRegion, int> regionIds = regions
			.Select( ( r, i ) => (r, i) )
			.ToDictionary( t => t.r, t => t.i )
			.ToFrozenDictionary();

		float radiusKm = (float) globe.RadiusKm;
		ConcurrentBag<Partials> partials = [];

		ParallelProcessing.Range( regions.Count, ( start, end, _ ) => {
			Dictionary<Node, Average> ridgeHalf = [];
			HashSet<Node> ridgeSeeds = [];
			HashSet<Node> convSeeds = [];
			Dictionary<Node, float> transformSlipMax = [];
			HashSet<ulong> seenEdge = [];

			for (int i = start; i < end; i++) {
				SphericalVoronoiRegion region = regions[ i ];
				SphericalVoronoiRegionTectonicPlateState aState = region.GetStateOrThrow<SphericalVoronoiRegionTectonicPlateState>();
				Vector3<float> omegaA = aState.AngularVelocity; // rad/Ma
				int idA = regionIds[ region ];

				foreach (PlateEdge edge in aState.Edges.Values) {
					if (regionIds[ edge.Neighbour.StateContainer ] <= idA)
						continue;

					Vector3<float> omegaB = edge.Neighbour.AngularVelocity; // rad/Ma

					foreach (Node n in edge.Nodes) 						foreach (Node m in n.NeighbouringNodes) {
							NodeTectonicLandscapeState mState = m.GetStateOrThrow<NodeTectonicLandscapeState>();
							if (mState.Region != edge.Neighbour.StateContainer)
								continue;

							if (!seenEdge.Add( EdgeKey( n, m ) ))
								continue;

							// Build tangent frame at midpoint of the segment (n -> m)
							//(Vector3<float> rMid, Vector3<float> acrossHat, Vector3<float> alongHat) = BuildTangentFrame( n.Vertex.Vector, m.Vertex.Vector );

							// Relative velocity at surface (km/Ma): v_rel = R * (Δω × r̂)
							//Vector3<float> vRel = radiusKm * (omegaB - omegaA).Cross( rMid );
							//float vAcross = vRel.Dot( acrossHat );  // normal to boundary (± divergence/convergence)
							//float vAlong = vRel.Dot( alongHat );   // along boundary (transform slip)

							// --- classify with one-step majority smoothing ---
							(BoundaryType type, float vAcross, float vAlong) = MajorityFilterPair( n, m, region, edge.Neighbour.StateContainer, omegaA, omegaB, radiusKm );

							// then your existing switch:
							switch (type) {
								case BoundaryType.Transform:
									UpsertMax( transformSlipMax, n, MathF.Abs( vAlong ) );
									UpsertMax( transformSlipMax, m, MathF.Abs( vAlong ) );
									break;

								case BoundaryType.Convergent:
									convSeeds.Add( n );
									convSeeds.Add( m );
									// (optional) store convergence magnitude if you want:
									// UpsertMax(convergenceMax, n, MathF.Abs(vAcross));
									// UpsertMax(convergenceMax, m, MathF.Abs(vAcross));
									break;

								case BoundaryType.Divergent:
									ridgeSeeds.Add( n );
									ridgeSeeds.Add( m );
									float half = 0.5f * MathF.Abs( vAcross );
									Accumulate( ridgeHalf, n, half );
									Accumulate( ridgeHalf, m, half );
									break;
							}
						}
				}
			}

			partials.Add( new Partials( ridgeHalf, ridgeSeeds, convSeeds, transformSlipMax ) );
		} );

		Dictionary<Node, Average> mergedHalf = new( globe.Nodes.Count );
		HashSet<Node> ridgeSeedsAll = [];
		HashSet<Node> convSeedsAll = [];
		Dictionary<Node, float> slipMaxAll = [];

		foreach (Partials p in partials) {
			MergeAvgInto( mergedHalf, p.RidgeHalfRates );
			ridgeSeedsAll.UnionWith( p.RidgeSeeds );
			convSeedsAll.UnionWith( p.ConvergentSeeds );
			MergeMaxInto( slipMaxAll, p.TransformSlipMax );
		}

		foreach (Node n in ridgeSeedsAll)
			n.GetStateOrThrow<NodeTectonicLandscapeState>().IsRidgeSeed = true;

		foreach (Node n in convSeedsAll)
			n.GetStateOrThrow<NodeTectonicLandscapeState>().IsConvergentSeed = true;

		foreach ((Node n, float slip) in slipMaxAll)
			n.GetStateOrThrow<NodeTectonicLandscapeState>().TransformSlipKmPerMa = slip;

		foreach ((Node n, Average avg) in mergedHalf)
			n.GetStateOrThrow<NodeTectonicLandscapeState>().RidgeHalfRateKmPerMa = avg.Mean;
	}

	//private static (Vector3<float> rMid, Vector3<float> acrossHat, Vector3<float> alongHat) BuildTangentFrame( in Vector3<float> a, in Vector3<float> b ) {
	//	Vector3<float> rMid = (a + b).Normalize<Vector3<float>, float>();
	//	Vector3<float> chord = b - a;
	//	Vector3<float> acrossHat = (chord - chord.Dot( rMid ) * rMid).Normalize<Vector3<float>, float>();
	//	Vector3<float> alongHat = rMid.Cross( acrossHat ).Normalize<Vector3<float>, float>();
	//	return (rMid, acrossHat, alongHat);
	//}

	//private static BoundaryType ClassifyBoundary( float vAcross, float vRelMag ) {
	//	// full = |vAcross|; tol guards against numeric jitter: max(0.5 km/Ma, 5% of |v_rel|)
	//	float full = MathF.Abs( vAcross );
	//	float tol = MathF.Max( 0.15f, 0.09f * vRelMag );
	//	if (full <= tol)
	//		return BoundaryType.Transform;
	//	return vAcross > 0 ? BoundaryType.Divergent : BoundaryType.Convergent;
	//}

	private static void Accumulate( Dictionary<Node, Average> dict, Node node, float value )
		=> dict[ node ] = dict.TryGetValue( node, out Average acc ) ? acc.Add( value ) : new Average( value, 1 );

	private static void UpsertMax( Dictionary<Node, float> dict, Node node, float value ) {
		if (value <= 0f)
			return;
		if (!dict.TryGetValue( node, out float cur ) || value > cur)
			dict[ node ] = value;
	}

	private static void MergeAvgInto( Dictionary<Node, Average> target, Dictionary<Node, Average> src ) {
		foreach ((Node k, Average v) in src)
			target[ k ] = target.TryGetValue( k, out Average cur ) ? Average.Merge( cur, v ) : v;
	}

	private static void MergeMaxInto( Dictionary<Node, float> target, Dictionary<Node, float> src ) {
		foreach ((Node k, float v) in src)
			if (!target.TryGetValue( k, out float cur ) || v > cur)
				target[ k ] = v;
	}

	private static ulong EdgeKey( Node a, Node b ) {
		uint aId = (uint) a.Vertex.Id, bId = (uint) b.Vertex.Id;
		return aId < bId
			? (ulong) aId << 32 | bId
			: (ulong) bId << 32 | aId;
	}

	public enum BoundaryType { Convergent, Divergent, Transform }

	// ---- tunables for classification + majority smoothing ----
	const float TolAbsKmPerMa = 0.10f;  // absolute deadband
	const float TolFracOfRel = 0.07f; // relative deadband (% of |v_rel|)
	const float FlipSlackFactor = 1.5f;  // how near tol a label must be to allow flipping

	// Compute axes, v_rel, and classify a single A(nodeA)->B(nodeB) pair
	private static (BoundaryType type, float vAcross, float vAlong, float vRelMag) ClassifyPair( Node nodeA, Node nodeB, Vector3<float> omegaA, Vector3<float> omegaB, float radiusKm ) {
		Vector3<float> a = nodeA.Vertex.Vector;                           // unit
		Vector3<float> b = nodeB.Vertex.Vector;                           // unit
		Vector3<float> rHat = (a + b).Normalize<Vector3<float>, float>(); // midpoint radial

		Vector3<float> chord = b - a;
		Vector3<float> acrossHat = (chord - chord.Dot( rHat ) * rHat).Normalize<Vector3<float>, float>(); // A->B (across)
		Vector3<float> alongHat = rHat.Cross( acrossHat ).Normalize<Vector3<float>, float>();          // along

		Vector3<float> vRel = radiusKm * (omegaB - omegaA).Cross( rHat ); // km/Ma
		float vRelM = vRel.Magnitude<Vector3<float>, float>();
		float vA = vRel.Dot( acrossHat );
		float vT = vRel.Dot( alongHat );

		float full = MathF.Abs( vA );
		float tol = MathF.Max( TolAbsKmPerMa, TolFracOfRel * vRelM );
		BoundaryType type = full <= tol ? BoundaryType.Transform : vA > 0f ? BoundaryType.Divergent : BoundaryType.Convergent;

		return (type, vA, vT, vRelM);
	}

	// One-step majority filter around the pair (n in A, m in B).
	// Votes from cross-plate neighbors that share n or m.
	// Flips only if majority disagrees AND current label is near tolerance (or Transform).
	private static (BoundaryType type, float vAcross, float vAlong) MajorityFilterPair( Node n, Node m, SphericalVoronoiRegion plateA, SphericalVoronoiRegion plateB, Vector3<float> omegaA, Vector3<float> omegaB, float radiusKm ) {
		(BoundaryType type, float vA, float vT, float vRelM) = ClassifyPair( n, m, omegaA, omegaB, radiusKm );

		int votesD = 0, votesC = 0, votesT = 0;
		void Vote( BoundaryType t ) { if (t == BoundaryType.Divergent) votesD++; else if (t == BoundaryType.Convergent) votesC++; else votesT++; }

		// vote current pair
		Vote( type );

		// neighbors that share n (A->B pairs: n with each B-neigh except m)
		foreach (Node nb in n.NeighbouringNodes) {
			NodeTectonicLandscapeState s = nb.GetStateOrThrow<NodeTectonicLandscapeState>();
			if (!ReferenceEquals( s.Region, plateB ) || ReferenceEquals( nb, m ))
				continue;
			(BoundaryType t, float _, float _, float _) = ClassifyPair( n, nb, omegaA, omegaB, radiusKm );
			Vote( t );
		}

		// neighbors that share m (A->B pairs: each A-neigh with m)
		foreach (Node na in m.NeighbouringNodes) {
			NodeTectonicLandscapeState s = na.GetStateOrThrow<NodeTectonicLandscapeState>();
			if (!ReferenceEquals( s.Region, plateA ) || ReferenceEquals( na, n ))
				continue;
			(BoundaryType t, float _, float _, float _) = ClassifyPair( na, m, omegaA, omegaB, radiusKm );
			Vote( t );
		}

		// majority choice
		BoundaryType majority =
			votesD >= votesC && votesD >= votesT ? BoundaryType.Divergent :
			votesC >= votesD && votesC >= votesT ? BoundaryType.Convergent :
													 BoundaryType.Transform;

		if (majority != type) {
			float tol = MathF.Max( TolAbsKmPerMa, TolFracOfRel * vRelM );
			bool weak = type == BoundaryType.Transform || MathF.Abs( vA ) <= FlipSlackFactor * tol;
			if (weak)
				type = majority; // flip only if current label is weak/near tol
		}

		return (type, vA, vT);
	}
}
