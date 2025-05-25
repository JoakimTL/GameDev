
namespace Civlike.World.GenerationState;

public sealed class Face<TState> : FaceBase where TState : FaceStateBase, new() {
	public Face( uint id, Vertex[] vertices, Edge[] edges ) : base(id, vertices, edges) {
		State = new() {
			Face = this
		};
	}

	public TState State { get; }

	public override void Apply( GameplayState.Face.Builder builder ) => State.Apply( builder );
}
