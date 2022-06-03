using System.Numerics;
using System.Runtime.InteropServices;

namespace Engine.Rendering.Data;
[StructLayout( LayoutKind.Explicit )]
public struct SceneCameraBlock {
	[FieldOffset( 0 )]
	public Matrix4x4 VPMatrix;
	[FieldOffset( 64 )]
	public Vector3 ViewUp;
	[FieldOffset( 80 )]
	public Vector3 ViewRight;

	public SceneCameraBlock( Matrix4x4 vpMat, Vector3 vUp, Vector3 vRight ) {
		this.VPMatrix = vpMat;
		this.ViewUp = vUp;
		this.ViewRight = vRight;
	}
}