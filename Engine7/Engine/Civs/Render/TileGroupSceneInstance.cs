using Civs.World;
using Engine;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Providers;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render;

namespace Civs.Render;

public sealed class TileGroupSceneInstance() : SceneInstanceBase( typeof( Entity3SceneData ) ) {
	public void UpdateMesh( GlobeModel globe, IReadOnlyCollection<FaceRenderModelWithId> faces, MeshProvider meshProvider, Vector4<float>? overrideColor = null ) {
		Mesh?.Dispose();
		SetMesh( CreateMesh( globe, faces, meshProvider, overrideColor ) );
	}

	private IMesh CreateMesh( GlobeModel globe, IReadOnlyCollection<FaceRenderModelWithId> faces, MeshProvider meshProvider, Vector4<float>? overrideColor ) {
		List<Vertex3> vertices = [];
		List<uint> indices = [];
		List<FaceRenderModelWithId> faceList = faces.ToList();
		for (int i = 0; i < faceList.Count; i++) {
			Vector3<float> a = globe.Blueprint.GetVertex( faceList[ i ].Model.IndexA );
			Vector3<float> b = globe.Blueprint.GetVertex( faceList[ i ].Model.IndexB );
			Vector3<float> c = globe.Blueprint.GetVertex( faceList[ i ].Model.IndexC );
			Vector4<byte> color = ((overrideColor ?? (globe.State.GetHeight( faceList[ i ].Id) >= 0 ? (0.37f, 0.97f, 0.2f, 1) : (0.06f, 0.27f, 1.0f, 1))) * 255)
				.Clamp<Vector4<float>, float>( 0, 255 )
				.CastSaturating<float, byte>();

			//Create mesh
			indices.Add( (uint) vertices.Count );
			vertices.Add( new( a, 0, 0, color ) );
			indices.Add( (uint) vertices.Count );
			vertices.Add( new( b, 0, 0, color ) );
			indices.Add( (uint) vertices.Count );
			vertices.Add( new( c, 0, 0, color ) );

		}

		return meshProvider.CreateMesh( vertices.ToArray(), [ .. indices ] );
	}
	public new void SetAllocated( bool allocated ) => base.SetAllocated( allocated );
	public new void SetVertexArrayObject( OglVertexArrayObjectBase? vertexArrayObject ) => base.SetVertexArrayObject( vertexArrayObject );
	public new void SetShaderBundle( ShaderBundleBase? shaderBundle ) => base.SetShaderBundle( shaderBundle );

	public bool Write( Entity3SceneData data ) => Write<Entity3SceneData>( data );
	public bool TryRead( out Entity3SceneData data ) => TryRead<Entity3SceneData>( out data );

	protected override void Initialize() {

	}
}