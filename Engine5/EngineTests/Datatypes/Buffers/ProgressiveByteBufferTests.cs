using Engine.Datatypes.Buffers;

namespace EngineTests.Datatypes.Buffers;

[TestFixture]
public class ProgressiveByteBufferTests {

    [Test]
    public void Flush_SingleAdd_Expecting512Bytes() {
        ProgressiveByteBuffer buffer = new();

        buffer.Add( new byte[ 512 ], 512 );
        var output = buffer.Flush( 0 );

        Assert.That( output, Has.Length.EqualTo( 512 ) );
    }

    [Test]
    public void Flush_MultipleAdd_Expecting512Bytes() {
        ProgressiveByteBuffer buffer = new();

        buffer.Add( new byte[ 256 ], 256 );
        buffer.Add( new byte[ 256 ], 256 );
        var output = buffer.Flush( 0 );

        Assert.That( output, Has.Length.EqualTo( 512 ) );
    }

    [Test]
    public void FlushWithOffset_SingleAdd_Expecting508Bytes() {
        ProgressiveByteBuffer buffer = new();

        buffer.Add( new byte[ 512 ], 512 );
        var output = buffer.Flush( 4 );

        Assert.That( output, Has.Length.EqualTo( 508 ) );
    }

    [Test]
    public void FlushWithOffset_MultipleAdd_Expecting508Bytes() {
        ProgressiveByteBuffer buffer = new();

        buffer.Add( new byte[ 256 ], 256 );
        buffer.Add( new byte[ 256 ], 256 );
        var output = buffer.Flush( 4 );

        Assert.That( output, Has.Length.EqualTo( 508 ) );
    }



}
