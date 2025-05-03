namespace Engine.Standard;

public sealed record GameStateChangeMessage( string Name, object? NewState );