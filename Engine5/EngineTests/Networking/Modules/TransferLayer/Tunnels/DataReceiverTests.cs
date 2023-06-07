using Engine.Networking.Modules.TransferLayer.Tunnels;

namespace EngineTests.Networking.Modules.TransferLayer.Tunnels;

[TestFixture]
public class DataReceiverTests {

	[Test]
	public void DataReceivedEvent_Receive2Parts_MessageIsCorrect() {
		byte[]? output = null;

		DataReceiver receiver = new();

		receiver.MessageComplete += data => output = data;

		receiver.ReceivedData( new byte[] { 16, 0, 0, 0, 0, 0, 0, 0 }, 8 );
		receiver.ReceivedData( new byte[] { 1, 1, 2, 3, 5, 8, 13, 21 }, 8 );

		Assert.That( output, Is.Not.Null );
		Assert.That( output, Has.Length.EqualTo( 12 ) );
		CollectionAssert.IsSubsetOf( new byte[] { 1, 1, 2, 3, 5, 8, 13, 21 }, output );
	}

	[Test]
	public void DataReceivedEvent_Receive2CompleteMessagesAs4Parts_MessagesAreCorrect() {
		List<byte[]?> output = new();
		DataReceiver receiver = new();

		receiver.MessageComplete += data => output.Add( data );

		receiver.ReceivedData( new byte[] { 16, 0, 0, 0, 0, 0, 0, 0 }, 8 );
		receiver.ReceivedData( new byte[] { 1, 1, 2, 3, 5, 8, 13, 21 }, 8 );
		receiver.ReceivedData( new byte[] { 16, 0, 0, 0, 0, 0, 0, 0 }, 8 );
		receiver.ReceivedData( new byte[] { 5, 8, 13, 21, 34, 55, 89, 144 }, 8 );

		Assert.That( output, Has.Count.EqualTo( 2 ) );
		Assert.That( output[ 0 ], Is.Not.Null );
		Assert.That( output[ 0 ], Has.Length.EqualTo( 12 ) );
		CollectionAssert.IsSubsetOf( new byte[] { 1, 1, 2, 3, 5, 8, 13, 21 }, output[ 0 ] );
		Assert.That( output[ 1 ], Is.Not.Null );
		Assert.That( output[ 1 ], Has.Length.EqualTo( 12 ) );
		CollectionAssert.IsSubsetOf( new byte[] { 5, 8, 13, 21, 34, 55, 89, 144 }, output[ 1 ] );
	}
}
