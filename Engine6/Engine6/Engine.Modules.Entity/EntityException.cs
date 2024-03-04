namespace Engine.Modules.ECS;

internal class EntityException : Exception {
	public Entity? Entity { get; }

	public EntityException( Entity e, string message, Exception? innerException ) : base( $"{message}{(message.Length > 0 ? Environment.NewLine : "")}Error stemming from {e}!", innerException ) {
		this.Entity = e;
	}

	public EntityException( Entity e, string message = "" ) : this( e, message, null ) { }
}