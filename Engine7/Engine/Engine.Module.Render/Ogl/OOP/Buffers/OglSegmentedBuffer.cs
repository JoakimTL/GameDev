using Engine.Buffers;
using Engine.Logging;
using OpenGL;
using System.Diagnostics.CodeAnalysis;

namespace Engine.Module.Render.Ogl.OOP.Buffers;

public unsafe class OglSegmentedBuffer( BufferUsage usage, uint initialLengthBytes, BufferAutoResizeMode autoResizeMode ) : OglBufferBase( usage, initialLengthBytes ) {
	private readonly List<OglBufferSegment> _segments = [];
	private readonly uint _initialLengthBytes = initialLengthBytes;
	private uint _currentOffsetCaret;
	private bool _fragmented;
	private readonly BufferAutoResizeMode _autoResizeMode = autoResizeMode;

	public bool TryAllocate( uint lengthBytes, [NotNullWhen( true )] out OglBufferSegment? segment ) {
		segment = null;
		uint nextCaret = this._currentOffsetCaret + lengthBytes;
		while (nextCaret > this.LengthBytes) {
			if (this._fragmented) {
				Defragment();
				nextCaret = this._currentOffsetCaret + lengthBytes;
				continue;
			}
			if (this._autoResizeMode == BufferAutoResizeMode.DISABLED)
				return false;
			AutoExtend();
		}
		segment = new( this, this._currentOffsetCaret, lengthBytes );
		this._segments.Add( segment );
		this._currentOffsetCaret = nextCaret;
		return true;
	}

	private void AutoExtend() {
		switch (this._autoResizeMode) {
			case BufferAutoResizeMode.LINEAR:
				Resize( this, this.LengthBytes + this._initialLengthBytes );
				break;
			case BufferAutoResizeMode.DOUBLE:
				Resize( this, this.LengthBytes * 2 );
				break;
		}
	}

	private void Defragment() {
		this._currentOffsetCaret = 0;
		for (int i = 0; i < this._segments.Count; i++) {
			OglBufferSegment segment = this._segments[ i ];
			if (segment.OffsetBytes > this._currentOffsetCaret) {
				//TODO: This is a really really expensive way to do this. Let's just not do anything about it for now though.
				InternalMove( this, segment.OffsetBytes, this._currentOffsetCaret, segment.LengthBytes );
				segment.SetOffsetBytes( this._currentOffsetCaret );
			}
			this._currentOffsetCaret += this._segments[ i ].LengthBytes;
		}
		this._fragmented = false;
		this.LogLine( $"Defragmented!", Log.Level.VERBOSE );
	}

	internal void Free( OglBufferSegment segment ) {
		int index = this._segments.IndexOf( segment );
		if (index == -1)
			return;
		this._segments.RemoveAt( index );
		if (index == this._segments.Count)
			this._currentOffsetCaret = segment.OffsetBytes;
		else
			this._fragmented = true;
	}

	internal bool WriteRange<T>( Span<T> source, uint destinationOffsetBytes ) where T : unmanaged {
		if (this.Disposed)
			return this.LogWarningThenReturn( "Already disposed!", false );
		if (source.Length == 0)
			return this.LogWarningThenReturn( "Cannot write 0 bytes!", false );
		uint bytesToCopy = (uint) (source.Length * sizeof( T ));
		if (destinationOffsetBytes + bytesToCopy > this.LengthBytes)
			return false;
		fixed (T* srcPtr = source)
			Write( new nint( srcPtr ), destinationOffsetBytes, bytesToCopy );
		return true;
	}

	internal bool WriteRange( void* srcPtr, uint srcLengthBytes, uint destinationOffsetBytes ) {
		if (this.Disposed)
			return this.LogWarningThenReturn( "Already disposed!", false );
		if (srcLengthBytes == 0)
			return this.LogWarningThenReturn( "Cannot write 0 bytes!", false );
		if (destinationOffsetBytes + srcLengthBytes > this.LengthBytes)
			return false;
		Write( new nint( srcPtr ), destinationOffsetBytes, srcLengthBytes );
		return true;
	}

}
