//using Engine.LinearAlgebra;
//using System;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading;

//namespace Engine.Physics.Old.D3.Shapes {
//	public class Shape3Polygon : BaseShape<Vector3> {

//		private Mold mold;
//		public int Size { get => mold.Points.Count; }
//		private bool updateNeeded;
//		private readonly List<Vector3> transformedVectors;
//		private Vector3 minAABB, maxAABB;
//		private AutoResetEvent betterLock;

//		public Shape3Polygon( BaseModel<Vector3> model ) : base( model ) {
//			model.Transform.OnAnyChangedEvent += TransformChanged;
//			mold = new Mold();
//			mold.Changed += TransformChanged;
//			transformedVectors = new List<Vector3>();
//			updateNeeded = true;
//			betterLock = new AutoResetEvent( true );
//		}

//		public Shape3Polygon( BaseModel<Vector3> model, Mold mold ) : base( model ) {
//			model.Transform.OnAnyChangedEvent += TransformChanged;
//			this.mold = mold;
//			mold.Changed += TransformChanged;
//			transformedVectors = new List<Vector3>();
//			updateNeeded = true;
//			betterLock = new AutoResetEvent( true );
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
//			betterLock.WaitOne();
//			transformedVectors.Clear();
//			IReadOnlyList<Vector3> moldPoints = mold.Points;
//			for( int i = 0; i < moldPoints.Count; i++ ) {
//				transformedVectors.Add( Vector3.Transform( moldPoints[ i ], model.Transform.Matrix ) );
//			}
//			UpdateAABB();
//			betterLock.Set();
//		}

//		private void UpdateAABB() {
//			minAABB = maxAABB = 0;
//			if( Size == 0 )
//				return;
//			minAABB = maxAABB = transformedVectors[ 0 ];
//			for( int i = 0; i < Size; i++ ) {
//				Vector3 n = transformedVectors[ i ];
//				minAABB.X = Math.Min( minAABB.X, n.X );
//				minAABB.Y = Math.Min( minAABB.Y, n.Y );
//				minAABB.Z = Math.Min( minAABB.Z, n.Z );
//				maxAABB.X = Math.Max( maxAABB.X, n.X );
//				maxAABB.Y = Math.Max( maxAABB.Y, n.Y );
//				maxAABB.Z = Math.Max( maxAABB.Z, n.Z );
//			}
//		}

//		public override void GetAABB( out Vector3 min, out Vector3 max ) {
//			min = minAABB;
//			max = maxAABB;
//		}

//		public override Vector3 GetFurthest( Vector3 dir ) {
//			if( Size == 0 )
//				return 0;
//			lock( transformedVectors ) {
//				Vector3 furthest = transformedVectors[ 0 ];
//				float dot = Vector3.Dot( dir, furthest );
//				for( int i = 1; i < Size; i++ ) {
//					Vector3 tP = transformedVectors[ i ];
//					float tDot = Vector3.Dot( dir, tP );
//					if( tDot > dot ) {
//						furthest = tP;
//						dot = tDot;
//					}
//				}
//				return furthest;
//			}
//		}

//		public override void GetClosest( Vector3 target, out Vector3 closest, out Vector3 nextClosest ) {
//			closest = nextClosest = 0;
//			if( Size == 0 )
//				return;
//			closest = nextClosest = transformedVectors[ 0 ];
//			float d = ( closest - target ).LengthSquared;
//			for( int i = 1; i < Size; i++ ) {
//				Vector3 vec = transformedVectors[ i ];
//				float d2 = ( vec - target ).LengthSquared;
//				if( d2 < d ) {
//					nextClosest = closest;
//					closest = vec;
//					d = d2;
//				}
//			}
//		}

//		public Vector3 GetTransformedPoint( int i ) {
//			return transformedVectors[ i ];
//		}

//		public override Vector3 GetInitial() {
//			return transformedVectors[ 0 ];
//		}

//		internal override void Dispose() {
//			mold.Changed -= TransformChanged;
//			mold = null;
//		}

//		public class Mold {

//			private List<Vector3> points;
//			public IReadOnlyList<Vector3> Points { get => points; }
//			public bool Locked { get; private set; }

//			public event Action Changed;

//			public Mold( IReadOnlyList<Vector3> points ) {
//				this.points = new List<Vector3>();
//				this.points.AddRange( points );
//				Locked = true;
//			}

//			public Mold() {
//				this.points = new List<Vector3>();
//				Locked = false;
//			}

//			public void Add( Vector3 t ) {
//				if( Locked )
//					return;
//				points.Add( t );
//				Changed?.Invoke();
//			}

//			public void Set( Vector3[] ts ) {
//				if( Locked )
//					return;
//				points.Clear();
//				points.AddRange( ts );
//				Changed?.Invoke();
//			}

//			public void Set( IReadOnlyList<Vector3> ts ) {
//				if( Locked )
//					return;
//				points.Clear();
//				points.AddRange( ts );
//				Changed?.Invoke();
//			}

//			public Shape3Polygon MoldNew( BaseModel<Vector3> model ) {
//				return new Shape3Polygon( model, this );
//			}

//			public IReadOnlyList<Vector3> CreateTransformed( Matrix4 transformation ) {
//				List<Vector3> ret = new List<Vector3>();
//				for( int i = 0; i < points.Count; i++ )
//					ret.Add( Vector3.Transform( points[ i ], transformation ) );
//				return ret;
//			}

//		}
//	}
//}
