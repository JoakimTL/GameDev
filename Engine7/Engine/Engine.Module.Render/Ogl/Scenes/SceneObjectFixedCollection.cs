using Engine.Buffers;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using System.Runtime.CompilerServices;

namespace Engine.Module.Render.Ogl.Scenes;

/// <summary>
/// Allows for manipulation of multiple instances with the same mesh and shader.
public sealed class SceneObjectFixedCollection<TVertexData, TInstanceData> : IndirectCommandProviderBase, IRemovable
	where TVertexData : unmanaged
	where TInstanceData : unmanaged {

	public IMesh Mesh { get; }
	public uint ActiveElements { get; private set; }
	public uint MaxElements { get; }
	private readonly uint _instanceSizeBytes;
	private readonly BufferSegment _segment;

	public bool Removed { get; private set; }
	public event RemovalHandler? OnRemoved;

	public uint BaseInstance => (uint) (this._segment.OffsetBytes / this._instanceSizeBytes);

	internal SceneObjectFixedCollection( uint renderLayer, OglVertexArrayObjectBase vertexArrayObject, ShaderBundleBase shaderBundle, IMesh mesh, BufferSegment segment ) : base( renderLayer, vertexArrayObject, shaderBundle ) {
		this.Mesh = mesh;
		_instanceSizeBytes = (uint) Unsafe.SizeOf<TInstanceData>();
		this._segment = segment;
		MaxElements = (uint) (segment.LengthBytes / _instanceSizeBytes);
		ActiveElements = 0;
	}

	public void SetActiveElements( uint count ) {
		if (count == ActiveElements)
			return;
		if (count > MaxElements)
			throw new ArgumentException( "Count is greater than the maximum number of elements" );
		ActiveElements = count;
		InvokeChanged();
	}

	public override void AddIndirectCommands( List<IndirectCommand> commandList ) {
		if (ActiveElements == 0)
			return;
		commandList.Add( new( Mesh.ElementCount, ActiveElements, Mesh.ElementOffset, Mesh.VertexOffset, BaseInstance ) );
	}

	public void Write( uint elementOffset, TInstanceData data ) {
		uint offsetBytes = elementOffset * _instanceSizeBytes;
		if (offsetBytes + _instanceSizeBytes > _segment.LengthBytes)
			throw new ArgumentException( "Element offset is out of bounds" );
		_segment.Write( elementOffset, data );
	}

	public void WriteRange( uint elementOffset, Span<TInstanceData> data ) {
		uint offsetBytes = elementOffset * _instanceSizeBytes;
		if (offsetBytes + _instanceSizeBytes > _segment.LengthBytes)
			throw new ArgumentException( "Element offset is out of bounds" );
		_segment.WriteRange( data, offsetBytes );
	}

	public TInstanceData Read( uint elementOffset ) {
		uint offsetBytes = elementOffset * _instanceSizeBytes;
		if (offsetBytes + _instanceSizeBytes > _segment.LengthBytes)
			throw new ArgumentException( "Element offset is out of bounds" );
		return _segment.Read( offsetBytes, out TInstanceData data ) ? data : throw new ArgumentException( "Element offset is out of bounds" );
	}

	public void Remove() {
		Removed = true;
		OnRemoved?.Invoke( this );
	}

	protected override bool InternalDispose() {
		return true;
	}
}