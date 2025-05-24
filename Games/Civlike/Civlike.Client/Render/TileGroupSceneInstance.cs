using Civlike.World.GameplayState;
using Engine;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Providers;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render;
using System.Runtime.CompilerServices;

namespace Civlike.Client.Render;

public sealed class TileGroupSceneInstance() : SceneInstanceBase( typeof( Entity3SceneData ) ) {
	public void UpdateMesh( Globe globe, IReadOnlyList<Face> faces, MeshProvider meshProvider, Vector4<float>? overrideColor = null ) {
		this.Mesh?.Dispose();
		SetMesh( CreateMesh( globe, faces, meshProvider, overrideColor ) );
	}

	private static IMesh CreateMesh( Globe globe, IReadOnlyList<Face> faces, MeshProvider meshProvider, Vector4<float>? overrideColor ) {
		List<Vertex3> vertices = [];
		List<uint> indices = [];
		float globeRadius = (float) globe.Radius;
		for (int i = 0; i < faces.Count; i++) {
			Face face = faces[ i ];
			Vertex vertexA = face.Blueprint.Vertices[ 0 ];
			Vertex vertexB = face.Blueprint.Vertices[ 1 ];
			Vertex vertexC = face.Blueprint.Vertices[ 2 ];
			Vector3<float> a = vertexA.Vector * ((vertexA.Height + globeRadius) / globeRadius);
			Vector3<float> b = vertexB.Vector * ((vertexB.Height + globeRadius) / globeRadius);
			Vector3<float> c = vertexC.Vector * ((vertexC.Height + globeRadius) / globeRadius);
			Vector4<byte> color = ((overrideColor ?? face.State /*.TerrainType*/.Color) * 255)
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