using Civs.World;
using Civs.World.NewWorld;

namespace Civs.Messages;

public sealed record CreateNewOwnerMessage( Face Face );
public sealed record RemoveOwnerMessage( Face Face );
public sealed record SetNeighbourOwnerMessage( Face Face, int Index );