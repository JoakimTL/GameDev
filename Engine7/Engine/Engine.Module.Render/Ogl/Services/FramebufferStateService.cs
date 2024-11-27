using Engine.Logging;
using Engine.Module.Render.Ogl.OOP;
using Engine.Module.Render.Ogl.OOP.Buffers;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Processing;
using OpenGL;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine.Module.Render.Ogl.Services;

public sealed class FramebufferStateService( ViewportStateService viewport ) : Identifiable {

	private readonly ViewportStateService _viewport = viewport;
	private uint _boundDrawBuffer = 0, _boundReadBuffer = 0;

	public OglFramebuffer CreateFramebuffer( Vector2<int> size ) {
		uint framebufferId = Gl.CreateFramebuffer();
		return new( framebufferId, size );
	}

	public void BindFramebuffer( FramebufferTarget target, OglFramebuffer buffer ) {
		if (buffer.Disposed) {
			this.LogWarning( $"Trying to bind disposed framebuffer {buffer}" );
			return;
		}
		if (buffer.RequiresGeneration)
			buffer.Generate();
		switch (target) {
			case FramebufferTarget.DrawFramebuffer:
				if (buffer.FramebufferId == _boundDrawBuffer)
					this.LogWarning( $"Framebuffer {buffer} already bound to draw buffer" );
				_boundDrawBuffer = buffer.FramebufferId;
				Gl.BindFramebuffer( target, buffer.FramebufferId );
				_viewport.Set( 0, buffer.Size );
				break;
			case FramebufferTarget.ReadFramebuffer:
				if (buffer.FramebufferId == _boundReadBuffer)
					this.LogWarning( $"Framebuffer {buffer} already bound to read buffer" );
				_boundReadBuffer = buffer.FramebufferId;
				Gl.BindFramebuffer( target, buffer.FramebufferId );
				break;
			case FramebufferTarget.Framebuffer:
				if (buffer.FramebufferId == _boundDrawBuffer && buffer.FramebufferId == _boundReadBuffer)
					this.LogWarning( $"Framebuffer {buffer} already bound to both draw and read buffer" );
				_boundDrawBuffer = _boundReadBuffer = buffer.FramebufferId;
				Gl.BindFramebuffer( target, buffer.FramebufferId );
				_viewport.Set( 0, buffer.Size );
				break;
			default:
				throw new ArgumentOutOfRangeException( nameof( target ), target, null );
		}
	}

	public void UnbindFramebuffer( FramebufferTarget target ) {
		switch (target) {
			case FramebufferTarget.DrawFramebuffer:
				if (_boundDrawBuffer == 0)
					this.LogWarning( "Trying to unbind framebuffer as draw buffer, but no framebuffer bound to draw buffer" );
				_boundDrawBuffer = 0;
				Gl.BindFramebuffer( target, 0 );
				break;
			case FramebufferTarget.ReadFramebuffer:
				if (_boundReadBuffer == 0)
					this.LogWarning( "Trying to unbind framebuffer as read buffer, but no framebuffer bound to read buffer" );
				_boundReadBuffer = 0;
				Gl.BindFramebuffer( target, 0 );
				break;
			case FramebufferTarget.Framebuffer:
				if (_boundDrawBuffer == 0 && _boundReadBuffer == 0)
					this.LogWarning( "Trying to unbind framebuffer as both draw and read buffer, but no framebuffer bound to either buffer" );
				_boundDrawBuffer = _boundReadBuffer = 0;
				Gl.BindFramebuffer( target, 0 );
				break;
			default:
				throw new ArgumentOutOfRangeException( nameof( target ), target, null );
		}
	}

	public void BlitFramebuffer( Vector2<int> srcPoint1, Vector2<int> srcPoint2, Vector2<int> dstPoint1, Vector2<int> dstPoint2, ClearBufferMask mask, BlitFramebufferFilter filter ) {
		if (srcPoint1.X < 0 || srcPoint1.Y < 0)
			throw new OpenGlArgumentException( "Point cannot be negative", nameof( srcPoint1 ) );
		if (dstPoint1.X < 0 || dstPoint1.Y < 0)
			throw new OpenGlArgumentException( "Point cannot be negative", nameof( dstPoint1 ) );
		if (srcPoint2.X < srcPoint1.X || srcPoint2.Y < srcPoint1.Y)
			throw new OpenGlArgumentException( "Second point must be greater than first point", nameof( srcPoint2 ) );
		if (dstPoint2.X < dstPoint1.X || dstPoint2.Y < dstPoint1.Y)
			throw new OpenGlArgumentException( "Second point must be greater than first point", nameof( dstPoint2 ) );
		Gl.BlitFramebuffer(
			srcPoint1.X, srcPoint1.Y, srcPoint2.X, srcPoint2.Y,
			dstPoint1.X, dstPoint1.Y, dstPoint2.X, dstPoint2.Y,
			mask, filter );
	}

	public void BlitToFrameBuffer( OglFramebuffer source, OglFramebuffer destination, ClearBufferMask mask, BlitFramebufferFilter filter ) {
		BindFramebuffer( FramebufferTarget.ReadFramebuffer, source );
		BindFramebuffer( FramebufferTarget.DrawFramebuffer, destination );
		BlitFramebuffer( (0, 0), source.Size, (0, 0), destination.Size, mask, filter );
		UnbindFramebuffer( FramebufferTarget.ReadFramebuffer );
		UnbindFramebuffer( FramebufferTarget.DrawFramebuffer );
	}

	public void BlitToScreen( OglFramebuffer source, OglWindow window, ClearBufferMask mask, BlitFramebufferFilter filter ) {
		BindFramebuffer( FramebufferTarget.ReadFramebuffer, source );
		UnbindFramebuffer( FramebufferTarget.DrawFramebuffer );
		BlitFramebuffer( (0, 0), source.Size, (0, 0), window.Size, mask, filter );
		//DrawBuffer( DrawBufferMode.Back );
		UnbindFramebuffer( FramebufferTarget.ReadFramebuffer );
	}

}

public sealed class VertexBufferObjectService : DisposableIdentifiable, IInitializable {
	private readonly Dictionary<Type, VertexBufferObject> _vbos;

	private OglDynamicBuffer? _elementBuffer;
	private OglDynamicBuffer? _uniformBuffer;
	private OglDynamicBuffer? _shaderStorage;
	public OglStaticBuffer ElementBuffer => _elementBuffer ?? throw new NullReferenceException( "ElementBuffer not initialized" );
	public OglStaticBuffer UniformBuffer => _uniformBuffer ?? throw new NullReferenceException( "UniformBuffer not initialized" );
	public OglStaticBuffer ShaderStorage => _shaderStorage ?? throw new NullReferenceException( "ShaderStorage not initialized" );

	public VertexBufferObjectService() {
		_vbos = new();
	}

	public void Initialize() {
		_elementBuffer = new( BufferUsage.DynamicDraw, 65_536u ) { Nickname = nameof(ElementBuffer)};
		_uniformBuffer = new( BufferUsage.DynamicDraw, 65_536u, 256 ) { Nickname = nameof( ElementBuffer ) };
		_shaderStorage = new( BufferUsage.DynamicDraw, 65_536u ) { Nickname = nameof( ElementBuffer ) };
	}

	public VertexBufferObject Get( Type t ) {
		if (_vbos.TryGetValue( t, out VertexBufferObject? vbo ))
			return vbo;
		vbo = new VertexBufferObject( t.Name, 65536, BufferUsage.DynamicDraw );
		_vbos[ t ] = vbo;
		return vbo;
	}

	public void Dispose() {
		ElementBuffer.Dispose();
		UniformBuffer.Dispose();
		ShaderStorage.Dispose();
		foreach (var vbo in _vbos.Values)
			vbo.Dispose();
	}
}

[Do<IInitializable>.After<VertexBufferObjectService>]
public sealed class VertexArrayLayoutService : DisposableIdentifiable, IInitializable {

	private readonly VertexBufferObjectService _vertexBufferObjectService;
	private Dictionary<Type, VertexArrayLayout> _layouts;

	public VertexArrayLayoutService( VertexBufferObjectService vertexBufferObjectService ) {
		_vertexBufferObjectService = vertexBufferObjectService ?? throw new ArgumentNullException( nameof( vertexBufferObjectService ) );
		_layouts = new();
	}

	public void Initialize() {
		var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany( p => p.GetTypes() ).Where( p => p.IsValueType );
		var layoutTypes = types.Where( p => p.GetCustomAttribute<VAO.SetupAttribute>() is not null );

		foreach (var layoutType in layoutTypes) {
			var structLayout = layoutType.StructLayoutAttribute;
			if (structLayout is null) {
				Log.Warning( $"{layoutType.Name} is missing the {nameof( StructLayoutAttribute )}!" );
				continue;
			}

			if (structLayout.Value != LayoutKind.Explicit) {
				Log.Warning( $"{layoutType.Name} has a {nameof( StructLayoutAttribute )}, but the layout is not {nameof( LayoutKind.Explicit )}!" );
				continue;
			}

			var valSetup = layoutType.GetCustomAttribute<VAO.SetupAttribute>().NotNull();

			int strideBytes = valSetup.StrideBytesOverride >= 0
				? valSetup.StrideBytesOverride
				: Marshal.SizeOf( layoutType ) + valSetup.TextureCount * sizeof( ushort );

			var vbo = _vertexBufferObjectService.Get( layoutType );

			List<VertexArrayLayoutFieldData> fields = new();

			foreach (var field in layoutType.GetFields()) {
				var data = field.GetCustomAttribute<VAO.DataAttribute>();
				if (data is null)
					continue;

				if (!field.FieldType.IsValueType) {
					Log.Warning( $"{layoutType.Name}.{field.Name} is not a value type!" );
					continue;
				}

				var fieldOffset = field.GetCustomAttribute<FieldOffsetAttribute>();
				if (fieldOffset is null) {
					Log.Warning( $"{layoutType.Name}.{field.Name} is missing the {nameof( FieldOffsetAttribute )}!" );
					continue;
				}

				uint vertices = data.VertexCount;
				int offsetBytes = data.RelativeOffsetBytesOverride >= 0 ? data.RelativeOffsetBytesOverride : fieldOffset.Value;
				uint sizePerVertex = (uint) Marshal.SizeOf( field.FieldType ) / vertices;

				if (sizePerVertex == 0)
					this.LogWarning( $"Bytesize per vertex for {layoutType.Name}.{field.Name} is {sizePerVertex}!" );
				if (sizePerVertex > 4 && data.AttributeType != VertexArrayAttributeType.LARGE)
					this.LogWarning( $"Bytesize per vertex for {layoutType.Name}.{field.Name} is {sizePerVertex} while not using the {nameof( VertexArrayAttributeType.LARGE )} {nameof( VertexArrayAttributeType )}!" );

				while (vertices > 0) {
					uint addedVertices = Math.Min( vertices, 4 );

					fields.Add( new VertexArrayLayoutFieldData( data.VertexAttributeType, addedVertices, (uint) offsetBytes, data.AttributeType, data.Normalized ) );
					offsetBytes += (int) (addedVertices * sizePerVertex);
					vertices -= addedVertices;
				}
			}

			int offset = Marshal.SizeOf( layoutType );
			for (int tex = 0; tex < valSetup.TextureCount; tex += 4) {
				int addedTextures = Math.Min( valSetup.TextureCount - tex, 4 );
				fields.Add( new VertexArrayLayoutFieldData( OpenGL.VertexAttribType.UnsignedShort, (uint) addedTextures, (uint) offset, VertexArrayAttributeType.INTEGER, false ) );
				offset += addedTextures * sizeof( ushort );
			}
			_layouts.Add( layoutType, new VertexArrayLayout( layoutType, vbo, valSetup.OffsetBytes, strideBytes, valSetup.InstanceDivisor, fields ) );
		}
	}

	public VertexArrayLayout? Get( Type t ) => _layouts.GetValueOrDefault( t );
}

public sealed class CompositeVertexArrayObjectService : Identifiable, IContextService {
	private readonly VertexBufferObjectService _vertexBufferObjectService;
	private readonly VertexArrayLayoutService _vertexArrayLayoutService;
	private readonly Dictionary<string, CompositeVertexArrayObject> _cvaos;

	public CompositeVertexArrayObjectService( VertexBufferObjectService vertexBufferObjectService, VertexArrayLayoutService vertexArrayLayoutService ) {
		_vertexBufferObjectService = vertexBufferObjectService;
		_vertexArrayLayoutService = vertexArrayLayoutService;
		_cvaos = new();
	}

	public CompositeVertexArrayObject? Get( Span<Type> vaoParts ) {
		string? id = GetIdentifyingString( vaoParts );
		if (id is null)
			return null;
		if (_cvaos.TryGetValue( id, out var cvao ))
			return cvao;
		var layouts = new VertexArrayLayout[ vaoParts.Length ];
		for (int i = 0; i < vaoParts.Length; i++) {
			var layout = _vertexArrayLayoutService.Get( vaoParts[ i ] );
			if (layout is null) {
				this.LogWarning( $"Couldn't get layout for {vaoParts[ i ]}!" );
				return null;
			}
			layouts[ i ] = layout;
		}
		cvao = new CompositeVertexArrayObject( _vertexBufferObjectService.ElementBuffer, layouts );
		_cvaos.Add( id, cvao );
		return cvao;
	}

	private static unsafe string? GetIdentifyingString( Span<Type> input ) {
		if (input.Length == 0)
			return null;
		Guid* srcPtr = stackalloc Guid[ input.Length ];
		for (int i = 0; i < input.Length; i++)
			srcPtr[ i ] = input[ i ].GUID;
		byte* dstPtr = stackalloc byte[ input.Length * sizeof( Guid ) ];
		Unsafe.CopyBlock( dstPtr, srcPtr, (uint) (input.Length * sizeof( Guid )) );
		return DataExtensions.CreateString( dstPtr, (uint) (input.Length * sizeof( Guid )) );
	}
}
