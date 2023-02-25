namespace Engine.GameLogic.ECPS;

public abstract class SystemBase : Identifiable {

	/// <summary>
	/// Determines how the system updater handles feeding entities to <see cref="Update(IEnumerable{Entity}, float, float)"/><br/>
	/// <b><see cref="SystemUpdateMode.None"/></b> updates no entities.<br/>
	/// <b><see cref="SystemUpdateMode.All"/></b> updates all entities in the container with one call. This is useful for general game logic updates.<br/>
	/// <b><see cref="SystemUpdateMode.Grid2"/></b> and <b><see cref="SystemUpdateMode.Grid3"/></b> updates entities in grids. One entity might then be updated twice or more, because of it's precence in multiple grids, but within different collection of entities. This is useful for collision detection and other processes require nearby entities and not general game logic updates.
	/// </summary>
	public SystemUpdateMode UpdateMode { get; protected set; }

	public SystemBase() {
		UpdateMode = SystemUpdateMode.All;
	}

	public abstract void Update( IEnumerable<Entity> entities, float time, float deltaTime );
}