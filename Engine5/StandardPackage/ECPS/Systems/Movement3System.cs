using Engine.GameLogic.ECPS;
using Engine.GameLogic.ECPS.Components;
using Engine.Structure.Attributes;
using StandardPackage.ECPS.Components;
using System.Numerics;

namespace StandardPackage.ECPS.Systems;

[Require<Transform3Component>]
[Require<LinearMovement3Component>]
[Require<RotationalMovement3Component>]
[Require<Mass3Component>]
public class Movement3System : SystemBase {
	public override void Update( IEnumerable<Entity> entities, float time, float deltaTime ) {
		foreach ( Entity e in entities ) {
			Transform3Component transform = e.Get<Transform3Component>() ?? throw new NullReferenceException( nameof( Transform3Component ) );
			LinearMovement3Component linMov = e.Get<LinearMovement3Component>() ?? throw new NullReferenceException( nameof( LinearMovement3Component ) );
			RotationalMovement3Component rotMov = e.Get<RotationalMovement3Component>() ?? throw new NullReferenceException( nameof( RotationalMovement3Component ) );
			Mass3Component mass = e.Get<Mass3Component>() ?? throw new NullReferenceException( nameof( Mass3Component ) );

			linMov.Velocity = linMov.Momentum / mass.Mass;
			linMov.Velocity += ( linMov.Force / mass.Mass + linMov.CurrentAcceleration ) * deltaTime;
			linMov.Velocity += linMov.CurrentImpulse;
			linMov.Momentum = linMov.Velocity * mass.Mass;
			linMov.CurrentImpulse = Vector3.Zero;
			linMov.CurrentAcceleration = Vector3.Zero;
			linMov.ResetForce();

			rotMov.AngularVelocity = rotMov.AngularMomentum / mass.Mass;
			rotMov.AngularVelocity += ( rotMov.Torque / mass.Mass + rotMov.CurrentAngularAcceleration ) * deltaTime;
			rotMov.AngularVelocity += rotMov.CurrentTwirl;
			rotMov.AngularMomentum = rotMov.AngularVelocity * mass.Mass;
			rotMov.CurrentTwirl = Vector3.Zero;
			rotMov.CurrentAngularAcceleration = Vector3.Zero;
			rotMov.ResetTorque();

			//Rotational energy needs to be conserved. If the inertia of the shape changes, this should be reflected in the spin.
			//Torque should be resisted by inertia.

			transform.Transform.Translation += linMov.Velocity * deltaTime;
			if ( rotMov.AngularVelocity != Vector3.Zero ) {
				float spinLen = rotMov.AngularVelocity.Length();
				transform.Transform.Rotation = Quaternion.Normalize( Quaternion.CreateFromAxisAngle( rotMov.AngularVelocity / spinLen, spinLen * deltaTime ) * transform.Transform.Rotation );
			}
		}
	}
}
