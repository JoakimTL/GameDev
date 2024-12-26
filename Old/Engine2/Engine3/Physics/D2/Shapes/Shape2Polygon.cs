using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace Engine.Physics.D2.Shapes {
	public class Shape2Polygon : PhysicsShape<Transform2, Vector2> {

		public Mold Shape { get; private set; }
		public int Size { get => Shape.Points.Count; }
		internal override int UniquePoints { get => Shape.Points.Count; }

		private Transform2 transform;
		public TransformInterface<Vector2, float, Vector2> Transform => transform.Interface;

		private Vector2 com;
		public override Vector2 CenterOfMass { get => com; }

		private bool updateNeeded;
		private readonly List<PointMass<Vector2>> transformedVectors;
		private Vector2 minAABB, maxAABB;

		public Shape2Polygon( float massProportion, Mold mold ) : base( massProportion ) {
			transform = new Transform2();
			this.Shape = mold;
			transformedVectors = new List<PointMass<Vector2>>();
			updateNeeded = true;

			transform.OnChangedEvent += TransformChanged;
			mold.Changed += TransformChanged;
		}

		public Shape2Polygon( float massProportion ) : this( massProportion, new Mold() ) { }

		internal override void Added( PhysicsModel<Transform2, Vector2> model ) {
			transform.SetParent( model.Transform );
		}

		internal override void Removed( PhysicsModel<Transform2, Vector2> model ) {
			transform.SetParent( null );
		}

		private void TransformChanged() {
			updateNeeded = true;
		}

		internal override bool UpdateShape() {
			if( !updateNeeded )
				return false;
			updateNeeded = false;

			transformedVectors.Clear();
			Shape.InsertTransformed( transformedVectors, Transform.Matrix );
			com = GetCenterOfMass();

			UpdateAABB();
			return true;
		}

		private Vector2 GetCenterOfMass() {
			Vector2 accumulatedOffset = new Vector2();
			float totalMass = 0;
			foreach( PointMass<Vector2> p in transformedVectors ) {
				accumulatedOffset += p.Offset * p.MassProportion;
				totalMass += p.MassProportion;
			}
			return accumulatedOffset / totalMass;
		}

		private void UpdateAABB() {
			minAABB = maxAABB = 0;
			if( Size == 0 )
				return;
			minAABB = maxAABB = transformedVectors[ 0 ].Offset;
			for( int i = 0; i < Size; i++ ) {
				Vector2 n = transformedVectors[ i ].Offset;
				minAABB.X = Math.Min( minAABB.X, n.X );
				minAABB.Y = Math.Min( minAABB.Y, n.Y );
				maxAABB.X = Math.Max( maxAABB.X, n.X );
				maxAABB.Y = Math.Max( maxAABB.Y, n.Y );
			}
		}

		public override void GetAABB( out Vector2 min, out Vector2 max ) {
			min = minAABB;
			max = maxAABB;
		}

		public override Vector2 GetFurthest( Vector2 dir ) {
			if( Size == 0 )
				return 0;
			lock( transformedVectors ) {
				Vector2 furthest = transformedVectors[ 0 ].Offset;
				float dot = Vector2.Dot( dir, furthest );
				for( int i = 1; i < Size; i++ ) {
					Vector2 tP = transformedVectors[ i ].Offset;
					float tDot = Vector2.Dot( dir, tP );
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

		public Vector2 GetTransformedPoint( int i ) {
			return transformedVectors[ i ].Offset;
		}

		internal override Vector2 GetUniquePoint( int id ) {
			lock( transformedVectors ) {
				if( id < transformedVectors.Count )
					return transformedVectors[ id ].Offset;
				return GetInitial();
			}
		}

		public override Vector2 GetInitial() {
			return transformedVectors[ 0 ].Offset;
		}

		public override float GetInertia( Vector2 dir, Vector2 centerOfMass ) {
			float accumulated = 0;
			foreach( PointMass<Vector2> pm in transformedVectors )
				accumulated += ( pm.Offset - centerOfMass ).LengthSquared;
			return accumulated;
		}

		public class Mold {

			private readonly List<PointMass<Vector2>> points;
			public IReadOnlyList<PointMass<Vector2>> Points { get => points; }
			public bool Locked { get; private set; }

			public event Action Changed;

			/// <summary>
			/// Creates a mold with uniform mass
			/// </summary>
			/// <param name="points">The points in the shape with uniform mass proportions</param>
			public Mold( IReadOnlyList<Vector2> points ) {
				this.points = new List<PointMass<Vector2>>();
				for( int i = 0; i < points.Count; i++ )
					this.points.Add( new PointMass<Vector2>( points[ i ], 1 ) );
				Locked = true;
			}

			/// <summary>
			/// Creates a mold which allows mass propotions to be changed
			/// </summary>
			/// <param name="points">The pointmasses in the shape</param>
			public Mold( IReadOnlyList<PointMass<Vector2>> points ) {
				this.points = new List<PointMass<Vector2>>();
				this.points.AddRange( points );
				Locked = true;
			}

			public Mold() {
				this.points = new List<PointMass<Vector2>>();
				Locked = false;
			}

			public void Add( Vector2 t, float massProportion = 1 ) {
				if( Locked )
					return;
				points.Add( new PointMass<Vector2>( t, massProportion ) );
				Changed?.Invoke();
			}

			public void Add( PointMass<Vector2> t ) {
				if( Locked )
					return;
				points.Add( t );
				Changed?.Invoke();
			}

			public void Set( PointMass<Vector2>[] ts ) {
				if( Locked )
					return;
				points.Clear();
				points.AddRange( ts );
				Changed?.Invoke();
			}

			public void Set( IReadOnlyList<PointMass<Vector2>> ts ) {
				if( Locked )
					return;
				points.Clear();
				points.AddRange( ts );
				Changed?.Invoke();
			}

			public void InsertTransformed( List<Vector2> vecs, Matrix4 transformation ) {
				for( int i = 0; i < points.Count; i++ )
					vecs.Add( Vector2.Transform( points[ i ].Offset, transformation ) );
			}

			public void InsertTransformed( List<PointMass<Vector2>> vecs, Matrix4 transformation ) {
				for( int i = 0; i < points.Count; i++ )
					vecs.Add( new PointMass<Vector2>( Vector2.Transform( points[ i ].Offset, transformation ), points[ i ].MassProportion ) );
			}

			public Shape2Polygon MoldNew( Physics2Model model, float massProportion = 1 ) {
				Shape2Polygon s = new Shape2Polygon( massProportion, this );
				model.Add( s );
				return s;
			}
		}
	}
}
