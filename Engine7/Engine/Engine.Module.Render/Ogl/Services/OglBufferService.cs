using Engine.Module.Render.Ogl.OOP.Buffers;
using OpenGL;

namespace Engine.Module.Render.Ogl.Services;

public sealed class OglBufferService : DisposableIdentifiable, IInitializable {

	private readonly Dictionary<Type, OglDynamicBuffer> _buffers = [];
	private OglDynamicBuffer? _elementBuffer = null;
	private OglSegmentedBuffer? _uniformBuffer = null;
	private OglSegmentedBuffer? _shaderStorage = null;

	public OglStaticBuffer ElementBuffer => this._elementBuffer ?? throw new NullReferenceException( "ElementBuffer not initialized" );
	public OglSegmentedBuffer UniformBuffer => this._uniformBuffer ?? throw new NullReferenceException( "UniformBuffer not initialized" );
	public OglSegmentedBuffer ShaderStorage => this._shaderStorage ?? throw new NullReferenceException( "ShaderStorage not initialized" );

	public void Initialize() {
		this._elementBuffer = new( BufferUsage.DynamicDraw, 65_536u ) { Nickname = nameof( this.ElementBuffer ) };
		this._uniformBuffer = new( BufferUsage.DynamicDraw, 65_536u, Buffers.BufferAutoResizeMode.LINEAR ) { Nickname = nameof( this.UniformBuffer ) };
		this._shaderStorage = new( BufferUsage.DynamicDraw, 65_536u, Buffers.BufferAutoResizeMode.DOUBLE ) { Nickname = nameof( this.ShaderStorage ) };
	}

	public OglStaticBuffer Get( Type t ) {
		if (this._buffers.TryGetValue( t, out OglDynamicBuffer? vbo ))
			return vbo;
		vbo = new OglDynamicBuffer( BufferUsage.DynamicDraw, 65536 ) { Nickname = t.Name };
		this._buffers[ t ] = vbo;
		return vbo;
	}

	protected override bool InternalDispose() {
		this.ElementBuffer.Dispose();
		this.UniformBuffer.Dispose();
		this.ShaderStorage.Dispose();
		foreach (OglDynamicBuffer vbo in this._buffers.Values)
			vbo.Dispose();
		return true;
	}
}
