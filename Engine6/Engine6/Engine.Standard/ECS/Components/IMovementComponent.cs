namespace Engine.Standard.ECS.Components;

public interface IMovementComponent<TVelocity, TMomentum> {
	TVelocity Velocity { get; }
	TMomentum Momentum { get; }

	void AddVelocity( TVelocity velocity );
	void AddMomentum( TMomentum momentum );
}