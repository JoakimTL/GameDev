using Engine.Module.Entities.Container;

namespace Engine.Module.Entities.Messages;

public sealed record EntityContainerCreatedEventMessage( EntityContainer EntityContainer );
public sealed record EntityContainerRemovedEventMessage( EntityContainer EntityContainer );
