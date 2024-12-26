using Engine.Graphics.Objects;
using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Physics.D3.Shapes {
	public class Shape3Ellipsoid : PhysicsShape<Transform3, Vector3> {
		/*public Vector3 Center => transform.GlobalTranslation;
		public override int NumPoints => -1;

		public Shape3Ellipsoid( Vector3 center, float radius ) {
			transform.Translation = center;
			transform.Scale = radius;
		}

		public Shape3Ellipsoid( Vector3 center, Vector3 scale ) {
			transform.Translation = center;
			transform.Scale = scale;
		}

		public Shape3Ellipsoid( Vector3 center, Quaternion rotation, Vector3 scale ) {
			transform.Translation = center;
			transform.Rotation = rotation;
			transform.Scale = scale;
		}

		public override void GetAABB( out Vector3 min, out Vector3 max ) {
			Vector3 minT = Vector3.Transform( -1, transform.TransformationMatrix );
			Vector3 maxT = Vector3.Transform( 1, transform.TransformationMatrix );
			min = new Vector3( Math.Min( minT.X, maxT.X ), Math.Min( minT.Y, maxT.Y ), Math.Min( minT.Z, maxT.Z ) );
			max = new Vector3( Math.Max( minT.X, maxT.X ), Math.Max( minT.Y, maxT.Y ), Math.Max( minT.Z, maxT.Z ) );
		}

		public override Vector3 GetFurthest( Vector3 direction ) {
			Vector4 d = new Vector4( direction, 1 ) * transform.TransformationMatrix.Transposed();
			Vector3 p0 = d.XYZ.Normalized;
			return Vector3.Transform( p0, transform.TransformationMatrix );
		}*/

		private Transform3 transform;
		public TransformInterface<Vector3, Quaternion, Vector3> Transform => transform.Interface;

		public override Vector3 CenterOfMass => transform.GlobalTranslation;

		internal override int UniquePoints => -1;

		private bool updated;

		private static Vector3[] defaultDirections = new Vector3[] {
			new Vector3(-1, 0, 0).Normalized,
			new Vector3(0, 1, 1).Normalized,
			new Vector3(0, -1, 1).Normalized,
			new Vector3(0, 1, -1).Normalized,
			new Vector3(-1, 1, 1).Normalized,
			new Vector3(1, -1, -1).Normalized
		};

		public Shape3Ellipsoid( float massProportion ) : base( massProportion ) {
			transform = new Transform3();
			transform.OnChangedEvent += TransformChanged;
		}

		private void TransformChanged() {
			updated = true;
		}

		public override void GetAABB( out Vector3 min, out Vector3 max ) {
			Vector3 minT = Vector3.Transform( -1, transform.Matrix );
			Vector3 maxT = Vector3.Transform( 1, transform.Matrix );
			min = new Vector3( Math.Min( minT.X, maxT.X ), Math.Min( minT.Y, maxT.Y ), Math.Min( minT.Z, maxT.Z ) );
			max = new Vector3( Math.Max( minT.X, maxT.X ), Math.Max( minT.Y, maxT.Y ), Math.Max( minT.Z, maxT.Z ) );
		}

		public override Vector3 GetFurthest( Vector3 dir ) {
			Vector4 d = new Vector4( dir, 1 ) * transform.Matrix.Transposed();
			Vector3 p0 = d.XYZ.Normalized;
			return Vector3.Transform( p0, transform.Matrix );
		}

		public override float GetInertia( Vector3 dir, Vector3 centerOfMass ) {
			return 2 / 5f;
		}

		public override Vector3 GetInitial() {
			return GetFurthest( -1 );
		}

		internal override Vector3 GetUniquePoint( int id ) {
			return GetFurthest( defaultDirections[ id % defaultDirections.Length ] );
		}

		internal override bool UpdateShape() {
			if( updated ) {
				updated = false;
				return true;
			}
				return false;
		}

		internal override void Added( PhysicsModel<Transform3, Vector3> model ) {
			transform.SetParent( model.Transform );
		}

		internal override void Removed( PhysicsModel<Transform3, Vector3> model ) {
			transform.SetParent( null );
		}
	}
}
