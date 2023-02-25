using Engine;
using Engine.GameLogic.ECPS;
using Engine.Structure.Interfaces;
using System.Numerics;

namespace StandardPackage.ECPS.Components;

public sealed class RotationalMovement3Component : ComponentBase, ISerializable {
	public Vector3 AngularVelocity { get; set; }
	public Vector3 Torque { get; private set; }
	public Vector3 AngularMomentum { get; internal set; }
	internal Vector3 CurrentAngularAcceleration { get; set; }
	internal Vector3 CurrentTwirl { get; set; }

	protected override string UniqueNameTag => $"{AngularVelocity}|{Torque}";

	public static Guid TypeIdentity { get; } = new( "5e882cf7-e3b8-4bf4-929b-c7603af3c6dc" );

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
	public void Accelerate( Vector3 accelerationVector ) => CurrentAngularAcceleration += accelerationVector;
	public void ApplyTorque( Vector3 torqueVector ) => Torque += torqueVector;

	public bool DeserializeData( byte[] data ) {
		unsafe {
			if ( (uint) data.Length < sizeof( Vector3 ) )
				return Log.WarningThenReturn( $"Length of data must be equal to or longer than {sizeof( Vector3 )}", false );
			fixed ( byte* srcPtr = data )
				AngularMomentum = *(Vector3*) srcPtr;
		}
		return true;
	}

	public byte[] SerializeData() {
		unsafe {
			byte[] data = new byte[ sizeof( Vector3 ) ];
			fixed ( byte* dstPtr = data )
				*(Vector3*) dstPtr = AngularMomentum;
			return data;
		}
	}
}
