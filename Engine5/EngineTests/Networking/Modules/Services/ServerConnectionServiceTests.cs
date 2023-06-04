using Engine.GlobalServices;
using Engine.Networking.Modules.Services;
using System.Net;

namespace EngineTests.Networking.Modules.Services;

[TestFixture]
public class ConnectionAccepterServiceTests {

	[Test]
	public void NewTcpSocket_ConnectionAccepted_EventFires() {
		// Arrange
		var serverPortService = new ServerPortService();
		var threadService = new ThreadService();
		var socketFactory = new SocketFactory();
		var connectionAccepterService = new ConnectionAccepterService( serverPortService, threadService, socketFactory );
		var tcpSocket = socketFactory.CreateTcp();
		var connectionAccepted = false;
		connectionAccepterService.NewTcpSocket += ( socket ) => connectionAccepted = true;

		// Act
		tcpSocket.Connect( new IPEndPoint( IPAddress.Loopback, serverPortService.Port ) );

		while ( !connectionAccepterService.HasIncomingSockets && !connectionAccepted )
			connectionAccepterService.Update( 0, 0 );

		connectionAccepterService.Update( 0, 0 );
		connectionAccepterService.Dispose();

		// Assert
		Assert.That( connectionAccepted, Is.True );
	}

}
