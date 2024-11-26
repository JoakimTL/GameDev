using Engine.Module.Entities.Container;

namespace Engine.Module.Entities.Messages;

public sealed record EntityContainerCreatedEvent( EntityContainer EntityContainer );

public sealed record EntityContainerRequestResponse( EntityContainer EntityContainer );