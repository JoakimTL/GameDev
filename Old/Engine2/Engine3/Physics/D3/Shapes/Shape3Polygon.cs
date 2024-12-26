using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Engine.Physics.D3.Shapes {
	public class Shape3Polygon : PhysicsShape<Transform3, Vector3> {

		public Mold Shape { get; private set; }
		public int Size { get => Shape.Points.Count; }
		internal override int UniquePoints { get => Shape.Points.Count; }

		private Transform3 transform;
		public TransformInterface<Vector3, Quaternion, Vector3> Transform => transform.Interface;

		private Vector3 com;
		public override Vector3 CenterOfMass { get => com; }

		private volatile bool updateNeeded;
		private bool facesUpdated;
		private readonly List<PointMass<Vector3>> transformedVectors;
		private readonly List<(int, int, int)> faces;
		private Vector3 minAABB, maxAABB;

		public Shape3Polygon( float massProportion, Mold mold ) : base( massProportion ) {
			transform = new Transform3();
			this.Shape = mold;
			transformedVectors = new List<PointMass<Vector3>>();
			updateNeeded = true;
			facesUpdated = true;
			faces = new List<(int, int, int)>();

			transform.OnChangedEvent += TransformChanged;
			mold.Changed += MoldChanged;
		}

		public Shape3Polygon( float massProportion ) : this( massProportion, new Mold() ) { }

		internal override void Added( PhysicsModel<Transform3, Vector3> model ) {
			transform.SetParent( model.Transform );
		}

		internal override void Removed( PhysicsModel<Transform3, Vector3> model ) {
			transform.SetParent( null );
		}

		private void MoldChanged() {
			updateNeeded = true;
			facesUpdated = true;
		}

		private void TransformChanged() {
			updateNeeded = true;
		}

		private void UpdateFaces() {
			facesUpdated = false;
			faces.Clear();
			(int, int) edges;
			//for( int i = 0; i < length; i++ ) {

			//}
		}

		internal override bool UpdateShape() {
			if( facesUpdated )
				UpdateFaces();
			if( !updateNeeded )
				return false;
			updateNeeded = false;

			lock( transformedVectors ) {
				transformedVectors.Clear();
				Shape.InsertTransformed( transformedVectors, Transform.Matrix );
			}
			com = GetCenterOfMass();
			UpdateAABB();

			return true;
		}

		private Vector3 GetCenterOfMass() {
			Vector3 accumulatedOffset = new Vector3();
			float totalMass = 0;
			lock( transformedVectors ) {
				foreach( PointMass<Vector3> p in transformedVectors ) {
					accumulatedOffset += p.Offset * p.MassProportion;
					totalMass += p.MassProportion;
				}
			}
			if( totalMass == 0 )
				return accumulatedOffset;
			return accumulatedOffset / totalMass;
		}

		private void UpdateAABB() {
			minAABB = maxAABB = 0;
			if( Size == 0 )
				return;
			lock( transformedVectors ) {
				minAABB = maxAABB = transformedVectors[ 0 ].Offset;
				for( int i = 1; i < Size; i++ ) {
					Vector3 n = transformedVectors[ i ].Offset;
					minAABB = Vector3.Min( minAABB, n );
					maxAABB = Vector3.Max( maxAABB, n );
				}
			}
		}

		public override void GetAABB( out Vector3 min, out Vector3 max ) {
			min = minAABB;
			max = maxAABB;
		}

		public override Vector3 GetFurthest( Vector3 dir ) {
			if( Size == 0 )
				return 0;
			lock( transformedVectors ) {
				Vector3 furthest = transformedVectors[ 0 ].Offset;
				float dot = Vector3.Dot( dir, furthest );
				for( int i = 1; i < Size; i++ ) {
					Vector3 tP = transformedVectors[ i ].Offset;
					float tDot = Vector3.Dot( dir, tP );
					if( tDot > dot ) {
						furthest = tP;
						dot = tDot;
					}
				}
				return furthest;
			}
		}

		/*public override void GetClosest( Vector3 target, out Vector3 closest, out Vector3 nextClosest ) {
			closest = nextClosest = 0;
			if( Size == 0 )
				return;
			closest = nextClosest = transformedVectors[ 0 ].Offset;
			float d = ( closest - target ).LengthSquared;
			for( int i = 1; i < Size; i++ ) {
				Vector3 vec = transformedVectors[ i ].Offset;
				float d2 = ( vec - target ).LengthSquared;
				if( d2 < d ) {
					nextClosest = closest;
					closest = vec;
					d = d2;
				}
			}
		}*/

		public Vector3 GetTransformedPoint( int i ) {
			lock( transformedVectors ) {
				return transformedVectors[ i ].Offset;
			}
		}

		internal override Vector3 GetUniquePoint( int id ) {
			lock( transformedVectors ) {
				if( id < transformedVectors.Count )
					return transformedVectors[ id ].Offset;
				return GetInitial();
			}
		}

		public override Vector3 GetInitial() {
			lock( transformedVectors ) {
				return transformedVectors[ 0 ].Offset;
			}
		}

		public override float GetInertia( Vector3 dir, Vector3 centerOfMass ) {
			float accumulated = 0;
			//Console.WriteLine( "len:" + transformedVectors.Count );
			lock( transformedVectors ) {
				foreach( PointMass<Vector3> pm in transformedVectors ) {
					//Console.WriteLine( "pm[" + pm.Offset + "]:" + ( pm.Offset - centerOfMass ) + ", " + dir + ": " + Vector3.Cross( pm.Offset - centerOfMass, dir ) + "::" + Vector3.Cross( pm.Offset - centerOfMass, dir ).LengthSquared );
					accumulated += Vector3.Cross( pm.Offset - centerOfMass, dir ).LengthSquared;
				}
				return accumulated / transformedVectors.Count;
			}
		}

		public override string ToString() {
			string s = "";
			lock( transformedVectors ) {
				for( int i = 0; i < transformedVectors.Count; i++ ) {
					s += transformedVectors[ i ].Offset.ToString();
					if( i < transformedVectors.Count - 1 )
						s += ", ";
				}
			}
			return "shape3:["+transform.Matrix.ToString()+"][" + s + "]";
		}

		public class Mold {

			private readonly List<PointMass<Vector3>> points;
			public IReadOnlyList<PointMass<Vector3>> Points { get => points; }
			public bool Locked { get; private set; }

			public event Action Changed;

			/// <summary>
			/// Creates a mold with uniform mass
			/// </summary>
			/// <param name="points">The points in the shape with uniform mass proportions</param>
			public Mold( IReadOnlyList<Vector3> points ) {
				this.points = new List<PointMass<Vector3>>();
				for( int i = 0; i < points.Count; i++ )
					this.points.Add( new PointMass<Vector3>( points[ i ], 1 ) );
				Locked = true;
			}

			/// <summary>
			/// Creates a mold which allows mass propotions to be changed
			/// </summary>
			/// <param name="points">The pointmasses in the shape</param>
			public Mold( IReadOnlyList<PointMass<Vector3>> points ) {
				this.points = new List<PointMass<Vector3>>();
				this.points.AddRange( points );
				Locked = true;
			}

			public Mold() {
				this.points = new List<PointMass<Vector3>>();
				Locked = false;
			}

			public void Add( Vector3 t, float massProportion = 1 ) {
				if( Locked )
					return;
				points.Add( new PointMass<Vector3>( t, massProportion ) );
				Changed?.Invoke();
			}

			public void Add( PointMass<Vector3> t ) {
				if( Locked )
					return;
				points.Add( t );
				Changed?.Invoke();
			}

			public void Set( PointMass<Vector3>[] ts ) {
				if( Locked )
					return;
				points.Clear();
				points.AddRange( ts );
				Changed?.Invoke();
			}

			public void Set( IReadOnlyList<PointMass<Vector3>> ts ) {
				if( Locked )
					return;
				points.Clear();
				points.AddRange( ts );
				Changed?.Invoke();
			}

			public void InsertTransformed( List<Vector3> vecs, Matrix4 transformation ) {
				for( int i = 0; i < points.Count; i++ )
					vecs.Add( Vector3.Transform( points[ i ].Offset, transformation ) );
			}

			public void InsertTransformed( List<PointMass<Vector3>> vecs, Matrix4 transformation ) {
				for( int i = 0; i < points.Count; i++ )
					vecs.Add( new PointMass<Vector3>( Vector3.Transform( points[ i ].Offset, transformation ), points[ i ].MassProportion ) );
			}

			public Shape3Polygon MoldNew( Physics3Model model, float massProportion = 1 ) {
				Shape3Polygon s = new Shape3Polygon( massProportion, this );
				model.Add( s );
				return s;
			}

		}
	}
}
