using Engine;
using Engine.Standard.Render;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Providers;
using Engine.Module.Render.Ogl.Scenes;
using Civs.World;

namespace Civs.Render.Nations;

public sealed class PopulationCenterTileGroupSceneInstance() : SceneInstanceBase( typeof( Entity3SceneData ) ) {
	public void UpdateMesh( IReadOnlyList<Face> faces, MeshProvider meshProvider, Vector4<float> overrideColor ) {
		Mesh?.Dispose();
		SetMesh( CreateMesh( faces, meshProvider, overrideColor ) );
	}

	private IMesh CreateMesh( IReadOnlyList<Face> faces, MeshProvider meshProvider, Vector4<float> overrideColor ) {
		List<Vertex3> vertices = [];
		List<uint> indices = [];

		HashSet<Face> borderFaces = [];
		HashSet<GlobeVertex> borderVertices = [];
		Span<uint> vertexIndices = stackalloc uint[ 3 ];
		//Border faces are all faces where not all vertices are shared with other faces in the faces list.
		for (int i = 0; i < faces.Count; i++) {
			Face face = faces[ i ];
			for (uint j = 0; j < 3; j++) {
				GlobeVertex vertex = face.Blueprint.GetVertex( j );
				if (!vertex.ConnectedFaces.All( faces.Contains ))
					borderVertices.Add( vertex );
			}
		}
		foreach (GlobeVertex borderVertex in borderVertices)
			foreach (Face potentialBorderFace in borderVertex.ConnectedFaces)
				if (faces.Contains( potentialBorderFace ))
					borderFaces.Add( potentialBorderFace );

		Vector4<byte> borderColor = (overrideColor * 255)
			.Clamp<Vector4<float>, float>( 0, 255 )
			.CastSaturating<float, byte>();
		Vector4<byte> internalColor = (new Vector4<float>( overrideColor.X, overrideColor.Y, overrideColor.Z, 0 ) * 255)
			.Clamp<Vector4<float>, float>( 0, 255 )
			.CastSaturating<float, byte>();

		foreach (Face borderFace in borderFaces) {
			//                 */\
			//                 /  \
			//                /	   \
			//               /	    \
			//              /		 \
			//             /		  \
			//            /            \
			//           /		 	    \
			//          *       *        *
			//         /		  	      \
			//        /		               \
			//       /		  	 	        \
			//      /      *         *       \
			//     /					 	  \
			//    /					 	       \
			//   /				 	 	 	    \
			// */_______________*________________\*
			GlobeVertex vertexA = borderFace.Blueprint.GetVertex( 0 );
			GlobeVertex vertexB = borderFace.Blueprint.GetVertex( 1 );
			GlobeVertex vertexC = borderFace.Blueprint.GetVertex( 2 );
			bool aAtBorder = borderVertices.Contains( vertexA );
			bool bAtBorder = borderVertices.Contains( vertexB );
			bool cAtBorder = borderVertices.Contains( vertexC );
			bool abAtBorder = !faces.Contains( borderFace.Blueprint.Connections[ 0 ].GetOther( borderFace ) );
			bool bcAtBorder = !faces.Contains( borderFace.Blueprint.Connections[ 1 ].GetOther( borderFace ) );
			bool acAtBorder = !faces.Contains( borderFace.Blueprint.Connections[ 2 ].GetOther( borderFace ) );

			Vector3<float> a = vertexA.Vector;
			Vector3<float> b = vertexB.Vector;
			Vector3<float> c = vertexC.Vector;
			Vector3<float> abCenter = (a + b) / 2f;
			Vector3<float> acCenter = (a + c) / 2f;
			Vector3<float> bcCenter = (b + c) / 2f;
			//var abacCenter = (abCenter + acCenter) / 2f;
			//var abbcCenter = (abCenter + bcCenter) / 2f;
			//var acbcCenter = (acCenter + bcCenter) / 2f;
			//Vector3<float> center = (a + b + c) / 3f;

			Vector4<byte> aColor = aAtBorder ? borderColor : internalColor;
			Vector4<byte> bColor = bAtBorder ? borderColor : internalColor;
			Vector4<byte> cColor = cAtBorder ? borderColor : internalColor;
			Vector4<byte> abCenterColor = abAtBorder ? borderColor : internalColor;
			Vector4<byte> acCenterColor = acAtBorder ? borderColor : internalColor;
			Vector4<byte> bcCenterColor = bcAtBorder ? borderColor : internalColor;
			//Vector4<byte> centerColor = internalColor;

			//Create mesh
			uint startIndex = (uint) vertices.Count;
			vertices.Add( new( a, 0, 0, aColor ) );
			vertices.Add( new( b, 0, 0, bColor ) );
			vertices.Add( new( c, 0, 0, cColor ) );
			vertices.Add( new( abCenter, 0, 0, abCenterColor ) );
			vertices.Add( new( acCenter, 0, 0, acCenterColor ) );
			vertices.Add( new( bcCenter, 0, 0, bcCenterColor ) );
			uint aIndex = startIndex;
			uint bIndex = startIndex + 1;
			uint cIndex = startIndex + 2;
			uint abCenterIndex = startIndex + 3;
			uint acCenterIndex = startIndex + 4;
			uint bcCenterIndex = startIndex + 5;
			indices.Add( aIndex );
			indices.Add( abCenterIndex );
			indices.Add( acCenterIndex );

			indices.Add( bIndex );
			indices.Add( bcCenterIndex );
			indices.Add( abCenterIndex );

			indices.Add( cIndex );
			indices.Add( acCenterIndex );
			indices.Add( bcCenterIndex );

			indices.Add( abCenterIndex );
			indices.Add( bcCenterIndex );
			indices.Add( acCenterIndex );


		}

		//for (int i = 0; i < faces.Count; i++) {
		//	var face = faces[ i ];
		//	Vector3<float> a = face.Blueprint.VectorA;
		//	Vector3<float> b = face.Blueprint.VectorB;
		//	Vector3<float> c = face.Blueprint.VectorC;
		//	Vector3<float> abCenter = (a + b) / 2f;
		//	Vector3<float> acCenter = (a + c) / 2f;
		//	Vector3<float> bcCenter = (b + c) / 2f;
		//	Vector3<float> center = (a + b + c) / 3f;
		//	Vector4<byte> color = ((overrideColor ?? face.State.TerrainType.Color) * 255)
		//		.Clamp<Vector4<float>, float>( 0, 255 )
		//		.CastSaturating<float, byte>();

		//	//Create mesh
		//	uint startIndex = (uint) vertices.Count;
		//	vertices.Add( new( a, 0, 0, color ) );
		//	vertices.Add( new( b, 0, 0, color ) );
		//	vertices.Add( new( c, 0, 0, color ) );
		//	vertices.Add( new( abCenter, 0, 0, (color.X, color.Y, color.Z, 25) ) );
		//	vertices.Add( new( center, 0, 0, (color.X, color.Y, color.Z, 25) ) );
		//	indices.Add( startIndex );
		//	indices.Add( startIndex + 1 );
		//	indices.Add( startIndex + 3 );
		//	indices.Add( startIndex + 1 );
		//	indices.Add( startIndex + 2 );
		//	indices.Add( startIndex + 3 );
		//	indices.Add( startIndex + 2 );
		//	indices.Add( startIndex );
		//	indices.Add( startIndex + 3 );

		//}

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