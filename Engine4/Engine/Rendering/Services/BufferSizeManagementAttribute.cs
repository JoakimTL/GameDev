namespace Engine.Rendering.Services;

[AttributeUsage( AttributeTargets.Struct )]
public class BufferSizeManagementAttribute : Attribute {
	public uint InitialSizeBytes { get; private set; }
	public uint ExpansionAmountBytes { get; private set; }

	public BufferSizeManagementAttribute( uint initialSizeBytes, uint expansionAmountBytes ) {
		this.InitialSizeBytes = initialSizeBytes;
		this.ExpansionAmountBytes = expansionAmountBytes;
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
