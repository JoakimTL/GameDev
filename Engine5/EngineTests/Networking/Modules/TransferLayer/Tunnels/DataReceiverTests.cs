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

}
