using Engine;
using Engine.Rendering.Standard;
using Engine.Rendering.Standard.Meshing;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;
using TestPlatform.Voxels.World;

namespace TestPlatform.Voxels.Rendering;

public class ChunkRender : ClosedSceneObject<Vertex3, VoxelFaceData> {

	public const uint InstanceCount = VoxelChunk.SideLength * VoxelChunk.SideLength * VoxelChunk.SideLength / (2 * 8);

	public ChunkRender( VertexMesh<Vertex3> mesh, SceneInstanceData<VoxelFaceData> sceneData, bool transparent ) {
		SetMesh( mesh );
		SetShaders( transparent ? Resources.Render.Shader.Bundles.Get<VoxelTransparentShaderBundle>() : Resources.Render.Shader.Bundles.Get<VoxelShaderBundle>() );
		SetSceneData( sceneData );
	}
}
