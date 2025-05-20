using Civlike.World;
using Engine;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Providers;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render;

namespace Civlike.Client.Render;

public sealed class TileGroupSceneInstance() : SceneInstanceBase( typeof( Entity3SceneData ) ) {
	public void UpdateMesh( IReadOnlyList<Face> faces, MeshProvider meshProvider, Vector4<float>? overrideColor = null ) {
		Mesh?.Dispose();
		SetMesh( CreateMesh( faces, meshProvider, overrideColor ) );
	}

	private IMesh CreateMesh( IReadOnlyList<Face> faces, MeshProvider meshProvider, Vector4<float>? overrideColor ) {
		List<Vertex3> vertices = [];
		List<uint> indices = [];
		for (int i = 0; i < faces.Count; i++) {
			var face = faces[ i ];
			Vector3<float> a = face.Blueprint.VectorA;
			Vector3<float> b = face.Blueprint.VectorB;
			Vector3<float> c = face.Blueprint.VectorC;
			Vector4<byte> color = ((overrideColor ?? face.State./*TerrainType.*/Color) * 255)
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