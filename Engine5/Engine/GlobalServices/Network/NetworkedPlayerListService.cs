using Engine.Networking;
using Engine.Structure.Interfaces;

namespace Engine.GlobalServices.Network;

public sealed class NetworkedPlayerListService : Identifiable, IGlobalService
{
    private readonly Dictionary<ulong, NetworkPlayer> _players;

    public event Action<NetworkPlayer>? PlayerAdded;
    public event Action<NetworkPlayer>? PlayerRemoved;

    public NetworkedPlayerListService()
    {
        _players = new();
    }

    public IReadOnlyCollection<NetworkPlayer> Players => _players.Values;

    internal void AddPlayer(NetworkPlayer player)
    {
        if (!_players.TryAdd(player.NetworkId, player))
        {
            this.LogWarning($"Player with network id {player.NetworkId} already exists.");
            return;
        }
        PlayerAdded?.Invoke(player);
    }

    internal void RemovePlayer(ulong playerId)
    {
        if (!_players.Remove(playerId, out var player))
        {
            this.LogWarning($"Player with network id {playerId} does not exist.");
            return;
        }
        PlayerRemoved?.Invoke(player);
    }
}
