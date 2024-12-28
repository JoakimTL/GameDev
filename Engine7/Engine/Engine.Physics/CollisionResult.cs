namespace Engine.Physics;

public readonly struct CollisionResult {
	public bool IsColliding { get; }
	public bool HasError { get; }

	internal CollisionResult( bool isColliding, bool hasError ) {
		IsColliding = isColliding;
		HasError = hasError;
	}
}