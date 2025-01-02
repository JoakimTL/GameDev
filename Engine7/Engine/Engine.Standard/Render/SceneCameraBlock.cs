using System.Runtime.InteropServices;

namespace Engine.Standard.Render;

[StructLayout( LayoutKind.Explicit )]
public struct SceneCameraBlock( Matrix4x4<float> vpMat, Vector3<float> vUp, Vector3<float> vRight ) {
	[FieldOffset( 0 )]
	public Matrix4x4<float> VPMatrix = vpMat;
	[FieldOffset( 64 )]
	public Vector3<float> ViewUp = vUp;
	[FieldOffset( 80 )]
	public Vector3<float> ViewRight = vRight;
}
