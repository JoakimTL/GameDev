using Engine.ECS;
using System.Numerics;

namespace StandardPackage.ECS.Components;

public sealed class LinearMovement3Component : ComponentBase {
	public Vector3 Velocity { get; set; }
	public Vector3 Force { get; private set; }
	public Vector3 Momentum { get; internal set; }

	public LinearMovement3Component() {
		Velocity = Vector3.Zero;
		Force = Vector3.Zero;
		Momentum = Vector3.Zero;
	}

	public void Impulse( Vector3 impulse ) => Velocity += impulse;

	public void ResetForce() => Force = Vector3.Zero;
	public void ApplyForce( Vector3 newtonsVector ) => Force += newtonsVector;
}
