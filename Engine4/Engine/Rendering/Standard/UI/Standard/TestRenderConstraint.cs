using System.Numerics;

namespace Engine.Rendering.Standard.UI.Standard;

internal class TestRenderConstraint : IUIConstraint<ButtonData> {
	public int ExecutionOrder => 0;

	public void Apply( float time, float deltaTime, ButtonData data ) => data.Color = new Vector4( MathF.Sin( time  ) / 2 + .5f, MathF.Sin( time + MathF.PI * 2 / 3f ) / 2 + .5f, MathF.Sin( time  + MathF.PI * 4 / 3f ) / 2 + .5f, 1 );
}
