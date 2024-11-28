using Engine.Module.Render.Ogl.OOP.Buffers;
using OpenGL;

namespace Engine.Module.Render.Ogl.OOP.VertexArrays;

public abstract class OglVertexArrayObjectBase : DisposableIdentifiable {

	private bool _setup;
	protected internal readonly uint VertexArrayId;
	private readonly Dictionary<uint, uint> _bufferBindings;

	protected OglBufferBase? ElementBuffer { get; private set; }

	public OglVertexArrayObjectBase() {
		this.VertexArrayId = Gl.CreateVertexArray();
		this.ElementBuffer = null;
		this._bufferBindings = [];
		this._setup = false;
		this.Nickname = $"VAO{this.VertexArrayId}";
	}

	protected void SetBindingDivisor( uint binding, uint divisor ) {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		if (binding >= this._bufferBindings.Count)
			throw new OpenGlArgumentException( "Binding index is out of range", nameof( binding ) );
		Gl.VertexArrayBindingDivisor( this.VertexArrayId, binding, divisor );
	}

	protected void SetAttribBinding( uint binding, uint attribIndex ) {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		Gl.VertexArrayAttribBinding( this.VertexArrayId, attribIndex, binding );
	}

	protected void SetAttribFormat( uint attribIndex, int size, VertexAttribType type, bool normalized, uint relativeOffset ) {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		Gl.VertexArrayAttribFormat( this.VertexArrayId, attribIndex, size, type, normalized, relativeOffset );
	}

	protected void SetAttribLFormat( uint attribIndex, int size, VertexAttribType type, uint relativeOffset ) {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		Gl.VertexArrayAttribLFormat( this.VertexArrayId, attribIndex, size, type, relativeOffset );
	}

	protected void SetAttribIFormat( uint attribIndex, int size, VertexAttribType type, uint relativeOffset ) {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		Gl.VertexArrayAttribIFormat( this.VertexArrayId, attribIndex, size, type, relativeOffset );
	}

	protected void EnableAttrib( uint attribIndex ) {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		Gl.EnableVertexArrayAttrib( this.VertexArrayId, attribIndex );
	}

	protected uint BindBuffer( uint buffer, uint offsetBytes, int strideBytes ) {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		if (this._bufferBindings.TryGetValue( buffer, out uint binding ))
			return binding;
		binding = (uint) this._bufferBindings.Count;
		//if (binding >= Gl.GetInteger<int>(GetPName.MaxVertexAttribBindings)
		//	throw new OpenGlArgumentException( "Too many buffer bindings", nameof( buffer ) );
		Gl.VertexArrayVertexBuffer( this.VertexArrayId, binding, buffer, (nint) offsetBytes, strideBytes );
		this._bufferBindings.Add( buffer, binding );
		return binding;
	}

	/// <summary>
	/// A composition of <see cref="SetAttribBinding(uint, uint)"/>, <see cref="SetAttribFormat(uint, int, VertexAttribType, bool, uint)"/> and <see cref="EnableAttrib(uint)"/>, executed in that order.
	/// </summary>
	/// <param name="binding">The binding location.</param>
	/// <param name="index">The attribute index to associate with the binding.</param>
	/// <param name="size">The size of the attribute, e.g. number of dimensions in the position of a vertex.</param>
	/// <param name="type">The type of data.</param>
	/// <param name="normalized">If the data is normalized. If bytes are used to represent colors this is useful.</param>
	/// <param name="relativeOffset">The relative offset of the data in the buffer.</param>
	protected void SetupAttrib( uint binding, uint index, int size, VertexAttribType type, bool normalized, uint relativeOffset ) {
		SetAttribBinding( binding, index );
		SetAttribFormat( index, size, type, normalized, relativeOffset );
		EnableAttrib( index );
	}

	/// <summary>
	/// A composition of <see cref="SetAttribBinding(uint, uint)"/>, <see cref="SetAttribFormat(uint, int, VertexAttribType, bool, uint)"/> and <see cref="EnableAttrib(uint)"/>, executed in that order.
	/// </summary>
	/// <param name="binding">The binding location.</param>
	/// <param name="index">The attribute index to associate with the binding.</param>
	/// <param name="size">The size of the attribute, e.g. number of dimensions in the position of a vertex.</param>
	/// <param name="type">The type of data.</param>
	/// <param name="relativeOffset">The relative offset of the data in the buffer.</param>
	protected void SetupAttribI( uint binding, uint index, int size, VertexAttribType type, uint relativeOffset ) {
		SetAttribBinding( binding, index );
		SetAttribIFormat( index, size, type, relativeOffset );
		EnableAttrib( index );
	}

	/// <summary>
	/// A composition of <see cref="SetAttribBinding(uint, uint)"/>, <see cref="SetAttribLFormat(uint, int, VertexAttribType, uint)"/> and <see cref="EnableAttrib(uint)"/>, executed in that order.
	/// </summary>
	/// <param name="binding">The binding location.</param>
	/// <param name="index">The attribute index to associate with the binding.</param>
	/// <param name="size">The size of the attribute, e.g. number of dimensions in the position of a vertex.</param>
	/// <param name="type">The type of data.</param>
	/// <param name="relativeOffset">The relative offset of the data in the buffer.</param>
	protected void SetupAttribL( uint binding, uint index, int size, VertexAttribType type, uint relativeOffset ) {
		SetAttribBinding( binding, index );
		SetAttribLFormat( index, size, type, relativeOffset );
		EnableAttrib( index );
	}

	/// <summary>
	/// Sets the element buffer for the VAO
	/// </summary>
	/// <param name="bufferId">The buffer ID for the element buffer.</param>
	protected void SetElementBuffer( OglBufferBase buffer ) {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		this.ElementBuffer = buffer;
		Gl.VertexArrayElementBuffer( this.VertexArrayId, buffer.BufferId );
	}

	public uint? GetBufferBinding( uint buffer ) {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		return this._bufferBindings.TryGetValue( buffer, out uint binding ) ? binding : null;
	}

	protected abstract void Setup();

	public void Bind() {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		if (!this._setup) {
			Setup();
			this._setup = true;
		}
		Gl.BindVertexArray( this.VertexArrayId );
	}

	public static void Unbind() => Gl.BindVertexArray( 0 );

	protected override bool InternalDispose() {
		Gl.DeleteVertexArrays( this.VertexArrayId );
		return true;
	}
}
