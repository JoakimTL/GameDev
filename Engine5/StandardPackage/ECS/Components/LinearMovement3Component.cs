using Engine.Datatypes.Transforms;
using Engine;
using Engine.GameLogic.ECS;
using Engine.Structure.Interfaces;
using System.Numerics;

namespace StandardPackage.ECS.Components;

public sealed class LinearMovement3Component : ComponentBase, ISerializable {
	public Vector3 Velocity { get; set; }
	public Vector3 Force { get; private set; }
	public Vector3 Momentum { get; internal set; }
	internal Vector3 CurrentAcceleration { get; set; }
	internal Vector3 CurrentImpulse { get; set; }

	protected override string UniqueNameTag => $"{Velocity}|{Force}";

	public static Guid TypeIdentity { get; } = new( "c7a1da80-eec7-4a4e-8dec-cbd7630a174e" );

	public LinearMovement3Component() {
		Velocity = Vector3.Zero;
		Force = Vector3.Zero;
		Momentum = Vector3.Zero;
	}

	public void Impulse( Vector3 impulse ) => CurrentImpulse += impulse;
	public void ResetForce() => Force = Vector3.Zero;
	public void Accelerate( Vector3 accelerationVector ) => CurrentAcceleration += accelerationVector;
	public void ApplyForce( Vector3 newtonsVector ) => Force += newtonsVector;

	public bool DeserializeData( byte[] data ) {
		unsafe {
			if ( (uint) data.Length < sizeof( Vector3 ) )
				return Log.WarningThenReturn( $"Length of data must be equal to or longer than {sizeof( Vector3 )}", false );
			fixed ( byte* srcPtr = data ) 
				Momentum = *(Vector3*) srcPtr;
		}
		return true;
	}

	public byte[] SerializeData() {
		unsafe {
			byte[] data = new byte[ sizeof( Vector3 ) ];
			fixed ( byte* dstPtr = data )
				*(Vector3*) dstPtr = Momentum;
			return data;
		}
	}
}
