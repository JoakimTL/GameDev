using Engine.Graphics.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Physics {
	public abstract class PhysicsShape<T, V> where T : Transform {

		/// <summary>
		/// The center of mass for this shape. Updated when using UpdateShape
		/// </summary>
		public abstract V CenterOfMass { get; }
		/// <summary>
		/// The proportion of mass concentrated in this shape. If all the proportions in one model is uniform, the mass is shared equally throughout the shapes.
		/// </summary>
		public float MassProportion { get; private set; }

		/// <summary>
		/// The amount of unique points in this shape. -1 if there are a theoretical infinite amount of unique points in this shape.
		/// </summary>
		internal abstract int UniquePoints { get; }

		public PhysicsShape( float massProportion ) {
			MassProportion = 1;
			if( massProportion > 0 )
				MassProportion = massProportion;
		}

		/// <summary>
		/// Mass proportion cannot be less than 0
		/// </summary>
		/// <param name="massProportion">The proportion of mass in this shape compared to the rest of the model</param>
		public void SetMassProportion( float massProportion ) {
			if( massProportion > 0 )
				MassProportion = massProportion;
		}

		public abstract void GetAABB( out V min, out V max );
		public abstract V GetInitial();
		public abstract V GetFurthest( V dir );
		/// <summary>
		/// Gets the point referenced by the id.
		/// </summary>
		/// <param name="id">Id of the point</param>
		/// <returns></returns>
		internal abstract V GetUniquePoint( int id );
		//public abstract void GetClosest( V target, out V closest, out V nextClosest );
		public abstract float GetInertia( V dir, V centerOfMass );
		/// <summary>
		/// 
		/// </summary>
		/// <returns>Whether or not the shape was updated.</returns>
		internal abstract bool UpdateShape();
		internal abstract void Added( PhysicsModel<T, V> model );
		internal abstract void Removed( PhysicsModel<T, V> model );

	}
}
