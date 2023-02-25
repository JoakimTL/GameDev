using Engine.Structure.Interfaces.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Engine.Datatypes.Buffers;

/// <summary>
/// <inheritdoc/>
/// </summary>
public class SegmentedBuffer : BufferBase {
	private readonly ulong _initialSizeBytes;
	private readonly bool _autoResize;
	private readonly List<BufferSegment> _segments;
	private bool _fragmented;
	private ulong _allocatedBytes;

	/// <inheritdoc cref="BufferBase(ulong, bool)"/>
	/// <param name="autoResize">If true the buffer will automatically increase in size. The initial size of the buffer will be padded on when automatically resizing. /></param>
	public SegmentedBuffer( string name, ulong initialSizeBytes, bool safeguardMultiThreading, bool autoResize ) : base( name, initialSizeBytes, safeguardMultiThreading ) {
		this._initialSizeBytes = initialSizeBytes;
		this._autoResize = autoResize;
		_segments = new();
		_fragmented = false;
		_allocatedBytes = 0;
	}

	public ISegmentedBufferSegment? AllocateSegment( ulong sizeBytes, int timeout = Timeout.Infinite ) {
		Lock( timeout );
		ulong newSize = _allocatedBytes + sizeBytes;
		while ( newSize > SizeBytes ) {
			if ( this._fragmented ) {
				Defragment( timeout );
				newSize = _allocatedBytes + sizeBytes;
				continue;
			}
			if ( !_autoResize ) {
				Unlock();
				return default;
			}
			Resize( newSize + _initialSizeBytes, timeout, false );
		}
		var segment = new BufferSegment( this, _allocatedBytes, sizeBytes, Write );
		_allocatedBytes = newSize;
		_segments.Add( segment );
		Unlock();
		return segment;
	}

	private void Defragment( int timeout ) {
		this._allocatedBytes = 0;
		for ( int i = 0; i < this._segments.Count; i++ ) {
			var segment = this._segments[ i ];
			if ( segment.OffsetBytes != this._allocatedBytes ) {
				Move( segment.OffsetBytes, this._allocatedBytes, segment.SizeBytes, timeout, false /*This only occurs in a locked environment!*/ );
				segment.SetOffset( this._allocatedBytes );
			}
			this._allocatedBytes += segment.SizeBytes;
		}
		this._fragmented = false;
		this.LogLine( $"Defragmented!", Log.Level.NORMAL );
	}

	private unsafe bool Write( ulong offsetBytes, nuint data, ulong sizeBytes ) => Write( offsetBytes, (void*) data, sizeBytes, 1000, true );

	internal void DisposeSegment( BufferSegment bufferSegment ) {
		_segments.Remove( bufferSegment );
		bufferSegment.SetDisposed();
		_fragmented = true;
		this.LogLine( $"Segment {bufferSegment} removed!", Log.Level.LOW );
	}
}


public class BufferWriteTracker : Identifiable {
	private readonly IListenableWriteableBuffer _buffer;

	private readonly ConcurrentQueue<ChangedSection> _incoming;
	private readonly BinaryTree<ChangedSection> _changes;

	public BufferWriteTracker( IListenableWriteableBuffer buffer ) {
		_incoming = new();
		_changes = new();
		this._buffer = buffer;
		_buffer.Written += OnWritten;
	}

	private void OnWritten( ulong offsetBytes, ulong lengthBytes ) => _incoming.Enqueue( new( offsetBytes, lengthBytes ) );

	public void GetChanges( List<ChangedSection> outputContainer ) {
		_changes.Clear();
		while ( _incoming.TryDequeue( out ChangedSection section ) )
			_changes.Add( section );

		outputContainer.Clear();
		if ( _changes.Count == 0 )
			return;

		ulong offset = _changes[ 0 ].Offset;
		ulong size = _changes[ 0 ].Size;

		for ( int i = 1; i < _changes.Count; i++ ) {
			var change = _changes[ i ];
			if ( change.Offset > offset + size + 4096 /*4KiB?*/ ) {
				outputContainer.Add( new ChangedSection( offset, size ) );
				offset = change.Offset;
				size = change.Size;
			} else {
				size = change.Offset + change.Size - offset;
			}
		}
		outputContainer.Add( new ChangedSection( offset, size ) );
	}

	public void Clear() {
		_incoming.Clear();
	}
}

public readonly struct ChangedSection : IComparable<ChangedSection> {
	public readonly ulong Offset;
	public readonly ulong Size;

	public ChangedSection( ulong offset, ulong size ) {
		this.Offset = offset;
		this.Size = size;
	}

	public int CompareTo( ChangedSection other ) => Offset.CompareTo( other.Offset );
}

public class BinaryTree<T> : IEnumerable<T> where T : IComparable<T> {

	private readonly List<T> _underlying;

	public int Count => _underlying.Count;

	public bool IsReadOnly => false;

	public T this[ int index ] {
		get => _underlying[ index ];
		set => _underlying[ index ] = value;
	}

	public BinaryTree() {
		_underlying = new List<T>();
	}

	public int IndexOf( T item ) => _underlying.IndexOf( item );

	public void RemoveAt( int index ) => _underlying.RemoveAt( index );

	public void Add( T item ) {
		if ( Count == 0 ) {
			_underlying.Add( item );
			return;
		}
		int index = FindIndex( 0, _underlying.Count, item );
		_underlying.Insert( index, item );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private int FindIndex( int start, int end, T item ) {
		int middle;
		T valueAtIndex;
		int order;
		while ( true ) {
			middle = start + ( end - start ) / 2;
			valueAtIndex = _underlying[ middle ];
			order = item.CompareTo( valueAtIndex );
			if ( order == 0 )
				return middle;
			if ( middle == start )
				return order < 0 ? start : end;
			if ( order < 0 ) {
				end = middle;
			} else {
				start = middle;
			}
		}
	}

	public void Clear() => _underlying.Clear();

	public bool Contains( T item ) => _underlying.Contains( item );

	public void CopyTo( T[] array, int arrayIndex ) => _underlying.CopyTo( array, arrayIndex );

	public bool Remove( T item ) => _underlying.Remove( item );

	public IEnumerator<T> GetEnumerator() => _underlying.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable) _underlying ).GetEnumerator();
}