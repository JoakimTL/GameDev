namespace Engine;

public abstract class NamedIdentifiable : Identifiable {
	public string? Nickname { get; protected set; }
	public override string ToString() => string.IsNullOrEmpty( this.Nickname ) ? base.ToString() : $"{this.Nickname}#{this.UniqueId}";
}