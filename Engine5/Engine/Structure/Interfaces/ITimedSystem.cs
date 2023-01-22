namespace Engine.Structure.Interfaces;

public interface ITimedSystem : ISystem {
	int SystemTickInterval { get; }
}
