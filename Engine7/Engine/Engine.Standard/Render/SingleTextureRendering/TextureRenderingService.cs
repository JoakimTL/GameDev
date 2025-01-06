using Engine.Module.Render.Ogl.OOP.DataBlocks;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;
using Engine.Processing;
using Engine.Standard.Render.Meshing.Services;
using Engine.Standard.Render.SingleTextureRendering.Shaders;
using OpenGL;

namespace Engine.Standard.Render.SingleTextureRendering;

//TODO no idea if this works. Renderdoc crashes when I try to use textures...
[Do<IInitializable>.After<VertexArrayLayoutService>()]
public sealed class TextureRenderingService( ShaderBundleService shaderBundleService, CompositeVertexArrayObjectService compositeVertexArrayObjectService, PrimitiveMesh2Provider primitiveMesh2Provider, DataBlockService dataBlockService ) : IInitializable {
	private readonly ShaderBundleService _shaderBundleService = shaderBundleService;
	private readonly CompositeVertexArrayObjectService _compositeVertexArrayObjectService = compositeVertexArrayObjectService;
	private readonly PrimitiveMesh2Provider _primitiveMesh2Provider = primitiveMesh2Provider;
	private readonly DataBlockService _dataBlockService = dataBlockService;

	private ShaderBundleBase _shaderBundle = null!;
	private OglVertexArrayObjectBase _vao = null!;
	private UniformBlock _uniformBlock = null!;
	private IMesh _mesh = null!;
	private IDataBlockCollection _dataBlockCollection = null!;

	public void Initialize() {
		this._shaderBundle = this._shaderBundleService.Get<SingleTextureRenderingShaderBundle>() ?? throw new( "Failed to get shader bundle" );
		this._vao = this._compositeVertexArrayObjectService.Get( [ typeof( Vertex2 ) ] ) ?? throw new( "Failed to get vao" );
		this._mesh = this._primitiveMesh2Provider.Get( Meshing.Primitive2.Rectangle );
		if (!this._dataBlockService.CreateUniformBlock( nameof( SingleTextureRenderingBlock ), 256, [ ShaderType.FragmentShader ], out this._uniformBlock! ))
			throw new( "Failed to get uniform block" );
		_dataBlockCollection = new DataBlockCollection( this._uniformBlock );
	}

	public void RenderTexture( ulong textureHandle ) {
		var shaderPipeline = this._shaderBundle.Get( "default" ) ?? throw new( "Failed to get shader pipeline" );
		shaderPipeline.Bind();
		_vao.Bind();
		_uniformBlock.Buffer.Write( 0u, new SingleTextureRenderingBlock( textureHandle ) );
		_dataBlockCollection.BindShader( shaderPipeline );
		Gl.DrawElements( PrimitiveType.Triangles, (int) _mesh.ElementCount, DrawElementsType.UnsignedInt, 0 );
		_dataBlockCollection.UnbindBuffers();
		OglShaderPipelineBase.Unbind();
		OglVertexArrayObjectBase.Unbind();
	}

}
