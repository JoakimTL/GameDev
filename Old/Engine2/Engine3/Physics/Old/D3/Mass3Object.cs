//using Engine.LinearAlgebra;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Engine.Physics.Old.D3 {
//	public abstract class Mass3Object {

//		public float LastUpdate { get; private set; }

//		public Vector3 Velocity { get; protected set; }
//		public Vector3 Acceleration { get; protected set; }
//		public Vector3 Momentum { get; protected set; }
//		public Vector3 Torque { get; protected set; }

//		/// <summary>
//		/// Gets the moment of intertia around the input axis.
//		/// </summary>
//		/// <param name="axis">The axis which to get the moment of inertia from</param>
//		/// <returns>The moment of inertia around the axis (kg * m^2)</returns>
//		public abstract float GetInertia( Vector3 axis );

//		/// <summary>
//		/// Returns the total mass of this object
//		/// </summary>
//		/// <returns>The mass of this object (kg)</returns>
//		public abstract float GetMass();

//		/// <summary>
//		/// Applies a force to the object at the input location.
//		/// //Collision: F = (0.5 * m * v^2) / d
//		/// </summary>
//		/// <param name="forceOrigin">The origin of the force.</param>
//		/// <param name="force">The force applied to this object, represented as a vector (kg*m/s^2 or Newton))</param>
//		public abstract void ApplyForce( Vector3 forceOrigin, Vector3 force );

//		/// <summary>
//		/// Applies an impulse to the object at the input location.
//		/// </summary>
//		/// <param name="impulseOrigin">The origin of the impulse.</param>
//		/// <param name="impulse">The impulse applied to this object, represented as a vector (kg*m/s or Newton))</param>
//		public abstract void ApplyImpulse( Vector3 forceOrigin, Vector3 force );

//		public void Update( float time ) {
//			float deltaTime = time - LastUpdate;
//			Momentum += Torque * deltaTime;
//			Velocity += Acceleration * deltaTime;
//			Torque = 0;
//			Acceleration = 0;
//			LastUpdate = time;
//		}

//	}
//}
