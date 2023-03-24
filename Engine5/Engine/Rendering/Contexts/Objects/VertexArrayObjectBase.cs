using OpenGL;

namespace Engine.Rendering.Contexts.Objects;

public abstract class VertexArrayObjectBase : Identifiable, IDisposable
{

    private bool _setup;
    public uint VAOID { get; private set; }
    private uint _numBufferBindings;

    protected override string UniqueNameTag => $"{VAOID}:[{_numBufferBindings}]";

    public VertexArrayObjectBase(string name) : base(name)
    {
        _numBufferBindings = 0;
        VAOID = Gl.CreateVertexArray();
	}

#if DEBUG
	~VertexArrayObjectBase()
	{
		System.Diagnostics.Debug.Fail($"{this} was not disposed!");
	}
#endif

	/// <summary>
	/// Guaranteed to be called in a context call. Direct method calls should be used inside this method, not indirect.
	/// </summary>
	protected abstract void Setup();

    #region Setup
    protected void SetBindingDivisor(uint binding, uint divisor)
        => Gl.VertexArrayBindingDivisor(VAOID, binding, divisor);
    protected void SetAttribBinding(uint binding, uint attribIndex)
        => Gl.VertexArrayAttribBinding(VAOID, attribIndex, binding);
    protected void SetAttribFormat(uint attribIndex, int size, VertexAttribType type, bool normalized, uint relativeOffset)
        => Gl.VertexArrayAttribFormat(VAOID, attribIndex, size, type, normalized, relativeOffset);
    protected void SetAttribLFormat(uint attribIndex, int size, VertexAttribType type, uint relativeOffset)
        => Gl.VertexArrayAttribLFormat(VAOID, attribIndex, size, type, relativeOffset);
    protected void SetAttribIFormat(uint attribIndex, int size, VertexAttribType type, uint relativeOffset)
        => Gl.VertexArrayAttribIFormat(VAOID, attribIndex, size, type, relativeOffset);
    protected void EnableAttrib(uint attribIndex)
        => Gl.EnableVertexArrayAttrib(VAOID, attribIndex);
    /// <summary>
    /// Binds a buffer to a the VAO.
    /// </summary>
    /// <param name="bufferId">The buffer to bind.</param>
    /// <param name="offset">The offset inside the buffer for the initial element.</param>
    /// <param name="stride">The size of each element in the buffer.</param>
    /// <returns>The buffer binding id, this id is used when setting attributes</returns>
    protected uint AddBuffer(VertexBufferObject buffer, ulong offset, int stride) => AddBuffer(buffer.BufferId, offset, stride);

    protected uint AddBuffer(uint bufferId, ulong offset, int stride)
    {
        uint binding = _numBufferBindings++;
        Gl.VertexArrayVertexBuffer(VAOID, binding, bufferId, (IntPtr)offset, stride);
        this.LogLine($"Added {bufferId} at offset {offset}, with stride {stride}B as binding {binding}!", Log.Level.NORMAL);
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
    protected void SetupAttrib(uint binding, uint index, int size, VertexAttribType type, bool normalized, uint relativeOffset)
    {
        SetAttribBinding(binding, index);
        SetAttribFormat(index, size, type, normalized, relativeOffset);
        EnableAttrib(index);
        this.LogLine($"Added attribute of type {type} with size {size} to index {index} at binding {binding} with offset {relativeOffset}B!{(normalized ? " [Normalized]" : "")}", Log.Level.LOW);
    }

    /// <summary>
    /// A composition of <see cref="SetAttribBinding(uint, uint)"/>, <see cref="SetAttribFormat(uint, int, VertexAttribType, bool, uint)"/> and <see cref="EnableAttrib(uint)"/>, executed in that order.
    /// </summary>
    /// <param name="binding">The binding location.</param>
    /// <param name="index">The attribute index to associate with the binding.</param>
    /// <param name="size">The size of the attribute, e.g. number of dimensions in the position of a vertex.</param>
    /// <param name="type">The type of data.</param>
    /// <param name="relativeOffset">The relative offset of the data in the buffer.</param>
    protected void SetupAttribI(uint binding, uint index, int size, VertexAttribType type, uint relativeOffset)
    {
        SetAttribBinding(binding, index);
        SetAttribIFormat(index, size, type, relativeOffset);
        EnableAttrib(index);
        this.LogLine($"Added attribute of type {type} with size {size} to index {index} at binding {binding} with offset {relativeOffset}B! [Integer]", Log.Level.LOW);
    }

    /// <summary>
    /// A composition of <see cref="SetAttribBinding(uint, uint)"/>, <see cref="SetAttribLFormat(uint, int, VertexAttribType, uint)"/> and <see cref="EnableAttrib(uint)"/>, executed in that order.
    /// </summary>
    /// <param name="binding">The binding location.</param>
    /// <param name="index">The attribute index to associate with the binding.</param>
    /// <param name="size">The size of the attribute, e.g. number of dimensions in the position of a vertex.</param>
    /// <param name="type">The type of data.</param>
    /// <param name="relativeOffset">The relative offset of the data in the buffer.</param>
    protected void SetupAttribL(uint binding, uint index, int size, VertexAttribType type, uint relativeOffset)
    {
        SetAttribBinding(binding, index);
        SetAttribLFormat(index, size, type, relativeOffset);
        EnableAttrib(index);
        this.LogLine($"Added attribute of type {type} with size {size} to index {index} at binding {binding} with offset {relativeOffset}B! [Large]", Log.Level.LOW);
    }

    /// <summary>
    /// Sets the element buffer for the VAO
    /// </summary>
    /// <param name="bufferId">The buffer ID for the element buffer.</param>
    protected void SetElementBuffer(VertexBufferObject buffer)
        => Gl.VertexArrayElementBuffer(VAOID, buffer.BufferId);

    #endregion

    public void Bind()
    {
        if (!_setup)
        {
            Setup();
            _setup = true;
        }
        Gl.BindVertexArray(VAOID);
    }

    public static void Unbind() => Gl.BindVertexArray(0);

    public void Dispose()
    {
        Gl.DeleteVertexArrays(VAOID);
        GC.SuppressFinalize(this);
    }

    //https://community.khronos.org/t/gldrawelementsindirect-with-element-array-buffer-offset/69421
}
