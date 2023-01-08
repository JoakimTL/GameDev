using System.Numerics;

namespace Engine.Physics;
public static class PhysicsExtensions {
	public static Matrix4x4 ScaleInertia( this Matrix4x4 tensor, Vector3 scales ) {
		var scale = Matrix4x4.CreateScale( new Vector3(
			( scales.Y * scales.Y + scales.Z * scales.Z ) * .5f,
			( scales.X * scales.X + scales.Z * scales.Z ) * .5f,
			( scales.X * scales.X + scales.Y * scales.Y ) * .5f ) );
		return tensor * scale;
	}
}
