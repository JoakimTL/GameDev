using Engine.Logging;
using System.Numerics;

namespace Engine.Physics;

/// <summary>
/// Keeps the state of the previous calculation.
/// </summary>
/// <typeparam name="TScalar"></typeparam>
public sealed class Collision2Calculation<TScalar>( ConvexShapeBase<Vector2<TScalar>, TScalar> shapeA, ConvexShapeBase<Vector2<TScalar>, TScalar> shapeB )
	where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {

	public static readonly TScalar Epsilon = TScalar.CreateSaturating( 0.00000190734 );

	private readonly ConvexShapeBase<Vector2<TScalar>, TScalar> _shapeA = shapeA;
	private readonly ConvexShapeBase<Vector2<TScalar>, TScalar> _shapeB = shapeB;
	private readonly Simplex2<TScalar> _simplex = new();
	public CollisionResult CollisionResult { get; private set; }

	public bool Evaluate( int maxIterations = 25 ) {
		//_simplex.Refresh( _shapeA, _shapeB );
		_simplex.Clear();
		for (int i = 0; i < maxIterations; i++)
			if (PerformStep())
				return true;
		return false;
	}

	/// <returns>True if the collision detection is resolved with the shapes' current configuration. Results can be found in <see cref="CollisionResult"/>.</returns>
	private bool PerformStep() {
		IReadOnlyList<Vector2<TScalar>> shapeAVertices = _shapeA.GetVertices();
		IReadOnlyList<Vector2<TScalar>> shapeBVertices = _shapeB.GetVertices();

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
							return true;
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
							return true;
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
		Vector2<TScalar> ac = c - a;

		Vector2<TScalar> abPerp = ab * new Bivector2<TScalar>( TScalar.One );
		Vector2<TScalar> acPerp = ac * new Bivector2<TScalar>( TScalar.One );

		if (abPerp.Dot( -bc ) < TScalar.Zero)
			abPerp = -abPerp;

		if (acPerp.Dot( bc ) < TScalar.Zero)
			acPerp = -acPerp;

		Vector2<TScalar> ao = -a;
		TScalar acPaoDot = acPerp.Dot( ao );
		TScalar acaoDot = ac.Dot( ao );
		TScalar abPaoDot = abPerp.Dot( ao );
		TScalar abaoDot = ab.Dot( ao );

		if (acPaoDot > -Epsilon) {
			if (acaoDot > -Epsilon) {
				_simplex.Remove( 1 );
				return false;
			}
			if (abaoDot > -Epsilon) {
				_simplex.Remove( 2 );
				return false;
			}

			_simplex.Remove( 2 );
			_simplex.Remove( 1 );
			return false;
		}

		if (abPaoDot > -Epsilon) {
			if (abaoDot > -Epsilon) {
				_simplex.Remove( 2 );
				return false;
			}
			_simplex.Remove( 2 );
			_simplex.Remove( 1 );
			return false;
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
