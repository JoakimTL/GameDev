using Civlike.World.State;
using Civlike.World.State.States;
using Engine;

namespace Civlike.World.TectonicGeneration.NoiseProviders;

public sealed class TectonicPlate( int id, Vector3<float> position, Vector3<float> movement, float height ) {
	public readonly int Id = id;
	public readonly Vector3<float> Position = position;
	public readonly Vector3<float> Movement = movement;
	public readonly float Height = height;

	//Negative number means these plates are moving away from each other.
	//Positive number means these plates are moving towards each other.
	public float GetFaultMovementDifference( TectonicPlate other ) {
		Vector3<float> positionDiff = (this.Position - other.Position).Normalize<Vector3<float>, float>();
		Vector3<float> movementDiff = (this.Movement - other.Movement) * 0.5f;
		return -positionDiff.Dot( movementDiff );
	}

	public float GetFaultReactionIntensity( TectonicPlate other ) {
		return float.Abs( this.Movement.Dot( other.Movement ) - 1 );
	}
}
