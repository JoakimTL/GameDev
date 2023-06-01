using Engine.Structure.Interfaces;
using System.Net;

namespace Engine.GlobalServices;

public sealed class NetworkConnectionService : Identifiable, IGlobalService {

	public IPEndPoint? RemoteTarget { get; private set; }

	public event Action? RemoteTargetChanged;

	public NetworkConnectionService() {
		RemoteTarget = null;
	}

	public void Connect( IPEndPoint endPoint ) {
		if (endPoint is null ) {
			this.LogWarning( $"Use the {nameof(Disonnect)} method to disconnect." );
			return;
		}
		RemoteTarget = endPoint;
		RemoteTargetChanged?.Invoke();
	}

	public void Disonnect() {
		if ( RemoteTarget is null ) {
			this.LogWarning( $"Remote is already null." );
			return;
		}
		RemoteTarget = null;
		RemoteTargetChanged?.Invoke();
	}

}
