﻿using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine.Buffers;
public class ThreadedByteBuffer {
	[ThreadStatic]
	private static Dictionary<string, ThreadedByteBuffer>? _buffers;

	public static ThreadedByteBuffer GetBuffer( string bufferIdentity ) {
		_buffers ??= [];
		if (!_buffers.TryGetValue( bufferIdentity, out ThreadedByteBuffer? buffer ))
			_buffers[ bufferIdentity ] = buffer = new( bufferIdentity );
		buffer._internalBuffer.Clear();
		return buffer;
	}

	private readonly List<byte> _internalBuffer;

	public string Identity { get; }

	public int Count => _internalBuffer.Count;

	public ThreadedByteBuffer( string identity ) {
		_internalBuffer = [];
		this.Identity = identity;
	}

	public void Add( byte value ) => _internalBuffer.Add( value );
	public void Add<T>( T value ) where T : unmanaged {
		Span<byte> bytes = stackalloc byte[ Unsafe.SizeOf<T>() ];
		MemoryMarshal.Write( bytes, value );
		AddRange( bytes );
	}

	public void AddRange( ReadOnlySpan<byte> bytes ) => _internalBuffer.AddRange( bytes );

	/// <summary>
	/// Remember to dispose of the data after use!
	/// </summary>
	/// <returns></returns>
	public PooledBufferData GetData( bool reset = true, object? tag = null ) {
		PooledBufferData output = new( _internalBuffer, tag );
		if (reset)
			_internalBuffer.Clear();
		return output;
	}
}
