using Engine.Module.Entities.Container;

namespace Engine.Module.Entities.Messages;

public sealed record EntityContainerCreatedEventMessage( Guid ContainerId );
public sealed record EntityContainerRemovedEventMessage( Guid ContainerId );
