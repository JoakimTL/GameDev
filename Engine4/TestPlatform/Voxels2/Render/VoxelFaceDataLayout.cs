using System.Runtime.InteropServices;
using Engine;
using Engine.Rendering.Standard.VertexArrayObjects;

namespace TestPlatform.Voxels2.Render;

[VertexLayoutBinding( typeof( VoxelFaceData ) )]
public class VoxelFaceDataLayout : CompositeVertexArrayObjectDataLayout {
	public VoxelFaceDataLayout() : base( nameof( VoxelFaceData ), Resources.Render.VBOs.Get<VoxelFaceData>(), 0, Marshal.SizeOf<VoxelFaceData>(), 1 ) {
		AddAttribute( new( OpenGL.VertexAttribType.Int, 3, 0 ) ); //translation
		AddAttribute( new( OpenGL.VertexAttribType.UnsignedShort, 1, 12, AttributeType.INTEGER ) ); //scale
		AddAttribute( new( OpenGL.VertexAttribType.UnsignedShort, 1, 14, AttributeType.INTEGER ) ); //id
	}
}