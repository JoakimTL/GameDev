using Civlike.World.GameplayState;

namespace Civlike.Messages;

public sealed record CreateNewPopulationCenterMessage( Face Face, Guid PlayerGuid );
public sealed record RemoveOwnerMessage( Face Face );
public sealed record SetNeighbourOwnerMessage( Face Face, int Index );

public sealed record CreateNewPlayerMessage;
public sealed record CreateNewPlayerMessageResponse(Guid PlayerEntityId);