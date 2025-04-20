namespace Engine.Modularity;

public sealed class Message( MessageQueueInterface? sender, object content, string? address ) {
	public readonly MessageQueueInterface? Sender = sender;
	public readonly object Content = content;
	public readonly string? Address = address;
}
