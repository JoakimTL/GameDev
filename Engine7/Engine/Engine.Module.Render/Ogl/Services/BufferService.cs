using Engine.Buffers;
using Engine.Module.Render.Ogl.OOP.Buffers;
using Engine.Processing;

namespace Engine.Module.Render.Ogl.Services;

/// <summary>
/// Handles system buffers which will be synchronized with OpenGL buffers. If you need to change anything in a buffer, use this sevices' buffers.
/// </summary>
[Do<IInitializable>.After<OglBufferService>]
[Do<IUpdateable>.Last]
public sealed class BufferService : DisposableIdentifiable, IInitializable, IUpdateable {
	private readonly OglBufferService _oglBufferService;

	private readonly Dictionary<Type, SegmentedSystemBuffer> _buffers;
	private readonly Dictionary<Type, HostClientSynchronizedBufferPair<SegmentedSystemBuffer>> _synchronizers;
	private readonly HostClientSynchronizedBufferPair<SegmentedSystemBuffer> _elementBufferSynchronizer;

	public SegmentedSystemBuffer ElementBuffer { get; }

	public BufferService( OglBufferService oglBufferService ) {
		this._oglBufferService = oglBufferService;
		this._buffers = [];
		this._synchronizers = [];
		this.ElementBuffer = new( 65_536u, BufferAutoResizeMode.DOUBLE ) { Nickname = "ElementBuffer" };
		this._elementBufferSynchronizer = new( this.ElementBuffer, null );
	}

	public void Initialize() {
		_elementBufferSynchronizer.SetClientBuffer( (OglDynamicBuffer) this._oglBufferService.ElementBuffer );
	}

	public void Update( double time, double deltaTime ) {
		this._elementBufferSynchronizer.Synchronize();
		foreach (HostClientSynchronizedBufferPair<SegmentedSystemBuffer> synchronizer in this._synchronizers.Values)
			synchronizer.Synchronize();
	}

	public SegmentedSystemBuffer Get( Type t ) {
		if (this._buffers.TryGetValue( t, out SegmentedSystemBuffer? buffer ))
			return buffer;
		buffer = new SegmentedSystemBuffer( 65_536u, BufferAutoResizeMode.DOUBLE ) { Nickname = t.Name };
		this._synchronizers[ t ] = new( buffer, (OglDynamicBuffer) this._oglBufferService.Get( t ) );
		this._buffers[ t ] = buffer;
		return buffer;
	}

	protected override bool InternalDispose() {
		this.ElementBuffer.Dispose();
		foreach (SegmentedSystemBuffer buffer in this._buffers.Values)
			buffer.Dispose();
		this._elementBufferSynchronizer.Dispose();
		foreach (HostClientSynchronizedBufferPair<SegmentedSystemBuffer> synchronizer in this._synchronizers.Values)
			synchronizer.Dispose();
		return true;
	}
}