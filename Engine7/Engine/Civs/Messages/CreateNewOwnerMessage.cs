using Civs.World;

namespace Civs.Messages;

public sealed record CreateNewOwnerMessage( Tile Tile );
public sealed record RemoveOwnerMessage( Tile Tile );
public sealed record SetNeighbourOwnerMessage( Tile Tile, int Index );