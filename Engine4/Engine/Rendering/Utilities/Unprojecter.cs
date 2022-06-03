using System.Numerics;

namespace Engine.Rendering.Utilities;
public static class Unprojector {
	public static Vector3 GetMouseUnprojected( Matrix4x4 inverseProjection, Matrix4x4 inverseView, Vector2 ndc ) {
		Vector4 mouseVector = new( ndc.X, ndc.Y, -1, 1 );
		Vector4 mouseEye = Vector4.Transform( mouseVector, inverseProjection );
		mouseEye.Z = -1;
		mouseEye.W = 0;
		Vector4 mouseWorld = Vector4.Transform( mouseEye, inverseView );

		return Vector3.Normalize( new Vector3( mouseWorld.X, mouseWorld.Y, mouseWorld.Z ) );
	}
}
