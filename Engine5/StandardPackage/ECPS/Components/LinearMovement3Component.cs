using Engine;
using Engine.Structure.Interfaces;
using System.Numerics;
using Engine.GameLogic.ECPS;

namespace StandardPackage.ECPS.Components;

public sealed class LinearMovement3Component : ComponentBase, ICustomizedSerializable {
	public Vector3 Velocity { get; set; }
	public Vector3 Force { get; private set; }
	public Vector3 Momentum { get; internal set; }
	internal Vector3 CurrentAcceleration { get; set; }
	internal Vector3 CurrentImpulse { get; set; }

	protected override string UniqueNameTag => $"{this.Velocity}|{this.Force}";

	public static Guid SerializationIdentity { get; } = new( "c7a1da80-eec7-4a4e-8dec-cbd7630a174e" );
	public bool ShouldSerialize => true;

	public LinearMovement3Component() {
		this.Velocity = Vector3.Zero;
		this.Force = Vector3.Zero;
		this.Momentum = Vector3.Zero;
	}

	public void Impulse( Vector3 impulse ) => this.CurrentImpulse += impulse;
	public void ResetForce() => this.Force = Vector3.Zero;
	public void Accelerate( Vector3 accelerationVector ) => this.CurrentAcceleration += accelerationVector;
	public void ApplyForce( Vector3 newtonsVector ) => this.Force += newtonsVector;

	public bool DeserializeData( byte[] data ) {
		unsafe {
			if ( (uint) data.Length < sizeof( Vector3 ) )
				return Log.WarningThenReturn( $"Length of data must be equal to or longer than {sizeof( Vector3 )}", false );
			fixed ( byte* srcPtr = data )
				this.Momentum = *(Vector3*) srcPtr;
		}
		return true;
	}

	public byte[] SerializeData() {
		unsafe {
			byte[] data = new byte[ sizeof( Vector3 ) ];
			fixed ( byte* dstPtr = data )
				*(Vector3*) dstPtr = this.Momentum;
			return data;
		}
	}
}
