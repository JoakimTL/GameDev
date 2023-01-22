using Engine.GameLogic.ECS;
using System.Numerics;

namespace StandardPackage.ECS.Components;

public sealed class RotationalMovement3Component : ComponentBase {
	public Vector3 AngularVelocity { get; set; }
	public Vector3 Torque { get; private set; }
	public Vector3 AngularMomentum { get; internal set; }
	internal Vector3 CurrentTwirl { get; set; }

	protected override string UniqueNameTag => $"{AngularVelocity}|{Torque}";

	public RotationalMovement3Component() {
		AngularVelocity = Vector3.Zero;
		Torque = Vector3.Zero;
		AngularMomentum = Vector3.Zero;
	}

	/// <summary>
	/// Applies a rotational impulse
	/// </summary>
	/// <param name="twirl"></param>
	public void Twirl( Vector3 twirl ) => CurrentTwirl += twirl;
	public void ResetTorque() => Torque = Vector3.Zero;
	public void ApplyTorwue( Vector3 torqueVector ) => Torque += torqueVector;
}
