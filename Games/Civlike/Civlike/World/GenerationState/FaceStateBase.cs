using Civlike.World.GameplayState;

namespace Civlike.World.GenerationState;

public abstract class FaceStateBase {
	public FaceBase Face { get; init; } = null!;

	public abstract void Apply( Face.Builder builder );
}