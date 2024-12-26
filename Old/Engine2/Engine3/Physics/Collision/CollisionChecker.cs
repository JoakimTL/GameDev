using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using Engine.Physics.Collision.D2;
using Engine.Physics.Collision.D3;
using Engine.Physics.D2;
using Engine.Physics.D2.Shapes;
using Engine.Physics.D3;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Physics.Collision {
	public static class CollisionChecker {

		//Implement 2d and 3d
		public static void CheckCollision( PhysicsModel<Transform3, Vector3> model1, PhysicsModel<Transform3, Vector3> model2, CollisionResult<Vector3> result, bool epa, int iterations = 60 ) {
			result.Clear();
			if( !CheckAABBCollision( model1, model2 ) )
				return;
			foreach( PhysicsShape<Transform3, Vector3> shapeA in model1.Shapes ) {
				shapeA.GetAABB( out Vector3 minA, out Vector3 maxA );
				foreach( PhysicsShape<Transform3, Vector3> shapeB in model2.Shapes ) {
					int uniquePoints = shapeA.UniquePoints * shapeB.UniquePoints;
					if( uniquePoints < 0 || uniquePoints >= 4 ) {
						//We have enough unique points in this shape to create a tetrahedron. There shouldn't be any intersection if there is no tetrahedron.
						shapeB.GetAABB( out Vector3 minB, out Vector3 maxB );
						if( CheckAABBOverlap( minA, maxA, minB, maxB ) ) {
							//AABB overlaps, let's run GJK then EPA if it requested.
							int it = 0;
							Hull3 hull = new Hull3();
							bool collision = false;
							while( it < iterations ) {
								it++;
								int r = GJK3.PerformStep( hull, shapeA, shapeB );
								if( r != 0 ) {
									collision = r == 1;
									if( !result.Colliding )
										result.SetCollisionState( collision );
									break;
								}
							}

							if( collision && epa ) {
								Hull3Faces faces = null;
								CollisionResult<Vector3>.CollisionData data = new CollisionResult<Vector3>.CollisionData();
								while( it < iterations ) {
									it++;
									if( EPA3.PerformStep( hull, ref faces, shapeA, shapeB, ref data ) )
										break;
								}
								result.Add( data );
							}
						}
					}
				}
			}
		}

		public static void CheckCollisionLogging( PhysicsModel<Transform3, Vector3> model1, PhysicsModel<Transform3, Vector3> model2, CollisionResult<Vector3> result, bool epa, int iterations = 60 ) {
			result.Clear();
			if( model1.ShapeCount == 0 || model2.ShapeCount == 0 )
				return;
			model1.UpdateShapes();
			model2.UpdateShapes();
			model1.GetAABB( out Vector3 minmA, out Vector3 maxmA );
			model2.GetAABB( out Vector3 minmB, out Vector3 maxmB );
			if( minmA.X <= maxmB.X && maxmA.X >= minmB.X &&
				minmA.Y <= maxmB.Y && maxmA.Y >= minmB.Y &&
				minmA.Z <= maxmB.Z && maxmA.Z >= minmB.Z ) {
				foreach( PhysicsShape<Transform3, Vector3> shapeA in model1.Shapes ) {
					shapeA.GetAABB( out Vector3 minA, out Vector3 maxA );
					foreach( PhysicsShape<Transform3, Vector3> shapeB in model2.Shapes ) {
						int uniquePoints = shapeA.UniquePoints * shapeB.UniquePoints;
						if( uniquePoints < 0 || uniquePoints >= 4 ) {
							//We have enough unique points in this shape to create a tetrahedron. There shouldn't be any intersection if there is no tetrahedron.
							shapeB.GetAABB( out Vector3 minB, out Vector3 maxB );
							if( CheckAABBOverlap( minA, maxA, minB, maxB ) ) {
								//AABB overlaps, let's run GJK then EPA if it requested.
								int it = 0;
								Hull3 hull = new Hull3();
								bool collision = false;
								while( it < iterations ) {
									it++;
									int r = GJK3.PerformStep( hull, shapeA, shapeB );
									if( r != 0 ) {
										collision = r == 1;
										if( !result.Colliding )
											result.SetCollisionState( collision );
										break;
									}
								}

								if( collision && epa ) {
									Hull3Faces faces = null;
									CollisionResult<Vector3>.CollisionData data = new CollisionResult<Vector3>.CollisionData();
									while( it < iterations ) {
										it++;
										if( EPA3.PerformStep( hull, ref faces, shapeA, shapeB, ref data ) )
											break;
									}
									result.Add( data );
								}
							}
						}
					}
				}
			}
		}

		public static void CheckCollision( PhysicsModel<Transform2, Vector2> model1, PhysicsModel<Transform2, Vector2> model2, CollisionResult<Vector2> result, bool epa, int iterations = 60 ) {
			result.Clear();
			if( !CheckAABBCollision( model1, model2 ) )
				return;
			foreach( PhysicsShape<Transform2, Vector2> shapeA in model1.Shapes ) {
				shapeA.GetAABB( out Vector2 minA, out Vector2 maxA );
				foreach( PhysicsShape<Transform2, Vector2> shapeB in model2.Shapes ) {
					int uniquePoints = shapeA.UniquePoints * shapeB.UniquePoints;
					if( uniquePoints < 0 || uniquePoints >= 3 ) { //Triangle
						shapeB.GetAABB( out Vector2 minB, out Vector2 maxB );
						if( CheckAABBOverlap( minA, maxA, minB, maxB ) ) {
							//AABB overlaps, let's run GJK then EPA if it requested.
							int it = 0;
							Hull2 hull = new Hull2();
							bool collision = false;
							while( it < iterations ) {
								it++;
								int r = GJK2.PerformStep( hull, shapeA, shapeB );
								if( r != 0 ) {
									collision = r == 1;
									if( !result.Colliding )
										result.SetCollisionState( collision );
									break;
								}
							}

							if( collision && epa ) {
								CollisionResult<Vector2>.CollisionData data = new CollisionResult<Vector2>.CollisionData();
								while( it < iterations ) {
									it++;
									if( EPA2.PerformStep( hull, shapeA, shapeB, ref data ) )
										break;
								}
								result.Add( data );
							}
						}
					}
				}
			}
		}

		public static void CheckCollision( PhysicsShape<Transform3, Vector3> shape, PhysicsModel<Transform3, Vector3> model, CollisionResult<Vector3> result, bool epa, int iterations = 60 ) {
			result.Clear();
			if( !CheckAABBCollision( shape, model ) )
				return;
			shape.GetAABB( out Vector3 minA, out Vector3 maxA );
			foreach( PhysicsShape<Transform3, Vector3> shapeB in model.Shapes ) {
				shapeB.GetAABB( out Vector3 minB, out Vector3 maxB );
				if( CheckAABBOverlap( minA, maxA, minB, maxB ) ) {
					//AABB overlaps, let's run GJK then EPA if it requested.
					int it = 0;
					Hull3 hull = new Hull3();
					bool collision = false;
					while( it < iterations ) {
						it++;
						int r = GJK3.PerformStep( hull, shape, shapeB );
						if( r != 0 ) {
							collision = r == 1;
							if( !result.Colliding )
								result.SetCollisionState( collision );
							break;
						}
					}

					if( collision && epa ) {
						Hull3Faces faces = null;
						CollisionResult<Vector3>.CollisionData data = new CollisionResult<Vector3>.CollisionData();
						while( it < iterations ) {
							it++;
							if( EPA3.PerformStep( hull, ref faces, shape, shapeB, ref data ) )
								break;
						}
						result.Add( data );
					}
				}
			}
		}

		public static void CheckCollision( PhysicsShape<Transform2, Vector2> shape, PhysicsModel<Transform2, Vector2> model, CollisionResult<Vector2> result, bool epa, int iterations = 60 ) {
			result.Clear();
			if( !CheckAABBCollision( shape, model ) )
				return;
			shape.GetAABB( out Vector2 minA, out Vector2 maxA );
			foreach( PhysicsShape<Transform2, Vector2> shapeB in model.Shapes ) {
				shapeB.GetAABB( out Vector2 minB, out Vector2 maxB );
				if( CheckAABBOverlap( minA, maxA, minB, maxB ) ) {
					//AABB overlaps, let's run GJK then EPA if it requested.
					int it = 0;
					Hull2 hull = new Hull2();
					bool collision = false;
					while( it < iterations ) {
						it++;
						int r = GJK2.PerformStep( hull, shape, shapeB );
						if( r != 0 ) {
							collision = r == 1;
							if( !result.Colliding )
								result.SetCollisionState( collision );
							break;
						}
					}

					if( collision && epa ) {
						CollisionResult<Vector2>.CollisionData data = new CollisionResult<Vector2>.CollisionData();
						while( it < iterations ) {
							it++;
							if( EPA2.PerformStep( hull, shape, shapeB, ref data ) )
								break;
						}
						result.Add( data );
					}
				}
			}
		}

		public static void CheckCollision( PhysicsShape<Transform3, Vector3> shapeA, PhysicsShape<Transform3, Vector3> shapeB, CollisionResult<Vector3> result, bool epa, int iterations = 60 ) {
			result.Clear();
			if( !CheckAABBCollision( shapeA, shapeB ) )
				return;
			//AABB overlaps, let's run GJK then EPA if it requested.
			int it = 0;
			Hull3 hull = new Hull3();
			bool collision = false;
			while( it < iterations ) {
				it++;
				int r = GJK3.PerformStep( hull, shapeA, shapeB );
				if( r != 0 ) {
					collision = r == 1;
					if( !result.Colliding )
						result.SetCollisionState( collision );
					break;
				}
			}

			if( collision && epa ) {
				Hull3Faces faces = null;
				CollisionResult<Vector3>.CollisionData data = new CollisionResult<Vector3>.CollisionData();
				while( it < iterations ) {
					it++;
					if( EPA3.PerformStep( hull, ref faces, shapeA, shapeB, ref data ) )
						break;
				}
				result.Add( data );
			}
		}

		public static void CheckCollision( PhysicsShape<Transform2, Vector2> shapeA, PhysicsShape<Transform2, Vector2> shapeB, CollisionResult<Vector2> result, bool epa, int iterations = 60 ) {
			result.Clear();
			if( !CheckAABBCollision( shapeA, shapeB ) )
				return;
			//AABB overlaps, let's run GJK then EPA if it requested.
			int it = 0;
			Hull2 hull = new Hull2();
			bool collision = false;
			while( it < iterations ) {
				it++;
				int r = GJK2.PerformStep( hull, shapeA, shapeB );
				if( r != 0 ) {
					collision = r == 1;
					if( !result.Colliding )
						result.SetCollisionState( collision );
					break;
				}
			}

			if( collision && epa ) {
				CollisionResult<Vector2>.CollisionData data = new CollisionResult<Vector2>.CollisionData();
				while( it < iterations ) {
					it++;
					if( EPA2.PerformStep( hull, shapeA, shapeB, ref data ) )
						break;
				}
				result.Add( data );
			}
		}

		public static bool CheckAABBCollision( PhysicsShape<Transform2, Vector2> shapeA, PhysicsShape<Transform2, Vector2> shapeB ) {
			shapeA.UpdateShape();
			shapeB.UpdateShape();
			shapeA.GetAABB( out Vector2 minA, out Vector2 maxA );
			shapeB.GetAABB( out Vector2 minB, out Vector2 maxB );
			return CheckAABBOverlap( minA, maxA, minB, maxB );
		}

		public static bool CheckAABBCollision( PhysicsShape<Transform3, Vector3> shapeA, PhysicsShape<Transform3, Vector3> shapeB ) {
			shapeA.UpdateShape();
			shapeB.UpdateShape();
			shapeA.GetAABB( out Vector3 minA, out Vector3 maxA );
			shapeB.GetAABB( out Vector3 minB, out Vector3 maxB );
			return CheckAABBOverlap( minA, maxA, minB, maxB );
		}

		public static bool CheckAABBCollision( PhysicsShape<Transform2, Vector2> shape, PhysicsModel<Transform2, Vector2> model ) {
			if( model.ShapeCount == 0 )
				return false;
			shape.UpdateShape();
			model.UpdateShapes();
			shape.GetAABB( out Vector2 minA, out Vector2 maxA );
			model.GetAABB( out Vector2 minB, out Vector2 maxB );
			return CheckAABBOverlap( minA, maxA, minB, maxB );
		}

		public static bool CheckAABBCollision( PhysicsShape<Transform3, Vector3> shape, PhysicsModel<Transform3, Vector3> model ) {
			if( model.ShapeCount == 0 )
				return false;
			shape.UpdateShape();
			model.UpdateShapes();
			shape.GetAABB( out Vector3 minA, out Vector3 maxA );
			model.GetAABB( out Vector3 minB, out Vector3 maxB );
			return CheckAABBOverlap( minA, maxA, minB, maxB );
		}

		public static bool CheckAABBCollision( PhysicsModel<Transform2, Vector2> model1, PhysicsModel<Transform2, Vector2> model2 ) {
			if( model1.ShapeCount == 0 || model2.ShapeCount == 0 )
				return false;
			model1.UpdateShapes();
			model2.UpdateShapes();
			model1.GetAABB( out Vector2 minA, out Vector2 maxA );
			model2.GetAABB( out Vector2 minB, out Vector2 maxB );
			return CheckAABBOverlap( minA, maxA, minB, maxB );
		}

		public static bool CheckAABBCollision( PhysicsModel<Transform3, Vector3> model1, PhysicsModel<Transform3, Vector3> model2 ) {
			if( model1.ShapeCount == 0 || model2.ShapeCount == 0 )
				return false;
			model1.UpdateShapes();
			model2.UpdateShapes();
			model1.GetAABB( out Vector3 minA, out Vector3 maxA );
			model2.GetAABB( out Vector3 minB, out Vector3 maxB );
			return CheckAABBOverlap( minA, maxA, minB, maxB );
		}

		public static bool CheckAABBOverlap( Vector3 minA, Vector3 maxA, Vector3 minB, Vector3 maxB ) {
			return 
				minA.X <= maxB.X && maxA.X >= minB.X &&
				minA.Y <= maxB.Y && maxA.Y >= minB.Y &&
				minA.Z <= maxB.Z && maxA.Z >= minB.Z;
			/*
			minmA.X <= maxmB.X && maxmA.X >= minmB.X &&
			minmA.Y <= maxmB.Y && maxmA.Y >= minmB.Y &&
			minmA.Z <= maxmB.Z && maxmA.Z >= minmB.Z
			*/
		}

		public static bool CheckAABBOverlap( Vector2 minA, Vector2 maxA, Vector2 minB, Vector2 maxB ) {
			return 
				minA.X <= maxB.X && maxA.X >= minB.X && 
				minA.Y <= maxB.Y && maxA.Y >= minB.Y;
		}
	}
}
