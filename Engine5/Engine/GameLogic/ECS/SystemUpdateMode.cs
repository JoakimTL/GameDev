namespace Engine.GameLogic.ECS;

/// <summary>
/// Describes the mode of updating.
/// </summary>
public enum SystemUpdateMode {
	/// <summary>
	/// Updates all entities in one call
	/// </summary>
	All,
	/// <summary>
	/// Updates entities in separate batches, separated spatially in a 2d grid. Applies only to entities with a <see cref="Engine.GameLogic.ECS.Components.Transform2Component"/>.
	/// </summary>
	Grid2,
	/// <summary>
	/// Updates entities in separate batches, separated spatially in a 3d grid. Applies only to entities with a <see cref="Engine.GameLogic.ECS.Components.Transform3Component"/>.
	/// </summary>
	Grid3
}
