﻿using OpenGL;

namespace Engine.Module.Render.OpenGL.Ogl.OOP;

public abstract class OglVertexArrayObjectBase : DisposableIdentifiable {

	private bool _setup;
	protected internal readonly uint VertexArrayId;
	private readonly Dictionary<uint, uint> _bufferBindings;

	protected override string ExtraInformation => $"VAO{VertexArrayId}";

	protected OglBufferBase? ElementBuffer { get; private set; }

	public OglVertexArrayObjectBase() {
		VertexArrayId = Gl.CreateVertexArray();
		ElementBuffer = null;
		_bufferBindings = [];
		_setup = false;
	}

	protected void SetBindingDivisor( uint binding, uint divisor ) {
		if (Disposed)
			throw new ObjectDisposedException( FullName );
		if (binding >= _bufferBindings.Count)
			throw new OpenGlArgumentException( "Binding index is out of range", nameof( binding ) );
		Gl.VertexArrayBindingDivisor( VertexArrayId, binding, divisor );
	}

	protected void SetAttribBinding( uint binding, uint attribIndex ) {
		if (Disposed)
			throw new ObjectDisposedException( FullName );
		Gl.VertexArrayAttribBinding( VertexArrayId, attribIndex, binding );
	}

	protected void SetAttribFormat( uint attribIndex, int size, VertexAttribType type, bool normalized, uint relativeOffset ) {
		if (Disposed)
			throw new ObjectDisposedException( FullName );
		Gl.VertexArrayAttribFormat( VertexArrayId, attribIndex, size, type, normalized, relativeOffset );
	}

	protected void SetAttribLFormat( uint attribIndex, int size, VertexAttribType type, uint relativeOffset ) {
		if (Disposed)
			throw new ObjectDisposedException( FullName );
		Gl.VertexArrayAttribLFormat( VertexArrayId, attribIndex, size, type, relativeOffset );
	}

	protected void SetAttribIFormat( uint attribIndex, int size, VertexAttribType type, uint relativeOffset ) {
		if (Disposed)
			throw new ObjectDisposedException( FullName );
		Gl.VertexArrayAttribIFormat( VertexArrayId, attribIndex, size, type, relativeOffset );
	}

	protected void EnableAttrib( uint attribIndex ) {
		if (Disposed)
			throw new ObjectDisposedException( FullName );
		Gl.EnableVertexArrayAttrib( VertexArrayId, attribIndex );
	}

	protected uint BindBuffer( uint buffer, uint offsetBytes, int strideBytes ) {
		if (Disposed)
			throw new ObjectDisposedException( FullName );
		if (_bufferBindings.TryGetValue( buffer, out uint binding ))
			return binding;
		binding = (uint) _bufferBindings.Count;
		//if (binding >= Gl.GetInteger<int>(GetPName.MaxVertexAttribBindings)
		//	throw new OpenGlArgumentException( "Too many buffer bindings", nameof( buffer ) );
		Gl.VertexArrayVertexBuffer( VertexArrayId, binding, buffer, (nint) offsetBytes, strideBytes );
		_bufferBindings.Add( buffer, binding );
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
	internal void SetElementBuffer( OglBufferBase buffer ) {
		if (Disposed)
			throw new ObjectDisposedException( FullName );
		ElementBuffer = buffer;
		Gl.VertexArrayElementBuffer( VertexArrayId, buffer.BufferId );
	}

	public uint? GetBufferBinding( uint buffer ) {
		if (Disposed)
			throw new ObjectDisposedException( FullName );
		return _bufferBindings.TryGetValue( buffer, out uint binding ) ? binding : null;
	}

	protected abstract void Setup();

	internal void Bind() {
		if (!_setup) {
			Setup();
			_setup = true;
		}
		Gl.BindVertexArray( VertexArrayId );
	}

	internal static void Unbind() => Gl.BindVertexArray( 0 );

	protected override bool InternalDispose() {
		Gl.DeleteVertexArrays( VertexArrayId );
		return true;
	}
}
