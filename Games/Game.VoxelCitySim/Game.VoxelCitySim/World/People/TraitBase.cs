namespace Game.VoxelCitySim.World.People;

public abstract class TraitBase {

	private float _value;
	public float Value { get => _value; protected set => _value = value; }

	public abstract string Name { get; }

	public abstract string Description { get; }

}
