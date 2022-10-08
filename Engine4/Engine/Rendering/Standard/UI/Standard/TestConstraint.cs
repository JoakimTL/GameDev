using System.Numerics;
using Engine.Data.Datatypes.Transforms;

namespace Engine.Rendering.Standard.UI.Standard;

internal class TestConstraint : IUIConstraint<Transform2> {
	public int ExecutionOrder => 0;

	public void Apply( float time, float deltaTime, Transform2 transform ) {
		transform.Translation = new Vector2( .025f, -.025f );
		transform.Scale = new Vector2( .025f, .025f );
	}
}
