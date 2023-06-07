using Engine;
using Engine.Datatypes;

namespace EngineTests.Datatypes;

[TestFixture]
public class SegmentationTests {

	//Test Segmentation then parsing working correctly
	[Test]
	public void SegmentThenParse_SegmentTestData_Returns1SegmentWithCorrectData() {
		byte[] data = new byte[ 1000 ];
		for ( int i = 0; i < data.Length; i++ )
			data[ i ] = (byte) ( i % byte.MaxValue );
		var segment = Segmentation.Segment( data );
		byte[][] parsed = Segmentation.ParseOrThrow( segment );
		Assert.That( parsed.Length, Is.EqualTo( 1 ) );
		Assert.That( parsed[ 0 ], Is.EqualTo( data ) );
	}
	//Same as above, but with padding

	[Test]
	public void SegmentThenParse_SegmentTestDataWithPadding_Returns1SegmentWithCorrectData() {
		byte[] data = new byte[ 1000 ];
		for ( int i = 0; i < data.Length; i++ )
			data[ i ] = (byte) ( i % byte.MaxValue );
		var segment = Segmentation.SegmentWithPadding( 100, 100, new[] { data } );
		byte[][] parsed = Segmentation.ParseOrThrow( segment, 100 );
		Assert.That( parsed.Length, Is.EqualTo( 1 ) );
		Assert.That( parsed[ 0 ], Is.EqualTo( data ) );
	}

	[Test]
	public void SegmentThenParse_SegmentTestDataWithPadding_ParsesCorrectlyButInputOffsetIsWrong_Returns0Segments() {
		byte[] data = new byte[ 1000 ];
		for ( int i = 0; i < data.Length; i++ )
			data[ i ] = (byte) ( i % byte.MaxValue );
		var segment = Segmentation.SegmentWithPadding( 100, 100, new[] { data } );
		byte[][] parsed = Segmentation.ParseOrThrow( segment );
		Assert.That( parsed.Length, Is.EqualTo( 0 ) );
	}

	//Same as all above, but with multiple segments
	[Test]
	public void SegmentThenParse_SegmentTestDataWithPadding_ReturnsMultipleSegmentsWithCorrectData() {
		byte[] data = new byte[ 1000 ];
		for ( int i = 0; i < data.Length; i++ )
			data[ i ] = (byte) ( i % byte.MaxValue );
		var segment = Segmentation.SegmentWithPadding( 100, 100, new[] { data, data, data } );
		byte[][] parsed = Segmentation.ParseOrThrow( segment, 100 );
		Assert.That( parsed.Length, Is.EqualTo( 3 ) );
		Assert.That( parsed[ 0 ], Is.EqualTo( data ) );
		Assert.That( parsed[ 1 ], Is.EqualTo( data ) );
		Assert.That( parsed[ 2 ], Is.EqualTo( data ) );
	}

	//Same as above, but with multiple different segments
	[Test]
	public void SegmentThenParse_SegmentTestDifferentiatingDataWithPadding_ReturnsMultipleSegmentsWithCorrectData() {
		byte[] data = new byte[ 1000 ];
		for ( int i = 0; i < data.Length; i++ )
			data[ i ] = (byte) ( i % byte.MaxValue );
		byte[] data2 = new byte[ 1000 ];
		for ( int i = 0; i < data2.Length; i++ )
			data2[ i ] = (byte) ( ( i + 1 ) % byte.MaxValue );
		var segment = Segmentation.SegmentWithPadding( 100, 100, new[] { data, data2, data } );
		byte[][] parsed = Segmentation.ParseOrThrow( segment, 100 );
		Assert.That( parsed.Length, Is.EqualTo( 3 ) );
		Assert.That( parsed[ 0 ], Is.EqualTo( data ) );
		Assert.That( parsed[ 1 ], Is.EqualTo( data2 ) );
		Assert.That( parsed[ 2 ], Is.EqualTo( data ) );
	}

	//Same as above, but data contains string converted to bytes, with null terminators and other attempts to break it early
	[Test]
	public void SegmentThenParse_SegmentTestStringDataWithPadding_ReturnsMultipleSegmentsWithCorrectData() {
		//string data = "Hello World";
		//string data2 = "Hello World2";
		//string data3 = "Hello World3";
		//Redo commented strings above, but with more complex data using string interpolation
		string data = $"Hello World{char.MinValue}";
		string data2 = $"Hello World2\r\n";
		string data3 = $"Hello World3\t\t\n\n\r\r{(char)0}{Math.PI}";
		var segment = Segmentation.SegmentWithPadding( 100, 100, new[] { data.ToBytes(), data2.ToBytes(), data3.ToBytes() } );
		byte[][] parsed = Segmentation.ParseOrThrow( segment, 100 );
		Assert.That( parsed.Length, Is.EqualTo( 3 ) );
		Assert.That( parsed[ 0 ].CreateStringOrThrow(), Is.EqualTo( data ) );
		Assert.That( parsed[ 1 ].CreateStringOrThrow(), Is.EqualTo( data2 ) );
		Assert.That( parsed[ 2 ].CreateStringOrThrow(), Is.EqualTo( data3 ) );
	}

	//Now test cases where Segmentation.ParseOrThrow should throw exceptions
	[Test]
	public void ParseOrThrow_ParseBogusData_ThrowsException() {
		byte[] data = new byte[ 1000 ];
		for ( int i = 0; i < data.Length; i++ )
			data[ i ] = (byte) ( i % byte.MaxValue );
		byte[] segment = Segmentation.Segment( data );
		segment[ 0 ] = 0b1111;
		Assert.Throws<InvalidOperationException>( () => Segmentation.ParseOrThrow( segment ) );
	}

	[Test]
	public void ParseOrThrow_ParseBogusDataWithPadding_ThrowsException() {
		byte[] data = new byte[ 1000 ];
		for ( int i = 0; i < data.Length; i++ )
			data[ i ] = (byte) ( i % byte.MaxValue );
		byte[] segment = Segmentation.SegmentWithPadding( 100, 100, new[] { data } );
		segment[ 100 ] = 0b1111;
		Assert.Throws<InvalidOperationException>( () => Segmentation.ParseOrThrow( segment, 100 ) );
	}

	[Test]
	public void ParseOrThrow_ParseOutsideDataUsingOffsetLargerThanSize_ThrowsException() {
		byte[] data = new byte[ 100 ];
		for ( int i = 0; i < data.Length; i++ )
			data[ i ] = (byte) ( i % byte.MaxValue );
		byte[] segment = Segmentation.Segment( data );
		Assert.Throws<ArgumentOutOfRangeException>( () => Segmentation.ParseOrThrow( segment, 200 ) );
	}

}
