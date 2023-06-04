using Engine.Networking;
using Engine.Structure.Interfaces;
using System.Net;

namespace Engine.GlobalServices.Network;

public sealed class NetworkConnectionService : Identifiable, IGlobalService {

	public IPEndPoint? RemoteTarget { get; private set; }

	public event Action? RemoteTargetChanged;

	public NetworkId? NetworkId { get; internal set; }

	public NetworkConnectionService() {
		RemoteTarget = null;
		NetworkId = null;
	}

	public void Connect( IPEndPoint endPoint ) {
		if ( endPoint is null ) {
			this.LogWarning( $"Use the {nameof( Disonnect )} method to disconnect." );
			return;
		}
		if ( endPoint == RemoteTarget ) {
			this.LogWarning( $"Remote is already {endPoint}." );
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
		NetworkId = null;
		RemoteTargetChanged?.Invoke();
	}

}
