using Engine.ECS;
using Engine.Structure.Attributes;
using StandardPackage.ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace StandardPackage.ECS.Systems;

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
			linMov.Velocity += linMov.Force / mass.Mass;
			linMov.Momentum = linMov.Velocity * mass.Mass;
			linMov.ResetForce();
			rotMov.AngularVelocity = rotMov.AngularMomentum / mass.Mass;
			rotMov.AngularVelocity += rotMov.Torque / mass.Mass;
			rotMov.AngularMomentum = rotMov.AngularVelocity * mass.Mass;
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
