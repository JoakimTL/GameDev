using Civlike.World.State;

namespace Civlike.Messages;

public sealed record CreateNewPopulationCenterMessage( Tile Tile, Guid PlayerGuid );
public sealed record RemoveOwnerMessage( Tile Tile );
public sealed record SetNeighbourOwnerMessage( Tile Tile, int Index );

public sealed record CreateNewPlayerMessage;
public sealed record CreateNewPlayerMessageResponse(Guid PlayerEntityId);