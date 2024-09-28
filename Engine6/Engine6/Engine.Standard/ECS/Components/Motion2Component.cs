using Engine.Modules.ECS;

namespace Engine.Standard.ECS.Components;

public sealed class Motion2Component : ComponentBase, IMovementComponent<Vector2<double>, double> {

	public Vector2<double> Velocity { get; private set; } = 0;
	public double Momentum { get; private set; } = 0;

	public void AddMomentum( double momentum ) => Momentum += momentum;

	public void AddVelocity( Vector2<double> velocity ) => Velocity += velocity;

}
