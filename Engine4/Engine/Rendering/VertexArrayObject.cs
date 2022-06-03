using OpenGL;

namespace Engine.Rendering;
public abstract class VertexArrayObject : DisposableIdentifiable {

	private bool _setup;
	public uint VAOID { get; private set; }
	private uint _numBufferBindings;

	private readonly List<VertexBufferBinding> _bindings;

	protected override string UniqueNameTag => $"{this.VAOID}:[{this._numBufferBindings}]";

	public VertexArrayObject( string name ) : base( name ) {
		this._numBufferBindings = 0;
		this._bindings = new List<VertexBufferBinding>();
		this.VAOID = Gl.CreateVertexArray();
	}

	/// <summary>
	/// Guaranteed to be called in a context call. Direct method calls should be used inside this method, not indirect.
	/// </summary>
	protected abstract void Setup();

	#region Setup
	protected void SetBindingDivisor( uint binding, uint divisor )
		=> Gl.VertexArrayBindingDivisor( this.VAOID, binding, divisor );
	protected void SetAttribBinding( uint binding, uint attribIndex )
		=> Gl.VertexArrayAttribBinding( this.VAOID, attribIndex, binding );
	protected void SetAttribFormat( uint attribIndex, int size, VertexAttribType type, bool normalized, uint relativeOffset )
		=> Gl.VertexArrayAttribFormat( this.VAOID, attribIndex, size, type, normalized, relativeOffset );
	protected void SetAttribLFormat( uint attribIndex, int size, VertexAttribType type, uint relativeOffset )
		=> Gl.VertexArrayAttribLFormat( this.VAOID, attribIndex, size, type, relativeOffset );
	protected void SetAttribIFormat( uint attribIndex, int size, VertexAttribType type, uint relativeOffset )
		=> Gl.VertexArrayAttribIFormat( this.VAOID, attribIndex, size, type, relativeOffset );
	protected void EnableAttrib( uint attribIndex )
		=> Gl.EnableVertexArrayAttrib( this.VAOID, attribIndex );
	/// <summary>
	/// Binds a buffer to a the VAO.
	/// </summary>
	/// <param name="bufferId">The buffer to bind.</param>
	/// <param name="offset">The offset inside the buffer for the initial element.</param>
	/// <param name="stride">The size of each element in the buffer.</param>
	/// <returns>The buffer binding id, this id is used when setting attributes</returns>
	protected uint AddBuffer( VertexBufferObject buffer, ulong offset, int stride ) {
		uint binding = this._numBufferBindings++;
		this._bindings.Add( new VertexBufferBinding( this, buffer, binding, offset, stride ) );
		this.LogLine( $"Added {buffer} at offset {offset}, with stride {stride}B as binding {binding}!", Log.Level.NORMAL );
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
		this.LogLine( $"Added attribute of type {type} with size {size} to index {index} at binding {binding} with offset {relativeOffset}B!{( normalized ? " [Normalized]" : "" )}", Log.Level.LOW );
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
		this.LogLine( $"Added attribute of type {type} with size {size} to index {index} at binding {binding} with offset {relativeOffset}B! [Integer]", Log.Level.LOW );
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
		this.LogLine( $"Added attribute of type {type} with size {size} to index {index} at binding {binding} with offset {relativeOffset}B! [Large]", Log.Level.LOW );
	}

	/// <summary>
	/// Sets the element buffer for the VAO
	/// </summary>
	/// <param name="bufferId">The buffer ID for the element buffer.</param>
	protected void SetElementBuffer( VertexBufferObject buffer )
		=> Gl.VertexArrayElementBuffer( this.VAOID, buffer.BufferId );

	private void SetBuffer( uint binding, uint bufferId, ulong offset, int stride ) {
		Gl.VertexArrayVertexBuffer( this.VAOID, binding, bufferId, (IntPtr) offset, stride );
		this.LogLine( $"Set buffer #{bufferId} to binding {binding}!", Log.Level.LOW );
	}
	#endregion

	public void DirectBind() {
		if ( !Resources.Render.InThread ) {
			this.LogError( "Can't be bound outside context thread." );
			return;
		}
		if ( !this._setup ) {
			Setup();
			this._setup = true;
		}
		Gl.BindVertexArray( this.VAOID );
	}

	public static void DirectUnbind() => Gl.BindVertexArray( 0 );

	protected override bool OnDispose() {
		foreach ( VertexBufferBinding? vbBinding in this._bindings )
			vbBinding.Dispose();
		Gl.DeleteVertexArrays( this.VAOID );
		return true;
	}

	public class VertexBufferBinding : DisposableIdentifiable {
		private readonly VertexArrayObject _vertexArrayObject;
		private readonly VertexBufferObject _buffer;
		private readonly uint _binding;
		private readonly ulong _offset;
		private readonly int _stride;

		public VertexBufferBinding( VertexArrayObject vertexArrayObject, VertexBufferObject buffer, uint binding, ulong offset, int stride ) {
			this._vertexArrayObject = vertexArrayObject;
			this._buffer = buffer;
			this._binding = binding;
			this._offset = offset;
			this._stride = stride;

			this._buffer.BufferIdSet += BufferIdSet;
			if ( this._buffer.BufferId != 0 ) {
				this._vertexArrayObject.SetBuffer( this._binding, this._buffer.BufferId, this._offset, this._stride );
			}
		}

		private void BufferIdSet() => this._vertexArrayObject.SetBuffer( this._binding, this._buffer.BufferId, this._offset, this._stride );

		protected override bool OnDispose() {
			this._buffer.BufferIdSet -= BufferIdSet;
			return true;
		}
	}

	//https://community.khronos.org/t/gldrawelementsindirect-with-element-array-buffer-offset/69421
}
