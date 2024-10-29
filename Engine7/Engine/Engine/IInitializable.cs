namespace Engine;

public interface IInitializable {
	/// <summary>
	/// Run on the first loop of the thread the instance is running on.
	/// </summary>
	void Initialize();
}