using System.Runtime.InteropServices;

namespace Engine.Rendering.Services;

public class RenderDataObjectManager : ModuleService, IUpdateable {

	public RenderDataObject ElementBuffer { get; }

	public bool Active => true;

	private readonly Dictionary<Type, RenderDataObject> _rdos;

	public RenderDataObjectManager() {
		this._rdos = new Dictionary<Type, RenderDataObject>();
		this.ElementBuffer = new RenderDataObject( "Element", 65_536u );
		//Resources.Render.ContextUpdate.Add( this );
	}

	public RenderDataObject Get<T>( bool aligned = true ) where T : unmanaged => Get( typeof( T ), aligned ? (uint) Marshal.SizeOf<T>() : 1 );
	public RenderDataObject Get( Type t, uint alignment = 1 ) {
		if ( this.Disposed )
			throw new DisposedObjectException( this );
		lock ( this._rdos ) {
			if ( this._rdos.TryGetValue( t, out RenderDataObject? obj ) )
				return obj;
			uint initialSizeBytes = 8192;
			uint expansionBytes = 0;
			object[]? attribs = t.GetCustomAttributes( false ).Where( p => p.GetType() == typeof( BufferSizeManagementAttribute ) ).ToArray();
			if ( attribs.Length > 0 && attribs[ 0 ] is BufferSizeManagementAttribute attrib ) {
				initialSizeBytes = attrib.InitialSizeBytes;
				expansionBytes = attrib.ExpansionAmountBytes;
			}
			this._rdos.Add( t, obj = new RenderDataObject( t.Name, initialSizeBytes, alignment, true, expansionBytes ) );
			return obj;
		}
	}

	/// <summary>
	/// Only looks at existing RDOs, and does not try to create a new one!"
	/// </summary>
	/// <exception cref="DisposedObjectException"></exception>
	public RenderDataObject? Peek( Type t ) {
		if ( this.Disposed )
			throw new DisposedObjectException( this );
		lock ( this._rdos ) {
			if ( this._rdos.TryGetValue( t, out RenderDataObject? obj ) )
				return obj;
			return null;
		}
	}

	public void Update( float time, float deltaTime ) {
		this.ElementBuffer.UpdateVBO( Resources.Render.VBOs.ElementBuffer );
		lock ( this._rdos )
			foreach ( KeyValuePair<Type, RenderDataObject> kvp in this._rdos ) {
				if ( !kvp.Value.AutoUpdate )
					continue;
				kvp.Value.UpdateVBO( Resources.Render.VBOs.Get( kvp.Key ) );
			}
	}

	protected override bool OnDispose() {
		lock ( this._rdos ) {
			foreach ( RenderDataObject? rdo in this._rdos.Values )
				rdo.Dispose();
			this._rdos.Clear();
			this.ElementBuffer.Dispose();
		}
		return true;
	}
}
/*
public class VertexBufferDataObjectManager : DisposableIdentifiable {

	private bool _initialized;
	private VertexBufferDataObject? _elementBuffer;
	private SegmentedVertexBufferObject? _uniformBuffer;
	private SegmentedVertexBufferObject? _shaderStorageBuffer;
	public VertexBufferDataObject ElementBuffer => this._elementBuffer ?? throw new NullReferenceException( "Manager not initialized!" );
	public SegmentedVertexBufferObject UniformBuffer => this._uniformBuffer ?? throw new NullReferenceException( "Manager not initialized!" );
	public SegmentedVertexBufferObject ShaderStorage => this._shaderStorageBuffer ?? throw new NullReferenceException( "Manager not initialized!" );
	private readonly Dictionary<Type, VertexBufferDataObject> _buffers;

	public VertexBufferDataObjectManager() {
		this._buffers = new Dictionary<Type, VertexBufferDataObject>();
	}

	public void Initialize() {
		if ( this._initialized ) {
			this.LogError( "Already initialized!" );
			return;
		}
		this._initialized = true;

		Gl.GetInteger( GetPName.UniformBufferOffsetAlignment, out uint alignment );
		this._elementBuffer = new VertexBufferDataObject( "Element", 65_536u, BufferUsage.DynamicDraw, sizeof( uint ), true );
		this._uniformBuffer = new SegmentedVertexBufferObject( "Uniform", 65_536u, BufferUsage.DynamicDraw, alignment );
		this._shaderStorageBuffer = new SegmentedVertexBufferObject( "ShaderStorage", 65_536u, BufferUsage.DynamicDraw, 1 );
	}

	public VertexBufferDataObject Get<T>( bool aligned = true ) where T : unmanaged {
		if ( this.Disposed )
			throw new DisposedObjectException( this );
		lock ( this._buffers ) {
			if ( this._buffers.TryGetValue( typeof( T ), out VertexBufferDataObject? obj ) ) {
				return obj;
			}
			uint initialSizeBytes = 8192;
			uint expansionBytes = 0;
			object[]? attribs = typeof( T ).GetCustomAttributes( false ).Where( p => p.GetType() == typeof( BufferSizeManagementAttribute ) ).ToArray();
			if ( attribs.Length > 0 && attribs[ 0 ] is BufferSizeManagementAttribute attrib ) {
				initialSizeBytes = attrib.InitialSizeBytes;
				expansionBytes = attrib.ExpansionAmountBytes;
			}
			this._buffers.Add( typeof( T ), obj = new VertexBufferDataObject( typeof( T ).Name, initialSizeBytes, BufferUsage.DynamicDraw, aligned ? (uint) Marshal.SizeOf<T>() : 1, true, expansionBytes ) );
			return obj;
		}
	}

	protected override bool OnDispose() {
		lock ( this._buffers ) {
			foreach ( VertexBufferDataObject? buffer in this._buffers.Values ) {
				buffer.Dispose();
			}
			this._buffers.Clear();
			this.ElementBuffer.Dispose();
		}
		return true;
	}

	[AttributeUsage( AttributeTargets.Struct )]
	public class BufferSizeManagementAttribute : Attribute {
		public uint InitialSizeBytes { get; private set; }
		public uint ExpansionAmountBytes { get; private set; }

		public BufferSizeManagementAttribute( uint initialSizeBytes, uint expansionAmountBytes ) {
			this.InitialSizeBytes = initialSizeBytes;
			this.ExpansionAmountBytes = expansionAmountBytes;
		}
	}
}*/
