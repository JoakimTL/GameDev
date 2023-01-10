using Engine.ECS;
using System.Numerics;

namespace StandardPackage.ECS.Components;

public sealed class RotationalMovement3Component : ComponentBase {
	public Vector3 AngularVelocity { get; set; }
	public Vector3 Torque { get; private set; }
	public Vector3 AngularMomentum { get; internal set; }

	public RotationalMovement3Component() {
		AngularVelocity = Vector3.Zero;
		Torque = Vector3.Zero;
		AngularMomentum = Vector3.Zero;
	}

	public void Twirl(Vector3 twirl) => AngularVelocity += twirl;

	public void ResetTorque() => Torque = Vector3.Zero;
	public void ResetTorque( Vector3 torqueVector ) => Torque += torqueVector;
}
