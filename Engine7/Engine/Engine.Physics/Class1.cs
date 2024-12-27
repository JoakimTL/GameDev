using Engine.Logging;
using Engine.Shapes;
using Engine.Transforms;
using System.ComponentModel;
using System.Numerics;

namespace Engine.Physics;

public class Class1 {

}

/// <summary>
/// Used when calculating a GJK collision.
/// </summary>
public class GJKConvexShape<TVector, TScalar>
	where TVector : unmanaged, ITransformableVector<Matrix4x4<TScalar>, TVector>, IInnerProduct<TVector, TScalar>
	where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {

	private readonly List<TVector> _baseVertices = [];
	private IMatrixProvider<TScalar>? _transform;
	private readonly List<TVector> _transformedVertices = [];
	private bool _hasChanged;

	public GJKConvexShape( ReadOnlySpan<TVector> vertices ) {
		_baseVertices.AddRange( vertices );
		_transformedVertices.AddRange( _baseVertices );
	}

	public GJKConvexShape( IEnumerable<TVector> vertices ) {
		_baseVertices.AddRange( vertices );
		_transformedVertices.AddRange( _baseVertices );
	}

	public void SetBaseVertices( ReadOnlySpan<TVector> vertices ) {
		_baseVertices.Clear();
		_baseVertices.AddRange( vertices );
		_hasChanged = true;
	}

	public void SetBaseVertices( IEnumerable<TVector> vertices ) {
		_baseVertices.Clear();
		_baseVertices.AddRange( vertices );
		_hasChanged = true;
	}

	public void SetTransform( IMatrixProvider<TScalar>? transform ) {
		if (_transform is not null)
			_transform.OnMatrixChanged -= OnMatrixChanged;
		_transform = transform;
		_hasChanged = true;
		if (_transform is not null)
			_transform.OnMatrixChanged += OnMatrixChanged;
	}

	private void OnMatrixChanged( IMatrixProvider<TScalar> provider ) => _hasChanged = false;

	public IReadOnlyList<TVector> GetVertices() {
		if (!_hasChanged)
			return _transformedVertices;
		_hasChanged = false;
		_transformedVertices.Clear();
		if (_transform is null) {
			_transformedVertices.AddRange( _baseVertices );
			return _transformedVertices;
		}
		foreach (TVector vector in _baseVertices) {
			TVector? transformed = vector.TransformWorld( _transform.Matrix );
			if (transformed.HasValue)
				_transformedVertices.Add( transformed.Value );
		}
		return _transformedVertices;
	}

	public (int index, TVector vector) GetFurthestInDirection( TVector direction ) {
		IReadOnlyList<TVector> vertices = GetVertices();
		int index = 0;
		TVector furthest = vertices[ 0 ];
		TScalar maxDot = direction.Dot( furthest );
		for (int i = 1; i < vertices.Count; i++) {
			TScalar dot = direction.Dot( vertices[ i ] );
			if (dot > maxDot) {
				maxDot = dot;
				furthest = vertices[ i ];
				index = i;
			}
		}
		return (index, furthest);
	}
}

public sealed class Simplex2<TScalar>()
	where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {

	private readonly MinkowskiDifference<Vector2<TScalar>>[] _minkowskiSums = new MinkowskiDifference<Vector2<TScalar>>[ 3 ];
	private int _count = 0;

	public IReadOnlyList<MinkowskiDifference<Vector2<TScalar>>> MinkowskiSums => _minkowskiSums;
	public int Count => _count;

	public MinkowskiDifference<Vector2<TScalar>> A => _minkowskiSums[ 0 ];
	public MinkowskiDifference<Vector2<TScalar>> B => _minkowskiSums[ 1 ];
	public MinkowskiDifference<Vector2<TScalar>> C => _minkowskiSums[ 2 ];

	internal bool HasIndexPair( int indexA, int indexB ) {
		for (int i = 0; i < _count; i++) {
			MinkowskiDifference<Vector2<TScalar>> minkowskiSum = _minkowskiSums[ i ];
			if (minkowskiSum.IndexShapeA == indexA && minkowskiSum.IndexShapeB == indexB || minkowskiSum.IndexShapeA == indexB && minkowskiSum.IndexShapeB == indexA)
				return true;
		}
		return false;
	}

	internal void Refresh( GJKConvexShape<Vector2<TScalar>, TScalar> shapeA, GJKConvexShape<Vector2<TScalar>, TScalar> shapeB ) {
		for (int i = 0; i < _count; i++) {
			MinkowskiDifference<Vector2<TScalar>> minkowskiSum = _minkowskiSums[ i ];
			_minkowskiSums[ i ] = new MinkowskiDifference<Vector2<TScalar>>( shapeA.GetVertices()[ minkowskiSum.IndexShapeA ] - shapeB.GetVertices()[ minkowskiSum.IndexShapeB ], minkowskiSum.IndexShapeA, minkowskiSum.IndexShapeB );
		}
	}

	internal void Support( int indexA, Vector2<TScalar> a, int indexB, Vector2<TScalar> b ) {
		for (int i = _minkowskiSums.Length - 1; i >= 1; i--)
			_minkowskiSums[ i ] = _minkowskiSums[ i - 1 ];
		_minkowskiSums[ 0 ] = new MinkowskiDifference<Vector2<TScalar>>( a - b, indexA, indexB ); //needs to shift the others. Point A should always be last in the list, but point A is also the first one added.
		_count++;
	}

	internal void Remove( int index ) {
		for (int i = index; i < _minkowskiSums.Length - 1; i++)
			_minkowskiSums[ i ] = _minkowskiSums[ i + 1 ];
		_count--;
	}
}

public readonly struct MinkowskiDifference<TVector>( TVector sum, int indexShapeA, int indexShapeB ) {
	public TVector Sum { get; } = sum;
	public int IndexShapeA { get; } = indexShapeA;
	public int IndexShapeB { get; } = indexShapeB;
}

/// <summary>
/// Keeps the state of the previous calculation.
/// </summary>
/// <typeparam name="TScalar"></typeparam>
public sealed class Collision2Calculation<TScalar>( GJKConvexShape<Vector2<TScalar>, TScalar> shapeA, GJKConvexShape<Vector2<TScalar>, TScalar> shapeB )
	where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {

	public static readonly TScalar Epsilon = TScalar.CreateSaturating( 0.00000190734 );

	private readonly GJKConvexShape<Vector2<TScalar>, TScalar> _shapeA = shapeA;
	private readonly GJKConvexShape<Vector2<TScalar>, TScalar> _shapeB = shapeB;
	private readonly Simplex2<TScalar> _simplex = new();
	public CollisionResult CollisionResult { get; private set; }

	public bool Evaluate( int maxIterations = 25 ) {
		_simplex.Refresh( _shapeA, _shapeB );
		for (int i = 0; i < maxIterations; i++)
			if (PerformStep())
				return true;
		return false;
	}

	/// <returns>True if the collision detection is resolved with the shapes' current configuration. Results can be found in <see cref="CollisionResult"/>.</returns>
	private bool PerformStep() {
		var shapeAVertices = _shapeA.GetVertices();
		var shapeBVertices = _shapeB.GetVertices();

		if (shapeAVertices.Count == 0 || shapeBVertices.Count == 0) {
			CollisionResult = new CollisionResult( false, true );
			return this.LogWarningThenReturn( "Shape missing vertices for collision detection.", true );
		}

		while (_simplex.Count < 3)
			switch (_simplex.Count) {
				case 0: {
						_simplex.Support( 0, shapeAVertices[ 0 ], 0, shapeBVertices[ 0 ] );
						break;
					}
				case 1: {
						if (_simplex.A.Sum == TScalar.Zero) {
							//When building the simplex we hit the origin, so the shapes are colliding or at least touching.
							CollisionResult = new CollisionResult( true, false );
							return true;
						}

						if (!Support( -_simplex.A.Sum )) {
							CollisionResult = new CollisionResult( false, false );
							return false;
						}
						break;
					}
				case 2: {
						Vector2<TScalar> a = _simplex.A.Sum;
						Vector2<TScalar> b = _simplex.B.Sum;

						Vector2<TScalar> ab = b - a;
						Vector2<TScalar> ao = -a;

						Vector2<TScalar> direction = Vector2<TScalar>.Zero;

						if (ab.Dot( ao ) > -Epsilon) {
							Vector2<TScalar> dir = ab * new Bivector2<TScalar>( TScalar.One );
							if (dir.Dot( ao ) < TScalar.Zero)
								dir = -dir;
							direction = dir; // From the line towards the origin. (perpendicular to the line, facing the origin)
						} else {
							direction = ao; // From point A towards the origin.
						}

						if (!Support( direction )) {
							CollisionResult = new CollisionResult( false, false );
							return false;
						}
						break;
					}
			}

		//Now we have a triangle:
		return PerformTriangleTest();
	}

	private bool PerformTriangleTest() {

		//1-2-3-4-5-6-7-8-9-0-1-2-3-4-5-6-7-8-9-0-1-2-3-4-5-6-7-8-9//
		//                       \         /                       //
		//                        \  R 6  /                        //
		//                         \     /                         //
		//                          \   /                          //
		//                           \ /                           //
		//                            ▲                            //
		//                           / \                           //
		//                          / A \                          //
		//                         /     \                         //
		//         Region 5       /#######\       Region 1         //
		//                       /#########\                       //
		//                      /###########\                      //
		//                     /#############\                     //
		//                    /###############\                    //
		//                   /#################\                   //
		//                  /###################\                  //
		//                 /####            #####\                 //
		//                /#####  Region 0  ######\                //
		//               /######            #######\               //
		//              /###########################\              //
		//             /#############################\             //
		//            /###############################\            //
		//           /#################################\           //
		//          /    ###########################    \          //
		//         /  C  ###########################  B  \         //
		//________/_______________________________________\________//
		//       /                                         \       //
		// R 4  /                 Region 3                  \  R 2 //
		//     /                                             \     //
		//    /                                               \    //
		//1-2-3-4-5-6-7-8-9-0-1-2-3-4-5-6-7-8-9-0-1-2-3-4-5-6-7-8-9//

		//If the origin is in Region 1 then Vertex C is removed.
		//If the origin is in Region 2 then Vertex C then Vertex A is removed.
		//If the origin is in Region 3 then Vertex A is removed.
		//If the origin is in Region 4 then Vertex B then Vertex A is removed.
		//If the origin is in Region 5 then Vertex B is removed.
		//If the origin is in Region 6 then Vertex C then Vertex B is removed.
		//Collision occurs if the origin is within Region 0. This is only possible if none of the other cases happen.

		Vector2<TScalar> a = _simplex.A.Sum;
		Vector2<TScalar> b = _simplex.B.Sum;
		Vector2<TScalar> c = _simplex.C.Sum;
		Vector2<TScalar> ab = b - a;
		Vector2<TScalar> bc = c - b;
		Vector2<TScalar> ca = a - c;

		Edge2<TScalar> abEdge = new( a, b );
		int winding = abEdge.Orientation( c );

		if (winding == 0) {
			//Degenerate case that should never happen if any of the two shapes contain a triangle with an area greater than 0.
			CollisionResult = new CollisionResult( false, false );
			return true;
		}

		if (winding > 0) {
			//Swap B and C for the calculation
			Vector2<TScalar> temp = ca;
			ca = bc;
			bc = temp;
		}

		Vector2<TScalar> abPerp = ab * new Bivector2<TScalar>( TScalar.One );
		Vector2<TScalar> caPerp = ca * new Bivector2<TScalar>( TScalar.One );
		Vector2<TScalar> bcPerp = bc * new Bivector2<TScalar>( TScalar.One );

		TScalar abPcDot = abPerp.Dot( c );
		TScalar caPbDot = caPerp.Dot( b );
		TScalar bcPaDot = bcPerp.Dot( a );

		if (abPcDot < -Epsilon) {
			_simplex.Remove( 2 );
		}

		if (caPbDot < -Epsilon) {
			_simplex.Remove( 1 );
		}

		if (bcPaDot < -Epsilon) {
			_simplex.Remove( 0 );
		}

		if (_simplex.Count == 3) {
			//The checks completed and the simplex is still a triangle. The origin is within the triangle.
			CollisionResult = new CollisionResult( true, false );
			return true;
		}
		return false;

		//TODO: UNIT TESTS LOTS OF TESTS FOR WEIRD SHAPES AND SITUATIONS.
	}

	private bool Support( Vector2<TScalar> direction ) {
		(int indexA, Vector2<TScalar> a) = _shapeA.GetFurthestInDirection( direction );
		(int indexB, Vector2<TScalar> b) = _shapeB.GetFurthestInDirection( -direction );
		if (_simplex.HasIndexPair( indexA, indexB ))
			return false;
		_simplex.Support( indexA, a, indexB, b );
		return _simplex.A.Sum.Dot( direction ) >= TScalar.Zero;
	}
}

public readonly struct CollisionResult {
	public bool IsColliding { get; }
	public bool HasError { get; }

	internal CollisionResult( bool isColliding, bool hasError ) {
		IsColliding = isColliding;
		HasError = hasError;
	}
}