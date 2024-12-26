//using Engine.Graphics.Objects;
//using Engine.Graphics.Objects.Default.Transforms;
//using Engine.LinearAlgebra;
//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Text;

//namespace Engine.Physics.Old.D2.Shapes {
//	public class Shape2Polygon : BaseShape<Vector2> {

//		private Mold mold;
//		public int Size { get => mold.Points.Count; }
//		private bool updateNeeded;
//		private readonly List<Vector2> transformedVectors;
//		private Vector2 minAABB, maxAABB;

//		public Shape2Polygon( BaseModel<Vector2> model ) : base( model ) {
//			model.Transform.OnAnyChangedEvent += TransformChanged;
//			mold = new Mold();
//			mold.Changed += TransformChanged;
//			transformedVectors = new List<Vector2>();
//			updateNeeded = true;
//		}

//		public Shape2Polygon( BaseModel<Vector2> model, Mold mold ) : base( model ) {
//			model.Transform.OnAnyChangedEvent += TransformChanged;
//			this.mold = mold;
//			mold.Changed += TransformChanged;
//			transformedVectors = new List<Vector2>();
//			updateNeeded = true;
//		}

//		private void TransformChanged() {
//			updateNeeded = true;
//		}

//		internal override void RemovedFromModel() {
//			model.Transform.OnAnyChangedEvent -= TransformChanged;
//		}

//		internal override void UpdateShape() {
//			if( !updateNeeded )
//				return;
//			updateNeeded = false;
//			transformedVectors.Clear();
//			for( int i = 0; i < mold.Points.Count; i++ ) {
//				transformedVectors.Add( Vector3.Transform( new Vector3( mold.Points[ i ], 0 ), model.Transform.Matrix ).XY );
//			}
//			UpdateAABB();
//		}

//		private void UpdateAABB() {
//			minAABB = maxAABB = 0;
//			if( Size == 0 )
//				return;
//			minAABB = maxAABB = transformedVectors[ 0 ];
//			for( int i = 0; i < Size; i++ ) {
//				Vector2 n = transformedVectors[ i ];
//				minAABB.X = Math.Min( minAABB.X, n.X );
//				minAABB.Y = Math.Min( minAABB.Y, n.Y );
//				maxAABB.X = Math.Max( maxAABB.X, n.X );
//				maxAABB.Y = Math.Max( maxAABB.Y, n.Y );
//			}
//		}

//		public override void GetAABB( out Vector2 min, out Vector2 max ) {
//			min = minAABB;
//			max = maxAABB;
//		}

//		public override Vector2 GetFurthest( Vector2 dir ) {
//			if( Size == 0 )
//				return 0;
//			Vector2 furthest = transformedVectors[ 0 ];
//			float dot = Vector2.Dot( dir, furthest );
//			for( int i = 1; i < Size; i++ ) {
//				Vector2 tP = transformedVectors[ i ];
//				float tDot = Vector2.Dot( dir, tP );
//				if( tDot > dot ) {
//					furthest = tP;
//					dot = tDot;
//				}
//			}
//			return furthest;
//		}

//		public override void GetClosest( Vector2 target, out Vector2 closest, out Vector2 nextClosest ) {
//			closest = nextClosest = 0;
//			if( Size == 0 )
//				return;
//			closest = nextClosest = transformedVectors[ 0 ];
//			float d = ( closest - target ).LengthSquared;
//			for( int i = 1; i < Size; i++ ) {
//				Vector2 vec = transformedVectors[ i ];
//				float d2 = ( vec - target ).LengthSquared;
//				if( d2 < d ) {
//					nextClosest = closest;
//					closest = vec;
//					d = d2;
//				}
//			}
//		}

//		public override Vector2 GetInitial() {
//			return transformedVectors[ 0 ];
//		}

//		public Vector2 GetTransformedPoint( int i ) {
//			return transformedVectors[ i ];
//		}

//		public Vector2 GetPoint( int i ) {
//			return mold.Points[ i ];
//		}

//		internal override void Dispose() {
//			mold.Changed -= TransformChanged;
//			mold = null;
//		}

//		public class Mold {

//			private List<Vector2> points;
//			public IReadOnlyList<Vector2> Points { get => points; }
//			public bool Locked { get; private set; }

//			public event Action Changed;

//			public Mold( IReadOnlyList<Vector2> points ) {
//				this.points = new List<Vector2>();
//				this.points.AddRange( points );
//				Locked = true;
//			}

//			public Mold() {
//				this.points = new List<Vector2>();
//				Locked = false;
//			}

//			public void Add( Vector2 t ) {
//				if( Locked )
//					return;
//				points.Add( t );
//				Changed?.Invoke();
//			}

//			public void Set( Vector2[] ts ) {
//				if( Locked )
//					return;
//				points.Clear();
//				points.AddRange( ts );
//				Changed?.Invoke();
//			}

//			public void Set( IReadOnlyList<Vector2> ts ) {
//				if( Locked )
//					return;
//				points.Clear();
//				points.AddRange( ts );
//				Changed?.Invoke();
//			}

//			public Shape2Polygon MoldNew( BaseModel<Vector2> model ) {
//				return new Shape2Polygon( model, this );
//			}

//		}
//	}
//}
