using System.Numerics;
using Engine.Rendering.Standard;
using Engine.Rendering.Standard.Meshing;
using Engine.Rendering.Standard.Scenes;
using Engine.Rendering.Standard.VertexArrayObjects;
using OpenGL;

namespace Engine.Rendering.Utilities;
public static class RenderUtils {

	public static void RenderPFX( ShaderPipeline shader, VertexMesh<Vector2> mesh, DataBlockCollection uniforms ) {
		if ( mesh.ElementSegmentData is null || mesh.VertexSegmentData is null ) {
			Log.Line( $"{mesh} invalid, rendering skipped.", Log.Level.CRITICAL );
			return;
		}
		if ( !Resources.Render.CompositeVAOs.TryGet<Vector2>(out CompositeVertexArrayObject? vao ) ) {
			Log.Line( $"Unable to get VAO!", Log.Level.CRITICAL );
			return;
		}

		shader.DirectBind();
		vao.DirectBind();
		uniforms.DirectBindShader( shader );

		Gl.DrawElementsBaseVertex( PrimitiveType.Triangles, (int) mesh.ElementSegmentData.OffsetBytes / sizeof( uint ), DrawElementsType.UnsignedInt, (IntPtr) mesh.ElementSegmentData.OffsetBytes, (int) mesh.VertexSegmentData.OffsetBytes );
	}

	public static void Render( ShaderPipeline shader, VertexArrayObject vao, IndirectCommand command, DataBlockCollection uniforms ) {
		if ( shader is null ) {
			Log.Line( $"Shader invalid, rendering skipped.", Log.Level.CRITICAL );
			return;
		}
		if ( vao is null ) {
			Log.Line( $"VAO invalid, rendering skipped.", Log.Level.CRITICAL );
			return;
		}

		shader.DirectBind();
		vao.DirectBind();
		uniforms.DirectBindShader( shader );
		Gl.DrawElementsInstancedBaseVertex(
			PrimitiveType.Triangles,
			(int) command.Count,
			DrawElementsType.UnsignedInt,
			(IntPtr) ( command.FirstIndex * sizeof( uint ) ),
			(int) command.InstanceCount,
			(int) command.BaseVertex
		);
	}
}
