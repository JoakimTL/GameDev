using Engine.Data;

namespace Engine.Tests.Data;

[TestFixture]
public sealed class ReadOnlyUnmanagedStackTests {

	[Test]
	public void TryRead() {
		ReadOnlyUnmanagedStack stack = new( 64 );
		if (!stack.TryAllocate( stackalloc[] { 16, 32, 64, 128, 256, 512, 1024, 2048 }, out IReadableSegment? segment )) {
			Assert.Fail( "Failed to allocate segment." );
			return;
		}
		bool success;
		success = segment.TryRead( 0, out int result );
		Assert.That( success, Is.True );
		Assert.That( result, Is.EqualTo( 16 ) );

		success = segment.TryRead( 4, out result );
		Assert.That( success, Is.True );
		Assert.That( result, Is.EqualTo( 32 ) );

		success = segment.TryRead( 8, out result );
		Assert.That( success, Is.True );
		Assert.That( result, Is.EqualTo( 64 ) );

		success = segment.TryRead( 12, out result );
		Assert.That( success, Is.True );
		Assert.That( result, Is.EqualTo( 128 ) );

		segment.Dispose();

		success = segment.TryRead( 0, out result );
		Assert.That( success, Is.False );
		Assert.That( result, Is.EqualTo( 0 ) );
	}

	[Test]
	public void Dispose() {
		ReadOnlyUnmanagedStack stack = new( 64 );
		if (!stack.TryAllocate( stackalloc[] { 16, 32, 64, 128, 256, 512, 1024, 2048 }, out IReadableSegment? segment )) {
			Assert.Fail( "Failed to allocate segment." );
			return;
		}
		stack.Dispose();
		Assert.That( stack.Disposed, Is.True );

		bool success = segment.TryRead( 0, out int result );
		Assert.That( success, Is.False );
		Assert.That( result, Is.EqualTo( 0 ) );
	}

	[Test]
	public void AllocationFailed() {
		ReadOnlyUnmanagedStack stack = new( 32 );
		if (!stack.TryAllocate( stackalloc[] { 16, 32, 64, 128, 256, 512, 1024, 2048 }, out IReadableSegment? segment )) {
			Assert.Fail( "Failed to allocate segment." );
			return;
		}
		bool success = stack.TryAllocate( stackalloc[] { 16, 32, 64, 128, 256, 512, 1024, 2048 }, out segment );
		Assert.That( success, Is.False );
		Assert.That( segment, Is.Null );
	}

	[Test]
	public void MultipleAllocationsCorrectOffset() {
		ReadOnlyUnmanagedStack stack = new( 32 );
		if (!stack.TryAllocate( stackalloc byte[] { 16, 42, 69 }, out IReadableSegment? segment )) {
			Assert.Fail( "Failed to allocate segment." );
			return;
		}
		if (!stack.TryAllocate( stackalloc int[] { 16, 42, 69 }, out IReadableSegment? segment2 )) {
			Assert.Fail( "Failed to allocate segment." );
			return;
		}
		Assert.That( segment.OffsetBytes, Is.EqualTo( (nuint) 0 ) );
		Assert.That( segment.SizeBytes, Is.EqualTo( 3u ) );
		Assert.That( segment2.OffsetBytes, Is.EqualTo( (nuint) 3 ) );
		Assert.That( segment2.SizeBytes, Is.EqualTo( 12u ) );

		segment.Dispose();

		if (!stack.TryAllocate( stackalloc byte[] { 16, 42, 69 }, out IReadableSegment? segment3 )) {
			Assert.Fail( "Failed to allocate segment." );
			return;
		}
		Assert.That( segment3.OffsetBytes, Is.EqualTo( (nuint) 15 ) );
		Assert.That( segment3.SizeBytes, Is.EqualTo( 3u ) );
	}

	[Test]
	public void TryReadSpanStack() {
		ReadOnlyUnmanagedStack stack = new( 64 );
		if (!stack.TryAllocate( stackalloc[] { 16, 32, 64, 128, 256, 512, 1024, 2048 }, out IReadableSegment? segment )) {
			Assert.Fail( "Failed to allocate segment." );
			return;
		}
		Span<int> result = stackalloc int[ 3 ];
		bool success = stack.TryRead( 4, result );
		Assert.That( success, Is.True );
		Assert.That( result[ 0 ], Is.EqualTo( 32 ) );
		Assert.That( result[ 1 ], Is.EqualTo( 64 ) );
		Assert.That( result[ 2 ], Is.EqualTo( 128 ) );
	}

	[Test]
	public void TryReadSpanSegment() {
		ReadOnlyUnmanagedStack stack = new( 64 );
		if (!stack.TryAllocate( stackalloc[] { 16, 32, 64, 128, 256, 512, 1024, 2048 }, out IReadableSegment? segment )) {
			Assert.Fail( "Failed to allocate segment." );
			return;
		}
		Span<int> result = stackalloc int[ 2 ];
		bool success = segment.TryRead( 0, result );
		Assert.That( success, Is.True );
		Assert.That( result[ 0 ], Is.EqualTo( 16 ) );
		Assert.That( result[ 1 ], Is.EqualTo( 32 ) );
	}

	public void ToArray() {
		ReadOnlyUnmanagedStack stack = new( 64 );
		if (!stack.TryAllocate( stackalloc[] { 16, 32, 64, 128, 256, 512, 1024, 2048 }, out IReadableSegment? segment )) {
			Assert.Fail( "Failed to allocate segment." );
			return;
		}
		int[] arr = stack.ToArray<int>( 4, 16 );
		Assert.That( arr[ 0 ], Is.EqualTo( 32 ) );
		Assert.That( arr[ 1 ], Is.EqualTo( 64 ) );
		Assert.That( arr[ 2 ], Is.EqualTo( 128 ) );
		Assert.That( arr[ 3 ], Is.EqualTo( 256 ) );
	}
}