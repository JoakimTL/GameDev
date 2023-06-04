using Engine.Structure.Interfaces;

namespace Engine.GlobalServices.Network;

public sealed class NetworkClientUsernameService : Identifiable, IGlobalService {

	public string Username { get; private set; }

	public event Action<string>? UsernameChanged;

	public NetworkClientUsernameService() {
		Username = "NoUsername";
	}

	public void SetUsername( string username ) {
		Username = username;
		UsernameChanged?.Invoke( username );
	}

}