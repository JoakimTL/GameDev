using System.Runtime.InteropServices;
using Engine;
using Engine.Rendering.Standard.VertexArrayObjects;

namespace TestPlatform.Voxels.Rendering;
[VertexLayoutBinding( typeof( VoxelFaceData ) )]
public class VoxelFaceDataLayout : CompositeVertexArrayObjectDataLayout {
	public VoxelFaceDataLayout() : base( nameof( VoxelFaceData ), Resources.Render.VBOs.Get<VoxelFaceData>(), 0, Marshal.SizeOf<VoxelFaceData>(), 0 ) {
		AddAttribute( new( OpenGL.VertexAttribType.Int, 3, 0 ) ); //translation
		AddAttribute( new( OpenGL.VertexAttribType.UnsignedByte, 3, 12 ) ); //scale
		AddAttribute( new( OpenGL.VertexAttribType.UnsignedShort, 1, 15 ) ); //id
	}
}
