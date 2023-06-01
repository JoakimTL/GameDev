namespace Engine.Networking.Module.Services;

public class ServerPortService : Identifiable, INetworkServerService {

	public ushort Port { get; private set; }

	public ServerPortService() {
		Port = 50043;
	}

}
