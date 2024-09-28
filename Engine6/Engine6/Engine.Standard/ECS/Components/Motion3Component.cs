using Engine.Modules.ECS;

namespace Engine.Standard.ECS.Components;

public sealed class Motion3Component : ComponentBase, IMovementComponent<Vector3<double>, Vector3<double>> {

	public Vector3<double> Velocity { get; private set; } = 0;
	public Vector3<double> Momentum { get; private set; } = 0;

	public void AddMomentum( Vector3<double> momentum ) => Momentum += momentum;

	public void AddVelocity( Vector3<double> velocity ) => Velocity += velocity;
}
