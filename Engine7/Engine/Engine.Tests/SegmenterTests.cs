using Engine.Algorithms;
using System.Runtime.InteropServices;

namespace Engine.Tests;

[TestFixture]
public class SegmenterTests {

	[Test]
	public void SegmentDesegmentTest() {
		Guid guidA = Guid.NewGuid();
		Guid guidB = Guid.NewGuid();

		Span<byte> guids = stackalloc byte[ 32 ];
		MemoryMarshal.Write( guids, guidA );
		MemoryMarshal.Write( guids[ 16.. ], guidB );
		Segmenter segmenter = new( guids );
		ReadOnlySpan<char> loremIpsumString = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.".AsSpan();

		ReadOnlySpan<byte> loremIpsum = MemoryMarshal.AsBytes( loremIpsumString );
		segmenter.Append( loremIpsum );

		var result = segmenter.Flush( "unit-test" );

		Desegmenter desegmenter = new();

		var spanLength = Segmentation.DetermineSpanLength( result.Payload.Span );

		Assert.That( spanLength, Is.EqualTo( loremIpsum.Length ) );

		Span<byte> output = stackalloc byte[ spanLength ];

		desegmenter.ReadInto( result.Payload.Span, output, out int readBytes );

		Assert.That( readBytes, Is.EqualTo( guids.Length ) );

		Guid firstGuid = MemoryMarshal.Read<Guid>( output[ ..16 ] );
		Guid secondGuid = MemoryMarshal.Read<Guid>( output[ 16.. ] );
		Assert.That( firstGuid, Is.EqualTo( guidA ) );
		Assert.That( secondGuid, Is.EqualTo( guidB ) );

		desegmenter.ReadInto( result.Payload.Span, output, out readBytes );

		Assert.That( readBytes, Is.EqualTo( loremIpsum.Length ) );
		string loremIpsumResult;
		unsafe {
			fixed (byte* srcPtr = output) {
				loremIpsumResult = new string( (char*) srcPtr, 0, loremIpsumString.Length );
			}
		}

		Assert.That( loremIpsumResult, Is.EqualTo( loremIpsumString.ToString() ) );


	}

}