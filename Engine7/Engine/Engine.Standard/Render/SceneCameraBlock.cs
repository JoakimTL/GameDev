using System.Runtime.InteropServices;

namespace Engine.Standard.Render;

[StructLayout( LayoutKind.Explicit )]
public struct SceneCameraBlock {
	[FieldOffset( 0 )]
	public Matrix4x4<float> VPMatrix;
	[FieldOffset( 64 )]
	public Vector3<float> ViewUp;
	[FieldOffset( 80 )]
	public Vector3<float> ViewRight;

	public SceneCameraBlock( Matrix4x4<float> vpMat, Vector3<float> vUp, Vector3<float> vRight ) {
		this.VPMatrix = vpMat;
		this.ViewUp = vUp;
		this.ViewRight = vRight;
	}
}
