using Engine.Graphics.Objects.Default.Transforms;
using Engine.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Physics.D3 {
	public class Mass3 {

		/// <summary>
		/// The total mass of this object in Kg
		/// </summary>
		public float Mass { get; private set; }

		public Vector3 LinearMomentum { get; protected set; }
		public Vector3 AngularMomentum { get; protected set; }
		public Vector3 Force { get; protected set; }
		public Vector3 Torque { get; protected set; }

		public Physics3Model Model { get; private set; }

		public Mass3( float massKg ) {
			Mass = massKg;
			Force = 0;
			Torque = 0;
			LinearMomentum = 0;
			AngularMomentum = 0;
		}

		public void SetMass( float massKg ) {
			Mass = massKg;
		}

		public void SetModel( Physics3Model model ) {
			Model = model;
		}

		public float GetKineticEnergy() {
			float I = GetInertia( AngularMomentum );
			return 0.5f * ( LinearMomentum.LengthSquared * Mass + AngularMomentum.LengthSquared * I );
		}

		/// <summary>
		/// Gets the moment of intertia around the input axis.
		/// </summary>
		/// <param name="axis">The axis which to get the moment of inertia from</param>
		/// <returns>The moment of inertia around the axis (kg * m^2)</returns>
		public float GetInertia( Vector3 axis ) {
			if( Model is null )
				return 1;
			if( Model.ShapeCount == 0 )
				return 1;
			if( axis == 0 )
				return 1;
			Model.UpdateShapes();
			Vector3 CoM = Model.GetCenterOfMass();
			float acc = 0;
			axis = axis.Normalized;
			foreach( PhysicsShape<Transform3, Vector3> shape in Model.Shapes ) {
				acc += shape.GetInertia( axis, CoM );
			}
			//Console.WriteLine( "com:" + CoM );
			//Console.WriteLine( "i:" + ( acc / Model.ShapeCount ) );
			//Console.WriteLine( "im:" + ( ( acc / Model.ShapeCount ) * Mass ) );
			return ( acc / Model.ShapeCount ) * Mass;
		}

		/// <summary>
		/// Applies a force to the object at the input location.
		/// //Collision: F = (0.5 * m * v^2) / d
		/// </summary>
		/// <param name="forceOrigin">The origin of the force.</param>
		/// <param name="force">The force applied to this object, represented as a vector (kg*m/s^2 or Newtons))</param>
		public void ApplyForce( Vector3 forceOrigin, Vector3 force ) {
			if( Model is null )
				return;
			Model.UpdateShapes();
			Force += force;
			Torque += Vector3.Cross( force, Model.GetCenterOfMass() - forceOrigin );
		}

		/// <summary>
		/// Applies a force to the object at the center of mass. (no torque)
		/// </summary>
		/// <param name="force">The force applied to this object, represented as a vector (kg*m/s^2 or Newtons))</param>
		public void ApplyForce( Vector3 force ) {
			if( Model is null )
				return;
			Force += force;
		}

		/// <summary>
		/// Applies torque to this mass
		/// </summary>
		/// <param name="torque">The torque applied as a vector.</param>
		public void ApplyTorque( Vector3 torque ) {
			if( Model is null )
				return;
			Torque += torque;
		}

		public void Update( float time, float deltaTime ) {
			if( Model is null )
				return;
			if( Mass <= 0 )
				return;
			if( Torque != 0 ) {
				Console.WriteLine( "t:" + AngularMomentum );
				AngularMomentum += Torque / GetInertia( Torque ) * deltaTime;
				Console.WriteLine( "t:" + AngularMomentum );
			}
			if( Force != 0 ) {
				LinearMomentum += Force / Mass * deltaTime;
				Console.WriteLine( "f:" + ( Force / Mass * deltaTime ) );
				Console.WriteLine( "f:" + LinearMomentum );
			}
			Model.Transform.Rotation = ( Quaternion.FromAxisVector( AngularMomentum * deltaTime ) * Model.Transform.Rotation ).Normalized;
			Model.Transform.Translation += LinearMomentum * deltaTime;
			Torque = 0;
			Force = 0;
		}

	}
}
